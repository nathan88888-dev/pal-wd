
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GMemoryCache : Singleton<GMemoryCache>
{

    // 游戏数据存储
    private Dictionary<string, Character[]> _characterCache;
    private Dictionary<string, Item[]> _itemCache;
    private GameState _currentGameState;
    //private Resouce _characterLoader;
    //private ResLoader _fileLoader;
    public MainCam mainCam;

    // 线程安全锁
    private readonly object _lock = new object();

    public GMemoryCache()
    {
        _characterCache = new Dictionary<string, Character[]>();
        _itemCache = new Dictionary<string, Item[]>();
        _currentGameState = new GameState();

        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            Debug.Log("onSceneLoaded:"+scene.name);
            _currentGameState.CurrentScene = scene;
        };
    }


    public async UniTask<GameObject> LoadCharacter(Transform parent, string name, LayerEnum layer) {
        GameObject go = (GameObject)await YAssetLoader.Instance.Load<GameObject>(
            (layer == LayerEnum.Character ? "characters_" : "enemy_") + name);
        go.SetActive(false);
        go.transform.SetParent(parent);
        go.layer = (int)layer;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.name = name+go.GetInstanceID();
        go.SetActive(true);
        return go;
    }
    public async UniTask<PatrolWithList[]> LoadEnemy(Transform parent)
    {
        PatrolWithList[] res = null;
        List<EnemyPathEntry> entries;
        if (_currentGameState.enemyData == null)
        {
            string jsonAsset = ((TextAsset)await YAssetLoader.Instance.Load<TextAsset>(
                _currentGameState.CurrentScene.name + "_sceneEnemy")).text;

            JObject root = JObject.Parse(jsonAsset);
            JArray arr = (JArray)root["entries"];
            entries = new List<EnemyPathEntry>();
            for (int i = 0;i < arr.Count;i++)
            {
                EnemyPathEntry entry = arr[i].ToObject<EnemyPathEntry>();
                entry.enemies = arr[i]["enemies"].ToObject<List<EnemyDefinition>>();
                entries.Add(entry);
            }
            _currentGameState.enemyData = entries;
        }
        else
        {
            entries = _currentGameState.enemyData;
        }
        if (entries == null)
        {
            Debug.Log($"can't find enemy file, on{_currentGameState.CurrentScene.name + "_sceneEnemy"}");
            return res;
        }

        res = new PatrolWithList[entries.Count];
        for (int i = 0; i < entries.Count; i++)
        {
            EnemyPathEntry entry = entries[i];
            GameObject go = (GameObject)await YAssetLoader.Instance.Load<GameObject>(
                "enemy_" + entry.monsterPrefab);
            go.SetActive(false);
            go.layer = (int)LayerEnum.Enemy;
            PatrolWithList spc = go.AddComponent<PatrolWithList>();
            spc.pathPoints = entry.pathMove.ToArray();
            go.transform.SetParent(parent);
            go.transform.position = spc.pathPoints[0];
            spc.speed = entry.moveSpeed;
            spc.enemies = entry.enemies.ToArray();
            go.name = entry.monsterPrefab + go.GetInstanceID();
            go.SetActive(true);
            res[i] = spc;
        }
        return res;
    }

        public bool CopyChild(Transform parent)
        {
            if(parent == null || parent.childCount == 0) return false;
            Transform newobj = UnityEngine.Object.Instantiate(parent.GetChild(0), parent);
            return true;
        }

        #region 角色数据管理
        public void StoreCharacters(string key, Character[] characters)
        {
            lock (_lock)
            {
                if (_characterCache.ContainsKey(key))
                {
                    _characterCache[key] = (Character[])_characterCache[key].Concat(characters);
                }
                else
                {
                    _characterCache.Add(key, characters);
                }
            }
        }

        public Character[] GetCharacters(string key)
        {
            lock (_lock)
            {
                if(_characterCache.ContainsKey(key))
                {
                    return _characterCache[key];
                }
                return new Character[_characterCache[key].Length];
            }
        }
        #endregion

        #region 物品数据管理
        public void StoreItems(string key, Item[] items)
        {
            lock (_lock)
            {
                if (_itemCache.ContainsKey(key))
                {
                    _itemCache[key] = items;
                }
                else
                {
                    _itemCache.Add(key, items);
                }
            }
        }

        public Item[] GetItems(string key)
        {
            lock (_lock)
            {
                if (_itemCache.ContainsKey(key))
                {
                    return _itemCache[key];
                }
                return new Item[0];
            }
        }

        public void AddItem(string key, Item newItem)
        {
            lock (_lock)
            {
                if (_itemCache.ContainsKey(key))
                {
                    _itemCache[key] = (Item[])_itemCache[key].Concat(new Item[] { newItem });
                }
                else
                {
                    _itemCache.Add(key, new Item[] { newItem });
                }
            }
        }

        public bool RemoveItem(string key, string itemId)
        {
            lock (_lock)
            {
                if (_itemCache.ContainsKey(key))
                {
                    List<Item> items =  new List<Item>(_itemCache[key]);
                    int index = items.FindIndex(i => i.Id == itemId);
                    if (index >= 0)
                    {
                        items.RemoveAt(index);
                        _itemCache[key] = items.ToArray();
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion

        #region 游戏状态管理
        public GameState GetGameState()
        {
            lock (_lock)
            {
                return _currentGameState;
            }
        }

        public void UpdateGameState(Action<GameState> updateAction)
        {
            lock (_lock)
            {
                updateAction?.Invoke(_currentGameState);
            }
        }

        public void SetGameState(GameState newState)
        {
            lock (_lock)
            {
                _currentGameState = (GameState)newState.Clone();
            }
        }
        #endregion

        #region 辅助方法
        public void ClearAllData()
        {
            lock (_lock)
            {
                _characterCache.Clear();
                _itemCache.Clear();
                _currentGameState = new GameState();
            }
        }

        public void ClearCache(string key)
        {
            lock (_lock)
            {
                _characterCache.Remove(key);
                _itemCache.Remove(key);
            }
        }
        #endregion
    }

    // 游戏状态类
public class GameState : ICloneable
{
    public Scene CurrentScene { get; set; }
    public DateTime GameTime { get; set; } = DateTime.Now;

    public Character[] chaList;
    public int MainCharacterIndex { get; set; } = 0;
    public Vector3 position { get; set; }
    public List<EnemyPathEntry> enemyData { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

}

// 物品基类
public class Item
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Count { get; set; } = 1;
    public bool CanStack { get; set; }
    public int MaxStack { get; set; } = 99;
}
