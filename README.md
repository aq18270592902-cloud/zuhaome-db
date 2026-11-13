<div align="center">

<img src="./assets/og.png" alt="租号么 · 数据浏览器" width="720">

# 租号么 · 公开数据镜像 + 客户端

25280 款游戏的公开数据快照、客户端反编译源码、接口文档，以及一个可搜索的纯前端数据浏览器。

[![Pages](https://img.shields.io/badge/demo-online-ff6b35?style=flat-square&logo=githubpages&logoColor=white)](https://aq18270592902-cloud.github.io/zuhaome-db/)
[![Release](https://img.shields.io/github/v/release/aq18270592902-cloud/zuhaome-db?style=flat-square&color=f59e0b&label=release)](https://github.com/aq18270592902-cloud/zuhaome-db/releases)
[![License](https://img.shields.io/github/license/aq18270592902-cloud/zuhaome-db?style=flat-square&color=34d399)](./LICENSE)
[![Games](https://img.shields.io/badge/games-25%2C280-22d3ee?style=flat-square)](./data/games.slim.json)
[![Stars](https://img.shields.io/github/stars/aq18270592902-cloud/zuhaome-db?style=flat-square&color=ff6b35)](https://github.com/aq18270592902-cloud/zuhaome-db/stargazers)
[![Last commit](https://img.shields.io/github/last-commit/aq18270592902-cloud/zuhaome-db?style=flat-square&color=9aa3b8)](https://github.com/aq18270592902-cloud/zuhaome-db/commits/main)

**[在线浏览](https://aq18270592902-cloud.github.io/zuhaome-db/)** ·
**[客户端下载](https://github.com/aq18270592902-cloud/zuhaome-db/releases)** ·
**[接口文档](./api/api-endpoints.md)** ·
**[逆向报告](./docs/租号么后台逆向报告.md)**

</div>

---

## 三十秒上手

打开 <https://aq18270592902-cloud.github.io/zuhaome-db/> → 左侧选分类或直接搜名字 → 点「加载更多」翻页。

25280 款游戏，可按分类 / 价格 / 名称 / AppID 筛选，纯前端，数据直接读仓库里的 JSON，没有任何后端。

首屏只下载 `games.index.json`（gzip 后约 9KB，240 款热门游戏 + 统计数字），页面渲染完成后在后台静默拉全量 `games.v2.json`（gzip 约 640KB），拉完自动补齐「加载更多」。所以打开就是秒开，不用等 3MB 的 JSON。

---

## 仓库结构

```
zuhaome-db/
├── index.html                  数据浏览器（单文件，GitHub Pages 入口）
├── site.webmanifest            PWA 清单
├── 404.html                    自定义错误页
├── assets/                     og.png 分享图 + 4 尺寸图标
├── data/
│   ├── games.index.json        首屏包（240 款 + 统计，30KB，页面先读这个）
│   ├── games.v2.json           全量紧凑包（25280 款，1.7MB，首屏后懒加载）
│   ├── games.slim.json         全量可读版（3MB，回退用 / 第三方引用）
│   ├── games.json.gz           完整原始数据 gzip（1.3MB，解压后 27.6MB，含全部字段）
│   ├── categories.json         20 个分类
│   ├── membership-plans.json   11 个会员套餐（价格单位：分）
│   ├── announcements.json      站内公告
│   ├── faqs.json               常见问题
│   └── client-download.json    客户端下载元信息
├── scripts/
│   ├── refresh-games.js        一键刷新 data/（拉上游全量 + 侧栏，带三道校验闸）
│   └── validate.js             CI 用：校验 SEO 标签 / 本地资源 / JSON 合法性
├── .github/workflows/ci.yml    push 时自动跑 validate.js + node --check
├── api/                        接口清单 / endpoints.json / OpenAPI 导出
├── docs/                       3 份分析报告 + Excel 数据导出
├── src/                        客户端反编译源码（MainForm2.cs 117KB）
├── client/                     官方 exe + games.tsv
├── frontend/                   官网前端资源（55 文件）
└── images/                     21 张截图
```

---

## 数据字段

`games.slim.json` 结构：`{"count": 25280, "games": [...]}`，每项：

```json
{ "i": 14, "n": "揍击派对", "e": "Pummel Party", "a": 880940, "p": 58, "r": 11.6, "c": "联机游戏/动作/休闲/独立", "o": 1 }
```

| 字段 | 含义 |
|---|---|
| `i` | 站内游戏 ID |
| `n` | 中文名 |
| `e` | 英文名 |
| `a` | Steam AppID（5189 款没有） |
| `p` | Steam 售价（元，平均 57.4） |
| `r` | 时租价格（元/小时） |
| `c` | 分类，斜杠分隔 |
| `o` | 是否在线可租（快照里全部为 1） |

完整版 `games.json.gz` 里 `CategoryNames` 是数组、`RentPrices` 是 `[{"hours":1,"price":0.3},…,{"hours":24,"price":1.5}]` 这样的阶梯价。

原始包以 gzip 归档（27.6MB → 1.3MB，压缩比 5%）。解压一条命令：

```bash
gunzip -k data/games.json.gz            # 得到 data/games.json，内容与上游逐字节一致
node -e "console.log(require('./data/games.json').data.length)"   # 25280
```

不解压也行，Node 里直接读：

```js
const zlib = require('zlib'), fs = require('fs');
const { data } = JSON.parse(zlib.gunzipSync(fs.readFileSync('data/games.json.gz')));
```

---

## 接口速查

```http
GET https://api.zuhaome.com/api/v1/games?page=1&page_size=20
GET https://api.zuhaome.com/api/v1/categories
GET https://api.zuhaome.com/api/v1/announcements
GET https://api.zuhaome.com/api/v1/faqs
GET https://api.zuhaome.com/api/v1/membership/plans
GET https://api.zuhaome.com/api/v1/client-download
```

游戏图标（可直接热链，无防盗链）：

```
https://api.zuhaome.com/uploads/games/game_appid_{SteamAppID}.jpg
```

**CORS**：`api.zuhaome.com` 返回 `{"error":"CORS origin not allowed"}` 403，**浏览器跨域直连被拒**。所以网页版只能读静态快照，不能做实时查询。要做实时查询得自建后端反代。

---

## 自己重新抓数据

### 推荐：用仓库自带的脚本

```bash
node scripts/refresh-games.js            # 拉全量 + 侧栏，重写 data/ 七个文件
node scripts/refresh-games.js --check    # 只比对，有变化 exit 2（适合挂 CI 定时巡检）
node scripts/refresh-games.js --slim     # 跳过网络，用本地 data/games.json.gz 重建 slim/tsv
```

脚本内置三道闸，数据不对就拒绝写盘：条目数 < 1000、ID 非数字、ID 重复。
侧栏四个接口（分类 / 公告 / FAQ / 套餐）失败不阻塞主流程，保留旧快照并打日志。
上游响应里的 `server_time` / `refresh_interval_seconds` 每次都会变，写盘前已剥掉，避免无意义 diff。

### 手动：一次请求就是全量

```bash
curl "https://api.zuhaome.com/api/v1/games" \
  -H "accept: application/json" -H "user-agent: Mozilla/5.0" \
  -o games.raw.json
```

**分页参数无效**：`page` / `page_size` / `pageSize` / `limit` 实测全部被忽略，
不管传什么都恒定返回全部 25280 条（约 26.9MB，耗时 40s 左右）。所以不要写分页循环 —— 一次拿完。

响应是 `{"data":[...]}`，34 个字段全留；`scripts/refresh-games.js` 里 `slim()` 负责压成页面用的 8 个字段。

---

## 客户端

`client/租号么免费租号器.exe` 是官方下发版本，也可直接从上游拉：

```
https://api.zuhaome.com/api/v1/client-download/file
```

从源码重编译（`src/`）：

```bat
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:租号么.exe MainForm2.cs AssemblyInfo.cs /win32icon:app.ico
```

---

## 本地预览

```bash
python -m http.server 8000
# 打开 http://127.0.0.1:8000/
```

必须用 HTTP 服务打开 —— `file://` 下 `fetch('data/games.slim.json')` 会被同源策略拦掉，页面只剩「数据加载失败」。

---

## 排障

| 现象 | 原因 | 处理 |
|---|---|---|
| 页面提示「数据加载失败」 | 用 `file://` 打开，或 `data/` 目录没上传 | 起 HTTP 服务；确认 `data/games.slim.json` 在 |
| 封面全是 🎮 占位 | `api.zuhaome.com` 的图被网络拦了 | 图片走的是上游热链，与仓库无关；挂代理或忽略 |
| 分类下拉是空的 | `data/categories.json` 结构变了 | 文件根是 `{data:[...]}`，代码两种都兼容，检查是否 404 |
| 「加载更多」点不动 | 已到列表末尾 | 按钮会显示「已显示 N / N」并自动隐藏 |
| 搜不到某个游戏 | 搜索匹配中英文名 / AppID / 站内 ID | 用 AppID 数字搜最准 |

---

## 目录之外的资料

- [`api/api-endpoints.md`](./api/api-endpoints.md) —— 完整接口清单
- [`api/save-openapi.json`](./api/save-openapi.json) —— OpenAPI 3 导出
- [`docs/租号么后台逆向报告.md`](./docs/租号么后台逆向报告.md) —— 后台接口与权限边界
- [`docs/第二阶段-客户端逆向与权限边界报告.md`](./docs/第二阶段-客户端逆向与权限边界报告.md) —— 客户端分析
- [`docs/租号么-公开数据导出.xlsx`](./docs/租号么-公开数据导出.xlsx) —— Excel 版数据

---

## 一句话

25280 款游戏的公开数据全在这个仓库，网页打开就能搜；exe 和反编译源码也在，接口文档在 `api/`。
