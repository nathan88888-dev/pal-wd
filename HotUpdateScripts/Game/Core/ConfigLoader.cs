

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

    public static class ConfigLoader
    {
        //private static ResLoader _configLoader;
        public static Dictionary<string, Character_att> Character_atts;

    public static async UniTask Init() {
        //_configLoader = ResLoader.Create();
        Character_atts = new Dictionary<string, Character_att>();

        Character_atts.Add("NianWan", new Character_att(
            (TextAsset)await YAssetLoader.Instance.Load<TextAsset>("Character_att_NianWan")));
        Character_atts.Add("103", new Character_att(
            (TextAsset)await YAssetLoader.Instance.Load<TextAsset>("Character_att_103")));
        Character_atts.Add("501", new Character_att(
            (TextAsset)await YAssetLoader.Instance.Load<TextAsset>("Character_att_501")));
        }
    }

