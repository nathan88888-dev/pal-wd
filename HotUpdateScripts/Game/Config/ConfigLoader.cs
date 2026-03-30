

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class ConfigLoader:Singleton<ConfigLoader>
{
    //private static ResLoader _configLoader;
    private Dictionary<string, Creature_att> Character_atts;
    private Dictionary<string, Creature_att> Enemy_atts;

    protected override async UniTask Initialize() {
        //_configLoader = ResLoader.Create();
        Character_atts = new Dictionary<string, Creature_att>();
        Enemy_atts = new Dictionary<string, Creature_att>();

        Character_atts.Add("NianWan", new Creature_att(
            (TextAsset)await YAssetLoader.Instance.Load<TextAsset>("Character_att_NianWan")));
        Enemy_atts.Add("103", new Creature_att(
            (TextAsset)await YAssetLoader.Instance.Load<TextAsset>("Enemy_att_103")));
        Enemy_atts.Add("501", new Creature_att(
            (TextAsset)await YAssetLoader.Instance.Load<TextAsset>("Enemy_att_501")));
        Enemy_atts.Add("Rabby_B", new Creature_att(
            (TextAsset)await YAssetLoader.Instance.Load<TextAsset>("Enemy_att_Rabby_B")));
    }

    public Creature_att getCreature(string name, LayerEnum layer) {

        if (layer == LayerEnum.Character ?
        !Character_atts.ContainsKey(name) :
        !Enemy_atts.ContainsKey(name))
        {
            Debug.LogError($"角色:{layer}-{name},不存在");
            return null;
        }
        return layer == LayerEnum.Character ?Character_atts[name] : Enemy_atts[name];
    }
}

