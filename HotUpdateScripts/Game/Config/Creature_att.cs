using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

    [Serializable]
    public class Creature_att
{
    [System.Serializable]
    public class AttributeGrowth
    {
        [JsonProperty("Base")]
        public float Base;

        [JsonProperty("level2Growth")]
        public float LevelSquaredGrowth;

        [JsonProperty("levelGrowth")]
        public float LevelLinearGrowth;

        public float Calculate(int level)
        {
            return Base + (LevelSquaredGrowth * level * level) + (LevelLinearGrowth * level);
        }
    }

    [System.Serializable]
    public class SkillLearningEntry
    {
        [JsonProperty("Level")]
        public int UnlockLevel;

        [JsonProperty("Skills")]
        public List<int> SkillIDs;
    }

    public string CharacterName;
    public Dictionary<string, AttributeGrowth> Attributes { get; set; }
    public string Formula { get; set; }
    public List<SkillLearningEntry> SkillLearning { get; set; }

    public Creature_att(TextAsset jsonText) {
        JObject root = JObject.Parse(jsonText.text);

        CharacterName = root["Character"]?.ToString();

        Attributes = new Dictionary<string, AttributeGrowth>();
        var attributes = (JObject)root["Attributes"];
        foreach (var property in attributes.Properties())
        {
            Attributes.Add(property.Name, property.Value.ToObject<AttributeGrowth>());
        }

        SkillLearning = root["SkillLearning"].ToObject<List<SkillLearningEntry>>();
        Debug.Log($"角色名:{CharacterName},属性数量:{Attributes.Count},技能数量:{SkillLearning.Count}");
    }

    public int CalculateAttribute(string name, int level)
        {
            return (int)Attributes[name].Calculate(level);
        }

        public List<int> GetSkillsLearnedAtLevel(int targetLevel)
        {
            var skills = new List<int>();
            foreach (var entry in SkillLearning)
            {
                if (entry.UnlockLevel <= targetLevel)
                {
                    skills.AddRange(entry.SkillIDs);
                }
            }
            return skills;
        }
    }