#!/usr/bin/env node
/**
 * scripts/validate.js —— 零依赖站点自检
 * 跑法：node scripts/validate.js
 * 检查项：
 *   1) index.html / 404.html 的必备 head 标签是否齐（canonical、og:title、og:image、manifest、theme-color）
 *   2) 页面里引用的本地相对资源是否真实存在（漏文件 = 404）
 *   3) 仓库内所有 .json 能否解析；games.json.gz 能否解压且条数对齐
 *   4) sw.js / 页面内联脚本之外的关键 .js 语法是否合法
 * 退出码：0 通过，1 有错
 */
"use strict";
const fs = require("fs");
const path = require("path");
const zlib = require("zlib");
const { execFileSync } = require("child_process");

const ROOT = path.resolve(__dirname, "..");
const errors = [];
const warns = [];
const fail = (m) => errors.push(m);
const warn = (m) => warns.push(m);

function read(p) { return fs.readFileSync(path.join(ROOT, p), "utf8"); }
function exists(p) { return fs.existsSync(path.join(ROOT, p)); }

/* ---------- 1. head 必备标签 ---------- */
const HEAD_RULES = [
  [/<link[^>]+rel=["']canonical["'][^>]*>/i, '缺少 <link rel="canonical">'],
  [/<meta[^>]+property=["']og:title["'][^>]*>/i, "缺少 og:title"],
  [/<meta[^>]+property=["']og:image["'][^>]*>/i, "缺少 og:image"],
  [/<meta[^>]+name=["']twitter:card["'][^>]*>/i, "缺少 twitter:card"],
  [/<link[^>]+rel=["']manifest["'][^>]*>/i, "缺少 manifest 声明"],
  [/<meta[^>]+name=["']theme-color["'][^>]*>/i, "缺少 theme-color"],
  [/<meta[^>]+name=["']description["'][^>]*>/i, "缺少 description"],
];
for (const page of ["index.html", "404.html"]) {
  if (!exists(page)) { fail(page + " 不存在"); continue; }
  const html = read(page);
  if (page === "index.html") {
    for (const [re, msg] of HEAD_RULES) if (!re.test(html)) fail(page + " " + msg);
  }
  if (!/<meta[^>]+name=["']viewport["'][^>]*>/i.test(html)) fail(page + " 缺少 viewport");
  if (!/<html[^>]+lang=/i.test(html)) warn(page + " <html> 没有 lang 属性");
  if (!/<title>[^<]{4,}<\/title>/i.test(html)) fail(page + " <title> 缺失或过短");
}

/* ---------- 2. 本地资源存在性 ---------- */
/* 站点绝对路径（以 / 开头）按 canonical 的 base 折算成仓库内相对路径 */
let SITE_BASE = "/";
if (exists("index.html")) {
  const cm = read("index.html").match(/<link[^>]+rel=["']canonical["'][^>]+href=["']([^"']+)["']/i);
  if (cm) {
    try { SITE_BASE = new URL(cm[1]).pathname; } catch (_) {}
    if (!SITE_BASE.endsWith("/")) SITE_BASE = SITE_BASE.replace(/[^/]*$/, "");
  }
}
function toRepoPath(ref) {
  if (ref.startsWith("/")) {
    if (SITE_BASE !== "/" && ref.startsWith(SITE_BASE)) return ref.slice(SITE_BASE.length) || ".";
    return null; // 站点根以外，无法判定
  }
  return ref;
}
const SCAN = ["index.html", "404.html", "site.webmanifest"].filter(exists);
const skipExt = /\.(?:json|png|jpg|jpeg|webp|gif|svg|ico|mp4|css|js|webmanifest|txt|xml)$/i;
for (const file of SCAN) {
  const src = read(file);
  const refs = new Set();
  for (const m of src.matchAll(/(?:src|href)\s*=\s*["']([^"']+)["']/gi)) refs.add(m[1]);
  if (file === "site.webmanifest") {
    for (const m of src.matchAll(/"src"\s*:\s*"([^"]+)"/g)) refs.add(m[1]);
  }
  for (const ref of refs) {
    if (/^(?:[a-z]+:|\/\/|#|data:)/i.test(ref)) continue;
    let clean = ref.split(/[?#]/)[0];
    if (!clean) continue;
    clean = toRepoPath(clean);
    if (clean === null || clean === "" || clean === ".") continue;
    if (!exists(clean)) fail(file + " 引用了不存在的文件: " + clean);
  }
}

/* ---------- 3. JSON 可解析 ---------- */
function walk(dir, out) {
  for (const e of fs.readdirSync(path.join(ROOT, dir), { withFileTypes: true })) {
    if (e.name === "node_modules" || e.name === ".git") continue;
    const rel = dir ? dir + "/" + e.name : e.name;
    if (e.isDirectory()) walk(rel, out);
    else out.push(rel);
  }
  return out;
}
const all = walk("", []);
for (const f of all) {
  if (!f.endsWith(".json")) continue;
  const size = fs.statSync(path.join(ROOT, f)).size;
  if (size > 40 * 1024 * 1024) { warn(f + " 超过 40MB，跳过解析"); continue; }
  try { JSON.parse(read(f)); } catch (e) { fail(f + " JSON 解析失败: " + e.message); }
}

/* ---------- 4. JS 语法 ---------- */
for (const f of all) {
  if (!/\.(?:js|mjs|cjs)$/.test(f)) continue;
  if (/^(?:frontend|client|docs)\//.test(f)) continue; // 打包产物/第三方不查
  try {
    execFileSync(process.execPath, ["--check", path.join(ROOT, f)], { stdio: "pipe" });
  } catch (e) {
    fail(f + " 语法错误: " + String(e.stderr || e.message).split("\n").slice(0, 3).join(" "));
  }
}

/* ---------- 5. 数据包契约（index / v2 / slim 三者必须同源） ---------- */
/* 页面按需 fetch data/*.json，这里把「页面要的文件」与「构建产物」对齐，
   任何一个漏生成都会在 CI 阶段炸出来，而不是等用户打开页面看到空白。 */
const DATA_FILES = new Set();
if (exists("index.html")) {
  for (const m of read("index.html").matchAll(/fetch\(\s*['"`](data\/[^'"`?]+)/g)) DATA_FILES.add(m[1]);
}
for (const f of DATA_FILES) if (!exists(f)) fail("index.html fetch 了不存在的 " + f);

function loadJSON(p) { try { return JSON.parse(read(p)); } catch (_) { return null; } }

const slim = exists("data/games.slim.json") ? loadJSON("data/games.slim.json") : null;
const idx  = exists("data/games.index.json") ? loadJSON("data/games.index.json") : null;
const v2   = exists("data/games.v2.json")   ? loadJSON("data/games.v2.json")   : null;

if (slim) {
  if (!Array.isArray(slim.games) || !slim.games.length) fail("data/games.slim.json 的 games 为空");
  else if (slim.count !== slim.games.length) fail("data/games.slim.json count=" + slim.count + " 与 games.length=" + slim.games.length + " 不一致");
}

if (!idx) fail("缺少 data/games.index.json（首屏包），页面会回落到 3MB 的 slim");
else {
  if (idx.v !== 1) fail("data/games.index.json 版本号应为 1，实际 " + idx.v);
  if (!Array.isArray(idx.games) || !idx.games.length) fail("data/games.index.json 的 games 为空");
  else if (idx.games.length > 300) warn("data/games.index.json 装了 " + idx.games.length + " 条，超过 300 会让首屏变重");
  const st = idx.stats || {};
  for (const k of ["count", "online", "avgPrice", "appid", "cats"]) {
    if (st[k] === undefined || st[k] === null) fail("data/games.index.json stats 缺 " + k);
  }
  if (slim && st.count !== slim.count) fail("index.stats.count=" + st.count + " 与 slim.count=" + slim.count + " 不一致");
  const bad = (idx.games || []).find((g) => !g || typeof g.n !== "string" || typeof g.a === "undefined");
  if (bad) fail("data/games.index.json 有缺 n/a 字段的条目: " + JSON.stringify(bad).slice(0, 80));
}

if (!v2) fail("缺少 data/games.v2.json（全量包）");
else {
  if (v2.v !== 2) fail("data/games.v2.json 版本号应为 2，实际 " + v2.v);
  if (!Array.isArray(v2.c) || !v2.c.length) fail("data/games.v2.json 的 c（分类字典）为空");
  if (!Array.isArray(v2.g) || !v2.g.length) fail("data/games.v2.json 的 g 为空");
  else {
    if (v2.count !== v2.g.length) fail("v2.count=" + v2.count + " 与 g.length=" + v2.g.length + " 不一致");
    if (slim && v2.count !== slim.count) fail("v2.count=" + v2.count + " 与 slim.count=" + slim.count + " 不一致");
    const CL = (v2.c || []).length;
    for (let i = 0; i < v2.g.length; i++) {
      const r = v2.g[i];
      if (!Array.isArray(r) || r.length !== 8) { fail("v2.g[" + i + "] 应为 8 元素数组，实际 " + (Array.isArray(r) ? r.length : typeof r)); break; }
      if (typeof r[6] !== "number" || r[6] < 0 || r[6] >= CL) { fail("v2.g[" + i + "][6] 分类下标越界: " + r[6] + " / " + CL); break; }
      if (typeof r[1] !== "string" || !r[1]) { fail("v2.g[" + i + "][1] 游戏名为空"); break; }
    }
  }
}

/* 原始归档：data/games.json.gz 必须能解压，且条目数与 slim 一致。
   它是 --slim 重建的唯一来源，坏了整条刷新链就断了，必须在 CI 里守住。 */
if (exists("data/games.json.gz")) {
  let raw = null;
  try {
    raw = JSON.parse(zlib.gunzipSync(fs.readFileSync(path.join(ROOT, "data/games.json.gz"))).toString("utf8"));
  } catch (e) {
    fail("data/games.json.gz 解压或解析失败: " + e.message);
  }
  if (raw) {
    const arr = Array.isArray(raw) ? raw : raw.data;
    if (!Array.isArray(arr) || !arr.length) fail("data/games.json.gz 的 data 不是非空数组");
    else if (slim && arr.length !== slim.count) fail("games.json.gz 条数 " + arr.length + " 与 slim.count=" + slim.count + " 不一致");
    else {
      const bad = arr.find((g) => !g || typeof g.ID !== "number" || !g.Name);
      if (bad) fail("games.json.gz 存在缺 ID/Name 的条目");
    }
  }
} else {
  warn("缺少 data/games.json.gz（原始归档），--slim 重建将无源可用");
}

/* ---------- 输出 ---------- */
for (const w of warns) console.log("::warning::" + w);
for (const e of errors) console.log("::error::" + e);
console.log(`检查完成：${all.length} 个文件，${errors.length} 个错误，${warns.length} 个警告`);
if (errors.length) process.exit(1);
