using Newtonsoft.Json;
using System;
using System.Collections.Generic;

    [Serializable]
    public class Magic
    {
        public string Name { get; set; }
        public string Type { get; set; }    // attack, support 等
        public int Power { get; set; }
        public int Cost { get; set; }
        public string Effect { get; set; }
    }
    [Serializable]
    public class MagicConfig
    {
        public string Attribute { get; set; }
        public List<Magic> Skills { get; set; }

        // 从 JSON 反序列化
        public static Magic FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Magic>(json);
        }
    }
