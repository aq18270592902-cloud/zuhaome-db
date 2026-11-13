/* sw.js — 离线缓存
 * HTML 与 JSON 数据走 network-first（保证内容新鲜），其余静态资源走 stale-while-revalidate。
 * 升级：改 CACHE 版本号即可让老缓存整批作废。
 */
const CACHE = "zuhaome-v3";
/* 预缓存只放首屏必需的小文件：27KB 的首屏包 + 分类 + 图标。
   全量包 games.v2.json 有 1.7MB，放 REVALIDATE 里首次访问后再缓存，不阻塞 install。 */
const CORE = ["./","./index.html","./site.webmanifest","./assets/og.png","./assets/icon-192.png","./assets/icon-512.png","./favicon.ico","./data/games.index.json","./data/categories.json"];
const REVALIDATE = new Set(["./index.html","./","./data/games.index.json","./data/games.v2.json","./data/games.slim.json","./data/categories.json","./data/membership-plans.json","./data/announcements.json"]);

const SCOPE = new URL(self.registration.scope).pathname;   // 例如 /zuhaome-db/
const toKey = (p) => (p.startsWith(SCOPE) ? "./" + p.slice(SCOPE.length) : p);

/* 只把成功的同源响应写进缓存：404 / 500 / opaque 一律不落盘，
   否则一次抖动就会把错误页永久钉死在客户端。 */
function put(req, res) {
  if (!res || res.status !== 200 || res.type === "opaque") return res;
  const copy = res.clone();
  caches.open(CACHE).then((c) => c.put(req, copy)).catch(() => {});
  return res;
}

self.addEventListener("install", (e) => {
  e.waitUntil(
    caches.open(CACHE)
      .then((c) => Promise.allSettled(CORE.map((u) => c.add(new Request(u, { cache: "reload" })))))
      .then(() => self.skipWaiting())
  );
});

self.addEventListener("activate", (e) => {
  e.waitUntil(
    caches.keys()
      .then((keys) => Promise.all(keys.filter((k) => k !== CACHE).map((k) => caches.delete(k))))
      .then(() => self.clients.claim())
  );
});

self.addEventListener("fetch", (e) => {
  const req = e.request;
  if (req.method !== "GET") return;

  const url = new URL(req.url);
  if (url.origin !== self.location.origin) return;          // 跨域上游 API / CDN 一律不介入
  if (url.pathname.includes("/api/")) return;               // 接口不缓存

  const key = toKey(url.pathname);

  if (req.mode === "navigate" || REVALIDATE.has(key)) {
    e.respondWith(
      fetch(req)
        .then((res) => put(req, res))
        .catch(() => caches.match(req).then((hit) => hit || caches.match("./index.html")))
    );
    return;
  }

  e.respondWith(
    caches.match(req).then((hit) => {
      const net = fetch(req).then((res) => put(req, res)).catch(() => hit);
      return hit || net;
    })
  );
});
