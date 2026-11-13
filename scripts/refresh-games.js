#!/usr/bin/env node
/**
 * scripts/refresh-games.js —— 从 api.zuhaome.com 拉全量游戏数据，重建 data/ 下的快照
 *
 * 跑法：
 *   node scripts/refresh-games.js            # 拉取并写入 data/
 *   node scripts/refresh-games.js --check    # 只比对，不写盘（有差异则 exit 2）
 *   node scripts/refresh-games.js --slim     # 只重建 slim/tsv（用本地已有 games.json）
 *   node scripts/refresh-games.js --no-raw   # 不重写 27MB 的 games.json（定时任务用，省仓库体积）
 *
 * 产出：
 *   data/games.json            全量原始数据 {"data":[...]}   （26MB 左右，34 字段全留）
 *   data/games.slim.json       精简数据 {"count":N,"games":[{i,n,e,a,p,r,c,o}]}（3MB，页面用这个）
 *   data/games.tsv             "id\t名称" 每行一条，便于 grep
 *   data/categories.json       分类列表
 *   data/announcements.json    站内公告
 *   data/faqs.json             常见问题
 *   data/membership-plans.json 会员套餐
 *
 * 退出码：0 成功 / 1 失败 / 2 --check 模式发现差异
 */
"use strict";
const fs = require("fs");
const path = require("path");

const ROOT = path.resolve(__dirname, "..");
const DATA = path.join(ROOT, "data");
const BASE = process.env.ZUHAOME_API || "https://api.zuhaome.com/api/v1";
const UA =
  "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
  "(KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";

const CHECK = process.argv.includes("--check");
const SLIM_ONLY = process.argv.includes("--slim");
const NO_RAW = process.argv.includes("--no-raw");

const log = (...a) => console.log("[" + new Date().toISOString() + "]", ...a);

/** 上游响应里带 server_time / refresh_interval_seconds 这类每次都会变的字段，
 *  原样落盘会让快照每次都产生无意义 diff（实测只有这两处变化）。写盘前剥掉。 */
function stripVolatile(obj) {
  if (Array.isArray(obj)) return obj.map(stripVolatile);
  if (obj && typeof obj === "object") {
    const out = {};
    for (const [k, v] of Object.entries(obj)) {
      if (k === "server_time" || k === "refresh_interval_seconds") continue;
      out[k] = stripVolatile(v);
    }
    return out;
  }
  return obj;
}

async function getJSON(url, timeoutMs = 180000) {
  const ac = new AbortController();
  const timer = setTimeout(() => ac.abort(), timeoutMs);
  try {
    const res = await fetch(url, {
      headers: { "User-Agent": UA, Accept: "application/json" },
      signal: ac.signal,
    });
    if (!res.ok) throw new Error(url + " -> HTTP " + res.status);
    const text = await res.text();
    try {
      return JSON.parse(text);
    } catch (e) {
      throw new Error(url + " 返回的不是合法 JSON：" + text.slice(0, 160));
    }
  } finally {
    clearTimeout(timer);
  }
}

/** 上游 34 字段 -> 页面用的 8 字段。规则必须与历史快照完全一致，否则页面筛选会漂。 */
function slim(g) {
  const cats = (g.CategoryNames || []).filter((c) => c);
  return {
    i: g.ID,
    n: g.Name || "",
    e: g.SourceName || "",
    a: g.SteamAppID === null || g.SteamAppID === undefined ? 0 : g.SteamAppID,
    p: g.SteamPrice || 0,
    r: g.UnlimitedPrice || 0,
    c: cats.join("/"),
    o: g.IsOnline ? 1 : 0,
  };
}

function readOldSlim() {
  try {
    return JSON.parse(fs.readFileSync(path.join(DATA, "games.slim.json"), "utf8"));
  } catch (_) {
    return { count: 0, games: [] };
  }
}

function writeJSON(file, obj) {
  const body = JSON.stringify(obj);
  fs.writeFileSync(path.join(DATA, file), body);
  return Buffer.byteLength(body);
}

async function main() {
  fs.mkdirSync(DATA, { recursive: true });

  /* ---------- 1. 全量游戏 ---------- */
  let raw;
  if (SLIM_ONLY) {
    log("--slim：跳过网络请求，直接用本地 data/games.json 重建");
    raw = JSON.parse(fs.readFileSync(path.join(DATA, "games.json"), "utf8"));
  } else {
    log("拉取全量游戏列表 …（上游不分页，一次返回约 27MB / 25000+ 条，耗时 40s 左右）");
    const t0 = Date.now();
    raw = await getJSON(BASE + "/games");
    log("游戏列表已取回，耗时 " + ((Date.now() - t0) / 1000).toFixed(1) + "s");
  }

  const games = Array.isArray(raw) ? raw : raw.data;
  if (!Array.isArray(games) || games.length < 1000) {
    throw new Error("游戏列表异常：只拿到 " + (Array.isArray(games) ? games.length : "非数组") + " 条，拒绝写入");
  }
  const ids = new Set();
  for (const g of games) {
    if (!g || typeof g.ID !== "number" || !g.Name) throw new Error("存在缺少 ID / Name 的条目，拒绝写入");
    if (ids.has(g.ID)) throw new Error("游戏 ID 重复：" + g.ID + "，拒绝写入");
    ids.add(g.ID);
  }
  log("游戏总数 " + games.length + "（ID 唯一性已校验）");

  /* ---------- 2. 精简版 + TSV ---------- */
  const slimGames = games.map(slim);
  const oldSlim = readOldSlim();
  const oldIds = new Set(oldSlim.games.map((g) => g.i));
  const newIds = new Set(slimGames.map((g) => g.i));
  const added = slimGames.filter((g) => !oldIds.has(g.i));
  const removed = oldSlim.games.filter((g) => !newIds.has(g.i));
  log(
    "对比旧快照：" + oldSlim.games.length + " -> " + slimGames.length +
    "，新增 " + added.length + "，下架 " + removed.length
  );
  if (added.length) log("新增示例：" + added.slice(0, 5).map((g) => g.i + " " + g.n).join(" / "));
  if (removed.length) log("下架示例：" + removed.slice(0, 5).map((g) => g.i + " " + g.n).join(" / "));

  /* ---------- 3. 侧栏数据（失败不阻塞主流程） ---------- */
  const side = {};
  if (!SLIM_ONLY && !CHECK) {
    for (const [file, url, pick] of [
      ["categories.json", "/categories", (j) => ({ data: j.data || j })],
      ["announcements.json", "/announcements", (j) => stripVolatile(j)],
      ["faqs.json", "/faqs", (j) => j],
      ["membership-plans.json", "/membership/plans", (j) => j],
    ]) {
      try {
        const j = await getJSON(BASE + url, 30000);
        side[file] = pick(j);
        log("侧栏 " + file + " 已取回");
      } catch (e) {
        log("侧栏 " + file + " 取回失败（保留旧快照）：" + e.message);
      }
    }
  }

  if (CHECK) {
    const changed =
      added.length > 0 ||
      removed.length > 0 ||
      games.length !== oldSlim.games.length ||
      JSON.stringify(slimGames) !== JSON.stringify(oldSlim.games);
    if (changed) {
      log("--check：数据有变化（新增 " + added.length + " / 下架 " + removed.length + "）");
      process.exit(2);
    }
    log("--check：数据无变化");
    return;
  }

  /* ---------- 4. 落盘 ---------- */
  if (!SLIM_ONLY && !NO_RAW) {
    const n1 = writeJSON("games.json", { data: games });
    log("写入 data/games.json " + (n1 / 1048576).toFixed(2) + "MB");
  } else if (NO_RAW) {
    log("--no-raw：跳过 data/games.json（27MB 大文件不进定时提交）");
  }
  const n2 = writeJSON("games.slim.json", { count: slimGames.length, games: slimGames });
  log("写入 data/games.slim.json " + (n2 / 1048576).toFixed(2) + "MB");

  const tsv = slimGames.map((g) => g.i + "\t" + g.n).join("\n") + "\n";
  fs.writeFileSync(path.join(DATA, "games.tsv"), tsv);
  log("写入 data/games.tsv " + (Buffer.byteLength(tsv) / 1024).toFixed(0) + "KB");

  for (const [file, obj] of Object.entries(side)) {
    const n = writeJSON(file, obj);
    log("写入 data/" + file + " " + n + "B");
  }
}

main().catch((e) => {
  console.error("[!] 刷新失败：" + e.message);
  process.exit(1);
});
