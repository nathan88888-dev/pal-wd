
## 物品表（items.json）

### 通用字段
| 字段名 | 类型 | 必填 | 说明 |
|-------|------|------|------|
| `id` | string | 是 | 物品ID，如"item_001" |
| `name` | string | 是 | 物品名称 |
| `type` | string | 是 | 主类型(medicine/dart/adjuvant/material/plot_props) |
| `subtype` | string | 否 | 子类型(heal/spirit/revive等)|
| `price` | integer | 否 | 购买价格 |
| `description` | string | 否 | 物品描述 |

### 药品类
| 字段名 | 类型 | 必填 | 说明 |
|-------|------|------|------|
| `target` | string | 是 | 使用目标(single/all等) |
| `effect` | object | 是 | 效果对象 |
| `battle_usable` | boolean | 是 | 是否战斗可用 |

### 材料类
| 字段名 | 类型 | 必填 | 说明 |
|-------|------|------|------|
| `synthesis` | string[] | 否 | 可合成的装备ID |

### 效果对象(effect)
| 字段名 | 类型 | 说明 |
|-------|------|------|
| `restore_hp` | integer | 恢复HP量 |
| `restore_mp` | integer | 恢复MP量 |
| `remove_status` | string[] | 解除的状态 |
| `revive` | boolean | 是否复活 |
| `element` | string | 符咒属性 |
| `power` | integer | 符咒威力 |

## 五灵属性说明
- 水(`water`)、火(`fire`)、雷(`thunder`)、风(`wind`)、土(`earth`)
- 数值范围：-30到+30（百分比）
- 正数表示加成/抗性，负数表示弱点

## 状态异常列表
| 状态 | 说明 |
|------|------|
| poison | 中毒 |
| seal | 封魔 |
| sleep | 睡眠 |
| confuse | 混乱 |
| weak | 虚弱 |