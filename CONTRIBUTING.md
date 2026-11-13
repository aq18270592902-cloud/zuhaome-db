# 参与开发

## 这个仓库是什么

租号么的**公开数据镜像** + 客户端逆向源码 + 一个纯静态的数据浏览器页面。没有 npm 依赖，脚本都是 Node 20 直跑。

## 目录速览

| 目录 | 干什么 |
|---|---|
| `data/` | 数据快照（分片：`games.index.json` 首屏 / `games.v2.json` 全量 / `games.slim.json` 回退 / `games.json.gz` 原始归档） |
| `scripts/` | `refresh-games.js` 抓数据、`validate.js` 校验 |
| `frontend/` | 官网前端资源存档 |
| `src/` | 客户端反编译源码（C#） |
| `api/` | 接口文档与 OpenAPI |
| `images/` | 截图（**已打码**，账号密码区做过马赛克） |
| `docs/` | 逆向报告 + Excel 数据导出 |

## 本地怎么跑

```bash
# 起静态服务器看数据浏览器
python -m http.server 8080
# 打开 http://127.0.0.1:8080/

# 重新抓数据（会改写 data/，注意先备份）
node scripts/refresh-games.js --check     # 只探测，不写
node scripts/refresh-games.js --slim      # 重建 slim 分片
node scripts/refresh-games.js             # 全量重抓
```

## 提交前必须跑的校验

```bash
node scripts/validate.js
```

它会检查：JSON 可解析、页面引用的每个数据包都真实存在、`games.index.json` 契约（`v===1`、条目 ≤300、stats 五项齐全且 `count` 与 slim 一致）、`games.v2.json` 契约（`v===2`、每行 8 个字段、`cidx` 不越界）、head 标签、robots/sitemap 死链。

再加：

```bash
for f in scripts/*.js *.js; do [ -e "$f" ] || continue; node --check "$f"; done
```

## 数据文件的规矩（重要）

这是这个仓库最容易出事的地方。

1. **抓数据必须带三道闸**（`refresh-games.js` 里已有，别删）：
   - 分页参数无效 → 靠 `--check` 比对条数，骤降就中止
   - 服务器时间字段易变 → 剥离 `server_time` / `refresh_interval_seconds`，否则每次都是无意义 diff
   - 条数下限 → 新数据比旧数据少 5% 以上直接 exit 1
2. **不要手工编辑 `games.*.json`**——它们是脚本产物，下次刷新就被覆盖。要改就改脚本再重跑。
3. **提交前扫一遍凭证**：账号、密码、手机号、cookie、token。历史上出过明文进仓库的事。
4. **截图必须打码**。`images/` 里的图账号密码区都做过马赛克，新增截图照这个标准做。

## 改代码时的约定

| 位置 | 约定 |
|---|---|
| `index.html` | 首屏只加载 `games.index.json`（约 9KB gzip），全量在后台异步拉 `games.v2.json`。**不要改回一次拉全量** |
| `sw.js` | 改缓存策略必须升 `CACHE` 版本号（现在是 `zuhaome-v3`）；`games.v2.json` 故意不进 CORE，避免 install 阶段阻塞 |
| `scripts/validate.js` | 新增数据文件要同步加契约检查 |
| `src/` | 反编译产物，加注释可以，别改逻辑——它和真实客户端对不上就没参考价值了 |

## 提交信息

```
fix(data): 修复首页价格排序把无价商品排最前
perf(data): 首屏分片从 754KB 降到 9KB
docs(readme): 补分片结构说明
chore(images): 截图账号密码打码
```

## 不要做的事

- 不要提交明文凭证（账号、密码、手机号、cookie、token、私钥）
- 不要提交未打码的客户端截图
- 不要手工改 `data/*.json`
- 不要在 README 里贴真实账号密码当「示例」

## 报问题

开 issue 用模板。安全问题（尤其是还有明文凭证）走 [SECURITY.md](./SECURITY.md)，**不要开公开 issue**。
