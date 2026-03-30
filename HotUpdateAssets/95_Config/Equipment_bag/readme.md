### 武器表（weapons）
| 字段名 | 类型 | 必填 | 说明 |
|-------|------|------|------|
| `id` | string | 是 | 装备唯一ID，如"w001" |
| `name` | string | 是 | 武器名称 |
| `user` | string[] | 是 | 可装备角色列表 |
| `attack` | integer | 是 | 攻击力加成 |
| `spirit` | integer | 否 | 灵(仙术攻击力)加成 |
| `defense` | integer | 否 | 防御力加成 |
| `speed` | integer | 否 | 速度加成(可为负) |
| `luck` | integer | 否 | 运气加成(可为负) |
| `elements` | object | 否 | 五灵属性加成 |
| `effects` | string[] | 否 | 特殊效果描述 |
| `price` | integer | 否 | 购买价格 |
| `description` | string | 是 | 装备描述 |

### 头部防具表（armors_head）
| 字段名 | 类型 | 必填 | 说明 |
|-------|------|------|------|
| `id` | string | 是 | 装备唯一ID，如"ah001" |
| `name` | string | 是 | 头部防具名称 |
| `user` | string[] | 是 | 可装备角色列表 |
| `attack` | integer | 否 | 攻击力加成 |
| `spirit` | integer | 否 | 灵(仙术攻击力)加成 |
| `defense` | integer | 是 | 防御力加成 |
| `speed` | integer | 否 | 速度加成(可为负) |
| `luck` | integer | 否 | 运气加成(可为负) |
| `elements` | object | 否 | 五灵属性加成 |
| `effects` | string[] | 否 | 特殊效果描述 |
| `price` | integer | 否 | 购买价格 |
| `description` | string | 是 | 装备描述 |

### 身体防具表（armors_body）
| 字段名 | 类型 | 必填 | 说明 |
|-------|------|------|------|
| `id` | string | 是 | 装备唯一ID，如"ab001" |
| `name` | string | 是 | 身体防具名称 |
| `user` | string[] | 是 | 可装备角色列表 |
| `attack` | integer | 否 | 攻击力加成 |
| `spirit` | integer | 否 | 灵(仙术攻击力)加成 |
| `defense` | integer | 是 | 防御力加成 |
| `speed` | integer | 否 | 速度加成(可为负) |
| `luck` | integer | 否 | 运气加成(可为负) |
| `elements` | object | 否 | 五灵属性加成 |
| `effects` | string[] | 否 | 特殊效果描述 |
| `price` | integer | 否 | 购买价格 |
| `description` | string | 是 | 装备描述 |

### 足部防具表（armors_foot）
| 字段名 | 类型 | 必填 | 说明 |
|-------|------|------|------|
| `id` | string | 是 | 装备唯一ID，如"af001" |
| `name` | string | 是 | 足部防具名称 |
| `user` | string[] | 是 | 可装备角色列表 |
| `attack` | integer | 否 | 攻击力加成 |
| `spirit` | integer | 否 | 灵(仙术攻击力)加成 |
| `defense` | integer | 是 | 防御力加成 |
| `speed` | integer | 否 | 速度加成(可为负) |
| `luck` | integer | 否 | 运气加成(可为负) |
| `elements` | object | 否 | 五灵属性加成 |
| `effects` | string[] | 否 | 特殊效果描述 |
| `price` | integer | 否 | 购买价格 |
| `description` | string | 是 | 装备描述 |

### 饰品表（accessories）
| 字段名 | 类型 | 必填 | 说明 |
|-------|------|------|------|
| `id` | string | 是 | 装备唯一ID，如"acc001" |
| `name` | string | 是 | 饰品名称 |
| `user` | string[] | 是 | 可装备角色列表 |
| `attack` | integer | 否 | 攻击力加成 |
| `spirit` | integer | 否 | 灵(仙术攻击力)加成 |
| `defense` | integer | 否 | 防御力加成 |
| `speed` | integer | 否 | 速度加成(可为负) |
| `luck` | integer | 否 | 运气加成(可为负) |
| `elements` | object | 否 | 五灵属性加成 |
| `effects` | string[] | 否 | 特殊效果描述 |
| `price` | integer | 否 | 购买价格 |
| `description` | string | 是 | 装备描述 |