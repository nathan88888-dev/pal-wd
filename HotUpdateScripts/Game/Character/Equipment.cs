using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    internal class EquipmentStats
    {
        // 基础属性，改为角色对应命名
        public int HealthPoint { get; set; }
        public int MagicPoint { get; set; }
        public int SkillPoint { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public int Luck { get; set; }
        public int Spirit { get; set; }

        // 五行灵属性
        public Dictionary<FiveElements, int> ElementResist { get; set; } = new Dictionary<FiveElements, int>();
        public Dictionary<FiveElements, int> ElementAttack { get; set; } = new Dictionary<FiveElements, int>();

        // 特效
        public List<string> Effects { get; set; } = new List<string>();

        public void Add(EquipmentStats other)
        {
            HealthPoint += other.HealthPoint;
            MagicPoint += other.MagicPoint;
            SkillPoint += other.SkillPoint;
            Attack += other.Attack;
            Defense += other.Defense;
            Speed += other.Speed;
            Luck += other.Luck;

            foreach (var element in Enum.GetValues(typeof(FiveElements)).Cast<FiveElements>())
            {
                ElementResist[element] = ElementResist.GetValueOrDefault(element) + other.ElementResist.GetValueOrDefault(element);
                ElementAttack[element] = ElementAttack.GetValueOrDefault(element) + other.ElementAttack.GetValueOrDefault(element);
            }

            Effects.AddRange(other.Effects);
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("╔═══════════ 装备总成 ═══════════╗");
            sb.AppendLine($"║ 精：{HealthPoint,4}  神：{MagicPoint,4}  气：{SkillPoint,4} ║");
            sb.AppendLine($"║ 武：{Attack,4}  防：{Defense,4}  速：{Speed,4} ║");
            sb.AppendLine($"║ 运：{Luck,4}                ║");
            sb.AppendLine("╠══════════ 五行灵抗 ═══════════╣");

            foreach (FiveElements element in Enum.GetValues(typeof(FiveElements)))
            {
                if (ElementResist.TryGetValue(element, out int resist) && resist != 0)
                {
                    sb.AppendLine($"║ {GetChineseElementName(element)}抗：{(resist > 0 ? "+" : "")}{resist,3}%            ║");
                }
            }

            sb.AppendLine("╠══════════ 五行灵攻 ═══════════╣");
            foreach (FiveElements element in Enum.GetValues(typeof(FiveElements)))
            {
                if (ElementAttack.TryGetValue(element, out int attack) && attack != 0)
                {
                    sb.AppendLine($"║ {GetChineseElementName(element)}攻：+{attack,3}            ║");
                }
            }

            sb.AppendLine("╠══════════ 装备特效 ═══════════╣");
            if (Effects.Count > 0)
            {
                foreach (var effect in Effects)
                {
                    sb.AppendLine($"║ ◎ {effect,-24} ║");
                }
            }
            else
            {
                sb.AppendLine("║        无特效              ║");
            }
            sb.AppendLine("╚═══════════════════════════════╝");

            return sb.ToString();
        }

        private string GetChineseElementName(FiveElements element)
        {
            return element switch
            {
                FiveElements.Water => "水",
                FiveElements.Fire => "火",
                FiveElements.Thunder => "雷",
                FiveElements.Wind => "风",
                FiveElements.Earth => "土",
                _ => string.Empty
            };
        }

    }
    public static class EquipmentCalculator
    {
        internal static EquipmentStats Calculate(Weapon weapon)
        {
            var stats = new EquipmentStats
            {
                Attack = weapon.Attack,
                MagicPoint = weapon.SPIRIT,
                Defense = weapon.DEF,
                Speed = weapon.SPD,
                Luck = weapon.LUK
            };

            if (weapon.Elements.Water != 0) stats.ElementAttack[FiveElements.Water] = weapon.Elements.Water;
            if (weapon.Elements.Thunder != 0) stats.ElementAttack[FiveElements.Thunder] = weapon.Elements.Thunder;

            stats.Effects.AddRange(weapon.Effects);
            return stats;
        }

        internal static EquipmentStats Calculate(Armor armor)
        {
            var stats = new EquipmentStats
            {
                Attack = armor.ATK,
                MagicPoint = armor.SPIRIT,
                Defense = armor.DEF,
                Speed = armor.SPD,
                Luck = armor.LUK
            };

            if (armor.Elements.Water != 0) stats.ElementResist[FiveElements.Water] = armor.Elements.Water;
            if (armor.Elements.Fire != 0) stats.ElementResist[FiveElements.Fire] = armor.Elements.Fire;
            if (armor.Elements.Wind != 0) stats.ElementResist[FiveElements.Wind] = armor.Elements.Wind;
            if (armor.Elements.Thunder != 0) stats.ElementResist[FiveElements.Thunder] = armor.Elements.Thunder;
            if (armor.Elements.Earth != 0) stats.ElementResist[FiveElements.Earth] = armor.Elements.Earth;

            stats.Effects.AddRange(armor.Effects);
            return stats;
        }

        internal static EquipmentStats CalculateTotal(
            Weapon weapon,
            Armor[] armors,
            Accessory[] accessories)
        {
            var total = new EquipmentStats();

            if (weapon != null) total.Add(Calculate(weapon));

            if (armors != null)
            {
                if (armors.Length != 3) Debug.LogWarning($"Armor count mismatch. Expected:3, Actual:{armors.Length}");
                foreach (var armor in armors)
                {
                    if (armor != null) total.Add(Calculate(armor));
                }
            }

            if (accessories != null)
            {
                if (accessories.Length != 2) Debug.LogWarning($"Accessory count mismatch. Expected:2, Actual:{accessories.Length}");
                foreach (var acc in accessories)
                {
                    if (acc != null)
                    {
                        total.Add(new EquipmentStats
                        {
                            Attack = acc.ATK,
                            MagicPoint = acc.SPIRIT,
                            Defense = acc.DEF,
                            Speed = acc.SPD,
                            Luck = acc.LUK,
                            ElementResist = new Dictionary<FiveElements, int>
                            {
                                [FiveElements.Water] = acc.Elements.Water,
                                [FiveElements.Wind] = acc.Elements.Wind
                            },
                            Effects = acc.Effects.ToList()
                        });
                    }
                }
            }

            return total;
        }
    }



    [System.Serializable]
    public class Accessory
    {
        public string ID;
        public string Name;
        public List<string> UsableCharacters;
        public int ATK;
        public int SPIRIT;
        public int DEF;
        public int SPD;
        public int LUK;
        public ElementData Elements;
        public List<string> Effects;
        public int Price;
        public string Description;

        [System.Serializable]
        public class ElementData
        {
            public int Water;
            public int Wind;
        }

        public Accessory()
        {
            UsableCharacters = new List<string>();
            Elements = new ElementData();
            Effects = new List<string>();
        }
    }

    [System.Serializable]
    public class Armor
    {
        public enum ArmorType { Head, Body, Feet }

        public string ID;
        public string Name;
        public ArmorType Type;
        public List<string> UsableCharacters;
        public int ATK;
        public int SPIRIT;
        public int DEF;
        public int SPD;
        public int LUK;
        public ElementData Elements;
        public List<string> Effects;
        public int Price;
        public string Description;

        [System.Serializable]
        public class ElementData
        {
            public int Water;
            public int Fire;
            public int Wind;
            public int Thunder;
            public int Earth;
        }

        public Armor()
        {
            UsableCharacters = new List<string>();
            Elements = new ElementData();
            Effects = new List<string>();
        }
    }

    [System.Serializable]
    public class Weapon
    {
        public string ID;
        public string Name;
        public List<string> UsableCharacters;
        public int Attack;
        public int SPIRIT;
        public int DEF;
        public int SPD;
        public int LUK;
        public ElementData Elements;
        public List<string> Effects;
        public int Price;
        public string Description;

        [System.Serializable]
        public class ElementData
        {
            public int Water;
            public int Thunder;
        }

        public Weapon()
        {
            UsableCharacters = new List<string>();
            Elements = new ElementData();
            Effects = new List<string>();
        }
    }