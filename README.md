# 租号么 · 公开数据镜像 + 客户端

[租号么](https://zuhaome.com) 公开接口的数据快照、客户端分析源码、接口文档，以及一个可搜索的静态数据浏览器。

> **在线浏览**：<https://aq18270592902-cloud.github.io/zuhaome-db/>
> 25122 款游戏，可按分类 / 价格 / 名称筛选。

---

## 这是什么

| 部分 | 说明 |
|---|---|
| `index.html` | 静态数据浏览器，纯前端，直接读仓库里的 JSON |
| `data/games.slim.json` | 25122 款游戏精简数据（3MB，页面用这个） |
| `data/games.json` | 完整原始数据（26.8MB，含全部字段） |
| `data/categories.json` | 20 个游戏分类 |
| `data/membership-plans.json` | 10 个会员时长套餐（价格单位：分） |
| `data/announcements.json` | 站内公告 |
| `data/faqs.json` | 常见问题 |
| `client/租号么免费租号器.exe` | 官方客户端（106KB，真 PE） |
| `src/MainForm2.cs` | 客户端反编译还原（117KB，登录 / 下单 / 心跳全在里面） |
| `api/` | 接口清单、endpoints.json、OpenAPI 导出 |
| `docs/` | 3 份分析报告 + Excel 数据导出 |
| `frontend/` | 官网前端资源（55 文件） |
| `images/` | 21 张截图 |

---

## 数据字段

`games.slim.json` 结构：`{"count": 25122, "games": [...]}`，每项：

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

完整版 `games.json` 里 `CategoryNames` 是数组、`RentPrices` 是 `[{"hours":1,"price":0.3},…,{"hours":24,"price":1.5}]` 这样的阶梯价。

---

## 接口速查

```
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

后台登录页：<https://admin.zuhaome.com/pxxxasl-7x9k2m/login>

**CORS**：`api.zuhaome.com` 返回 `{"error":"CORS origin not allowed"}` 403，**浏览器跨域直连被拒**。所以网页版只能读静态快照，不能做实时查询。要做实时查询得自建后端反代。

---

## 自己重新抓数据

```bash
curl "https://api.zuhaome.com/api/v1/games?page=1&page_size=100" \
  -H "accept: application/json"
```

分页循环即可，`page_size` 上限实测 100。共 25122 条 → 252 页。

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

## 目录

```
zuhaome-db/
├── index.html          数据浏览器（GitHub Pages 入口）
├── data/               games.json / games.slim.json / categories / plans / 公告 / FAQ
├── docs/               3 份分析报告 + Excel
├── api/                接口清单 / endpoints.json / OpenAPI
├── src/                客户端反编译源码
├── client/             官方 exe + games.tsv
├── frontend/           官网前端资源
└── images/             截图
```

---

## 一句话

25122 款游戏的公开数据全在这个仓库，网页打开就能搜；exe 和反编译源码也在，接口文档在 `api/`。
