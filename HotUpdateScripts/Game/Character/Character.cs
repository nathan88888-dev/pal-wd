
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public enum FiveElements { Water, Fire, Thunder, Wind, Earth }

    public class Character
    {
        // Basic Info
        public string Name { get; private set; }
        public int Level { get; private set; } = 1;
        public int Experience { get; private set; }
        public int FavorLevel { get; private set; }
        public FiveElements PrimaryElement { get; private set; }

        // Vital Stats
        public int MaxHP { get; private set; }
        public int CurrentHP { get; private set; }
        public int MaxMP { get; private set; }
        public int CurrentMP { get; private set; }
        public int MaxSP { get; private set; }
        public int CurrentSP { get; private set; }

        // Core Attributes
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int Speed { get; private set; }
        public int Luck { get; private set; }
        public int Spirit { get; private set; }
        public int SkillPoint { get; private set; }
        public int HealthPoint { get; private set; }
        public int MagicPoint { get; private set; }

        public UInt16 water;
        public UInt16 fire;
        public UInt16 thund;
        public UInt16 earth;
        public UInt16 wind;

        public int exNow;
        public int exNeed;

        public int Favor { get; private set; }
        //battle attribute
        public float currentMove;

        // Equipment
        public Weapon EquippedWeapon { get; private set; }
        public Accessory[] EquippedAccessories { get; private set; } = new Accessory[2];
        public Armor[] EquippedArmors { get; private set; } = new Armor[3];

        // Abilities and Status
        public List<Magic> Skills { get; private set; } = new List<Magic>();
        public List<string> StatusEffects { get; private set; } = new List<string>();

        // UI References
        public Slider speedSlider { get; set; }
        public UIStatus StatusUI { get; private set; }
        public GameObject CharacterObject { get; set; }
        public Animator animator { get; set; }

        private Character_att _config;

        public Character(string name, int startingLevel = 1)
        {
            if (!ConfigLoader.Character_atts.ContainsKey(name)) { 
                Debug.LogError($"角色:{name},不存在");
                return; 
            }
            Name = name;
            _config = ConfigLoader.Character_atts[name];
            Level = startingLevel;

            CalculateBaseAttributes();
        }


        private void CalculateBaseAttributes()
        {
            EquipmentStats eqpStats = EquipmentCalculator.CalculateTotal(EquippedWeapon, EquippedArmors, EquippedAccessories);

            Attack = _config.CalculateAttribute("Attack", Level)+ eqpStats.Attack;
            Defense = _config.CalculateAttribute("Defense", Level)+ eqpStats.Defense;
            Speed = _config.CalculateAttribute("Speed", Level)+ eqpStats.Speed;
            Luck = _config.CalculateAttribute("Luck", Level)+ eqpStats.Luck;
            Spirit = _config.CalculateAttribute("Spirit", Level)+ eqpStats.Spirit;
            SkillPoint = _config.CalculateAttribute("SkillPoint", Level)+ eqpStats.SkillPoint;
            HealthPoint = _config.CalculateAttribute("HealthPoint", Level)+ eqpStats.HealthPoint;
            MagicPoint = _config.CalculateAttribute("MagicPoint", Level)+ eqpStats.MagicPoint;

            UpdateBaseAttributes();
        }

        private void UpdateBaseAttributes() {

            MaxHP = HealthPoint;
            MaxMP = MagicPoint;
            MaxSP = SkillPoint;
            CurrentHP = MaxHP;
            CurrentMP = MaxMP;
            CurrentSP = MaxSP;
        }

        public void LevelUp()
        {
            Level++;
            CalculateBaseAttributes();
        }

        public void EquipWeapon(Weapon weapon)
        {
            EquippedWeapon = weapon;
            RecalculateStats();
        }

        public void EquipArmor(Armor armor, int slot)
        {
            if (slot >= 0 && slot < 3)
            {
                EquippedArmors[slot] = armor;
                RecalculateStats();
            }
        }

        public void EquipAccessory(Accessory accessory, int slot)
        {
            if (slot >= 0 && slot < 2)
            {
                EquippedAccessories[slot] = accessory;
                RecalculateStats();
            }
        }

        private void RecalculateStats()
        {
            // Combine base stats with equipment bonuses
            var equipmentStats = EquipmentCalculator.CalculateTotal(
                EquippedWeapon,
                EquippedArmors,
                EquippedAccessories
            );

            // Apply equipment bonuses to character
            // (Implementation depends on how you want to combine base and equipment stats)
        }

        internal void ChangeFavor(int v)
        {
            Favor += v;
        }

        internal void CleanUI()
        {
            speedSlider = null;
            StatusUI = null;
            CharacterObject = null;
        }

        internal async UniTask Dead()
        {
            if (animator != null)
            {
                animator.CrossFade("die", 0.1f);
                await UniTask.Delay(2000);
            }
            speedSlider.gameObject.SetActive(false);
            CharacterObject.gameObject.SetActive(false);
            if(StatusUI!=null)
                StatusUI.gameObject.SetActive(false);
            CleanUI();
        }

        internal bool UpdateCurrentMove()
        {
            currentMove += Speed * 3;
            speedSlider.value = currentMove / 10000f;
            if (currentMove > 10000)
            {
                currentMove -= 10000;
                return true;
            }
            return false;
        }

        internal void UpdateStatusUI(UIStatus status = null) {
            if(status!=null)
                StatusUI = status;
            if (StatusUI != null)
            {
                StatusUI.hp.value = (float)CurrentHP / HealthPoint;
                StatusUI.hpt.text = $"{CurrentHP}/{HealthPoint}";
                StatusUI.sp.value = (float)CurrentSP / SkillPoint;
                StatusUI.spt.text = $"{CurrentSP}/{SkillPoint}";
                StatusUI.mp.value = (float)CurrentMP / MagicPoint;
                StatusUI.mpt.text = $"{CurrentMP}/{MagicPoint}";
            }
        }


        internal async UniTask<bool> AttackMove(Character target)
        {
            Vector3 position = CharacterObject.transform.position;
            if (animator != null)
                animator.CrossFade("attack", 0.1f);
            CharacterObject.transform.DOMove(
                target.CharacterObject.transform.GetChild(0).position, 0.5f).OnComplete(() =>
                {
                    target.CurrentHP += -Attack;
                    target.UpdateStatusUI();
                    Debug.Log($"{target.Name}受到:{Attack},伤害,剩余:{target.CurrentHP}");
                    CharacterObject.transform.DOMove(position, 0.5f).SetDelay(1f);
                });
            await UniTask.Delay(1000);
            if (target.CurrentHP < 0)
            {
                target.Dead().Forget();
            }
            await UniTask.Delay(1000);
            return target.CurrentHP < 0;
        }

    }