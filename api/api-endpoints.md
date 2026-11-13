# 租号么ERP 管理后台 — 接口全量清单

提取自 `https://admin.zuhaome.com/assets/*.js`（登录页 + 22 个页面 chunk 静态反编译）。

- API Base：`https://api.zuhaome.com/api/v1`
- 认证：`Authorization: Bearer <localStorage.admin_token>`
- 错误体：`{"error":"..."}`；401 时前端清 token 并跳 `/pxxxasl-7x9k2m/login`
- 请求头：`Content-Type: application/json`；FormData 上传时不设


## open-platforms  (31)

```
POST   /admin/open-platforms/agent-apps?${i.toString()}
PATCH  /admin/open-platforms/agent-apps/${i}/status
PUT    /admin/open-platforms/agent-apps/${i}
POST   /admin/open-platforms/agent-apps
PATCH  /admin/open-platforms/apps/${a}
DELETE /admin/open-platforms/goods-mapping-groups?${i.toString()}
PUT    /admin/open-platforms/goods-mapping-groups
DELETE /admin/open-platforms/goods-mappings/${i}
DELETE /admin/open-platforms/orders?${i.toString()}
DELETE /admin/open-platforms/orders?provider=${i}&older_than_days=${r}
PATCH  /admin/open-platforms/refund-automation?${i.toString()}
POST   /admin/open-platforms/refund-automation/run
PATCH  /admin/open-platforms/refund-automation
GET    /admin/open-platforms/remote-products?${i.toString()}
GET    /admin/open-platforms/remote-products/preview?${r.toString()}
PATCH  /admin/open-platforms/ship-notes?${c.toString()}
PATCH  /admin/open-platforms/ship-notes?provider=${r}&app_id=${i.app_id}
GET    /admin/open-platforms/taobao-items?${i.toString()}
GET    /admin/open-platforms/taobao-items/jobs?${i.toString()}
POST   /admin/open-platforms/taobao-items/jobs/${r}/retry
GET    /admin/open-platforms/taobao-items/jobs/${r}
POST   /admin/open-platforms/taobao-items/jobs
POST   /admin/open-platforms/taobao-items/sync
POST   /admin/open-platforms/taobao-orders?${i.toString()}
POST   /admin/open-platforms/taobao-orders/${r}/allocate
POST   /admin/open-platforms/taobao-orders/${r}/review
POST   /admin/open-platforms/taobao-orders/${r}/ship-card
POST   /admin/open-platforms/taobao-orders/${r}/ship
POST   /admin/open-platforms/taobao-orders/sync
PATCH  /admin/open-platforms/taobao-settings/tmall
PATCH  /admin/open-platforms/taobao-settings
```

## games  (23)

```
POST   /admin/games/${a}/blocked-accounts/resolve
PUT    /admin/games/${a}/blocked-accounts
POST   /admin/games/${a}/priority-accounts/resolve
PUT    /admin/games/${a}/priority-accounts
POST   /admin/games/${i}/blocked-accounts/import
PUT    /admin/games/${i}/blocked-accounts
POST   /admin/games/${i}/merge
PUT    /admin/games/${i}/priority-accounts
PUT    /admin/games/${i}/rent-prices
DELETE /admin/games/${i}${r?
PUT    /admin/games/${i}
POST   /admin/games/delete-empty
POST   /admin/games/delist-keywords/apply
PUT    /admin/games/delist-keywords
PATCH  /admin/games/display-config
POST   /admin/games/import
PUT    /admin/games/rent-price-config
DELETE /admin/games/rent/orders?${i.toString()}
POST   /admin/games/unlimited-price/auto-adjust
GET    /admin/games/unlimited/orders?${i.toString()}
DELETE /admin/games/unlimited/orders/${i}
POST   /admin/games/upload-image
POST   /admin/games
```

## accounts  (16)

```
GET    /admin/accounts/${a}/family-checks/${i}
POST   /admin/accounts/${a}/family-checks
GET    /admin/accounts/${a}/status
POST   /admin/accounts/${a}/token
POST   /admin/accounts/${i}/family-checks/${r}/confirm-maintenance
POST   /admin/accounts/${i}/force-offline
DELETE /admin/accounts/${i}
PUT    /admin/accounts/${i}
POST   /admin/accounts/bulk-delete/resolve
POST   /admin/accounts/bulk-delete
POST   /admin/accounts/import/jobs/${encodeURIComponent(a)}/cancel
POST   /admin/accounts/import/jobs/${encodeURIComponent(a)}
POST   /admin/accounts/import/start
GET    /admin/accounts/update-games/jobs/${encodeURIComponent(a)}
POST   /admin/accounts/update-games/start
POST   /admin/accounts
```

## auth  (8)

```
DELETE /admin/auth/events?${i.toString()}
DELETE /admin/auth/events?older_than_days=${i}
DELETE /admin/auth/guest-cleanup?older_than_days=${i}
DELETE /admin/auth/guest-cleanup/preview?older_than_days=${a}
GET    /admin/auth/identities?${i.toString()}
POST   /admin/auth/login
PATCH  /admin/auth/settings
POST   /admin/auth/users/${i}/unbind-wechat${c}
```

## membership  (8)

```
GET    /admin/membership/orders?${i.toString()}
PATCH  /admin/membership/orders/${i}/pay-status
PATCH  /admin/membership/orders/${i}/proof-review
DELETE /admin/membership/orders/${i}
POST   /admin/membership/plans?${i.toString()}
PATCH  /admin/membership/plans/${i}/status
PUT    /admin/membership/plans/${i}
POST   /admin/membership/plans
```

## faqs  (5)

```
PATCH  /admin/faqs/${i}/status
DELETE /admin/faqs/${i}
PUT    /admin/faqs/${i}
POST   /admin/faqs/upload-image
POST   /admin/faqs
```

## users  (5)

```
POST   /admin/users/${i}/reset-today-supply
DELETE /admin/users/${i}
PUT    /admin/users/${i}
POST   /admin/users/reset-silver
POST   /admin/users
```

## admins  (4)

```
PATCH  /admin/admins/${i}/status
DELETE /admin/admins/${i}
PUT    /admin/admins/${i}
POST   /admin/admins
```

## announcements  (4)

```
PATCH  /admin/announcements/${i}/status
DELETE /admin/announcements/${i}
PUT    /admin/announcements/${i}
POST   /admin/announcements
```

## categories  (4)

```
PATCH  /admin/categories/${i}/status
DELETE /admin/categories/${i}
PUT    /admin/categories/${i}
POST   /admin/categories
```

## cdks  (4)

```
DELETE /admin/cdks/${i}
PUT    /admin/cdks/${i}
POST   /admin/cdks/batch-generate
POST   /admin/cdks/fixed-generate
```

## faq-categories  (4)

```
PATCH  /admin/faq-categories/${i}/status
DELETE /admin/faq-categories/${i}
PUT    /admin/faq-categories/${i}
POST   /admin/faq-categories
```

## notifications  (4)

```
PATCH  /admin/notifications/${i}/status
DELETE /admin/notifications/${i}
PUT    /admin/notifications/${i}
POST   /admin/notifications
```

## orders  (4)

```
GET    /admin/orders/${a}/credential
POST   /admin/orders/${i}/block-dispatch
DELETE /admin/orders/${i}
PUT    /admin/orders/${i}
```

## tasks  (4)

```
PATCH  /admin/tasks/${i}/status
DELETE /admin/tasks/${i}
PUT    /admin/tasks/${i}
POST   /admin/tasks
```

## dashboard  (3)

```
GET    /admin/dashboard/breakdown?${i.toString()}
GET    /admin/dashboard/overview${r?
GET    /admin/dashboard/trend?${i.toString()}
```

## client-download  (2)

```
PATCH  /admin/client-download/config
POST   /admin/client-download/upload
```

## feedbacks  (2)

```
PATCH  /admin/feedbacks/${i}/status
DELETE /admin/feedbacks/cleanup?older_than_days=${i}
```

## game-stock-alerts  (2)

```
PATCH  /admin/game-stock-alerts/config
POST   /admin/game-stock-alerts/run
```

## accounts?${r.toString()}  (1)

```
POST   /admin/accounts?${r.toString()}
```

## admins?${i.toString()}  (1)

```
POST   /admin/admins?${i.toString()}
```

## announcements?${i.toString()}  (1)

```
POST   /admin/announcements?${i.toString()}
```

## categories?${i.toString()}  (1)

```
POST   /admin/categories?${i.toString()}
```

## categories?page=1&size=500  (1)

```
GET    /admin/categories?page=1&size=500
```

## cdks?${i.toString()}  (1)

```
POST   /admin/cdks?${i.toString()}
```

## faq-categories?${i.toString()}  (1)

```
POST   /admin/faq-categories?${i.toString()}
```

## faqs?${i.toString()}  (1)

```
POST   /admin/faqs?${i.toString()}
```

## feedbacks?${i.toString()}  (1)

```
PATCH  /admin/feedbacks?${i.toString()}
```

## game-stock-alerts?${i.toString()}  (1)

```
GET    /admin/game-stock-alerts?${i.toString()}
```

## games?${i.toString()}  (1)

```
GET    /admin/games?${i.toString()}
```

## games?${y.toString()}  (1)

```
GET    /admin/games?${y.toString()}
```

## invite-redeems?${i.toString()}  (1)

```
DELETE /admin/invite-redeems?${i.toString()}
```

## invite-redeems?older_than_days=${i}  (1)

```
DELETE /admin/invite-redeems?older_than_days=${i}
```

## me  (1)

```
GET    /admin/me
```

## notifications?${r.toString()}  (1)

```
POST   /admin/notifications?${r.toString()}
```

## notifications?older_than_days=${i}  (1)

```
DELETE /admin/notifications?older_than_days=${i}
```

## offline-records?${i.toString()}  (1)

```
DELETE /admin/offline-records?${i.toString()}
```

## offline-records?older_than_days=${i}  (1)

```
DELETE /admin/offline-records?older_than_days=${i}
```

## offline  (1)

```
PATCH  /admin/offline/config
```

## orders?${i.toString()}  (1)

```
GET    /admin/orders?${i.toString()}
```

## orders?older_than_days=${i}  (1)

```
DELETE /admin/orders?older_than_days=${i}
```

## rental-plans?${i.toString()}  (1)

```
GET    /admin/rental-plans?${i.toString()}
```

## rental-plans  (1)

```
DELETE /admin/rental-plans/${i}
```

## tasks?${i.toString()}  (1)

```
POST   /admin/tasks?${i.toString()}
```

## users?${i.toString()}  (1)

```
DELETE /admin/users?${i.toString()}
```
