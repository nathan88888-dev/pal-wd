using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class DataOperation:Singleton<DataOperation>
{
    private readonly string SaveFolderPath = Path.Combine(Application.persistentDataPath, "Saves");
    private readonly string EncryptionKey = "YourStrongKey1234"; // 16字符密钥，必须保证长度16
    private readonly int MaxSaveCount = 30;

    private readonly object _lock = new object();

    public GameState[] gameStates;

    protected override async UniTask Initialize() {

        if (!Directory.Exists(SaveFolderPath))
        {
            Directory.CreateDirectory(SaveFolderPath);
            Debug.Log("文件夹不存在，已创建: " + SaveFolderPath);
        }
        else
        {
            Debug.Log("文件夹已存在: " + SaveFolderPath);
        }
        RefreshDatas();
    }

    public void RefreshDatas()
    {
        gameStates = LoadAllSaves();
    }

    /// <summary>
    /// 保存到指定槽位，编号1~50
    /// </summary>
    public void SaveGame(int slot, GameState _currentGameState)
    {
        if (slot < 1 || slot > MaxSaveCount)
        {
            Debug.LogError($"存档编号错误，应在1到{MaxSaveCount}之间");
            return;
        }

        lock (_lock)
        {
            if (!Directory.Exists(SaveFolderPath))
                Directory.CreateDirectory(SaveFolderPath);


            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new Vector3Converter());
            string json = JsonConvert.SerializeObject(_currentGameState, Formatting.Indented, settings);
            Debug.Log($"saved json:{json}");
            byte[] encrypted = Encrypt(json, EncryptionKey);

            string saveFilePath = GetSaveFilePath(slot);
            File.WriteAllBytes(saveFilePath, encrypted);

            Debug.Log($"游戏已存档槽 {slot}: {saveFilePath}");
            RefreshDatas();
        }
    }

    /// <summary>
    /// 读取指定槽位的存档
    /// </summary>
    public GameState LoadGame(int slot)
    {
        if (slot < 1 || slot > MaxSaveCount)
        {
            Debug.LogError($"存档编号错误，应在1到{MaxSaveCount}之间");
            return null;
        }

        string saveFilePath = GetSaveFilePath(slot);

        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning($"存档槽 {slot} 不存在");
            return null;
        }

        return LoadGameFromFile(saveFilePath);
    }


    public void showGameDate(Button[] togs, UIGameDate[] gameDataUI, 
        Action<UIGameDate, GameState> onClickObj)
    {
        if (gameStates.Length != 30 || togs.Length != 6 || gameDataUI.Length != 5)
        {
            Debug.LogError($"showGameDate is wrong, {gameStates.Length}, {togs.Length}, {gameDataUI.Length}");
        }
        for (int i = 0; i < togs.Length; i++)
        {
            Button tog = togs[i];
            int index = i * 5;
            tog.onClick.AddListener(() =>
            {
                for (int gsi = 0; gsi < 5; gsi++)
                {
                    GameState thisState = gameStates[index + gsi];
                    UIGameDate thisData = gameDataUI[gsi];
                    thisData.name = (index + gsi+1).ToString();
                    thisData.dataID.text = thisData.name;
                    if (thisState == null)
                    {
                        thisData.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    }
                    else
                    {
                        thisData.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        thisData.gameTime.text = thisState.allGameTime;
                        thisData.screenShoot.texture = thisState.GetScreenshotTexture();
                    }
                    thisData.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        onClickObj(thisData, thisState);
                    });
                }
            });
        }
    }
    /// <summary>
    /// 一次性读取所有存在的存档槽，返回列表 (槽号, GameState)
    /// </summary>
    public GameState[] LoadAllSaves()
    {
        var results = new GameState[MaxSaveCount];

        if (!Directory.Exists(SaveFolderPath))
        {
            Debug.LogWarning("存档文件夹不存在");
            return results;
        }

        for (int slot = 1; slot <= MaxSaveCount; slot++)
        {
            string path = GetSaveFilePath(slot);
            if (File.Exists(path))
            {
                try
                {
                    byte[] encrypted = File.ReadAllBytes(path);
                    string json = Decrypt(encrypted, EncryptionKey);
                    var loadedState = JsonConvert.DeserializeObject<GameState>(json);
                    if (loadedState != null)
                    {
                        loadedState.StartNewSession();
                        results[slot] = loadedState;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"读取存档槽 {slot} 失败: {ex}");
                }
            }
        }

        return results;
    }

    private string GetSaveFilePath(int slot)
    {
        return Path.Combine(SaveFolderPath, $"save_{slot}.dat");
    }

    private GameState LoadGameFromFile(string filePath)
    {
        try
        {
            byte[] encrypted = File.ReadAllBytes(filePath);
            string json = Decrypt(encrypted, EncryptionKey);
            var loadedState = JsonConvert.DeserializeObject<GameState>(json);

            if (loadedState != null)
            {
                loadedState.StartNewSession();
                GameState _currentGameState = loadedState;
                Debug.Log($"存档加载成功: {filePath}");
                return _currentGameState;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"读取存档失败: {ex}");
        }

        return null;
    }

    private byte[] Encrypt(string plainText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            byte[] keyBytes = GetAesKey(key);
            aes.KeySize = 256;
            aes.Key = keyBytes;
            aes.IV = new byte[16]; // 固定IV，推荐改进为随机IV

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
        }
    }

    private string Decrypt(byte[] cipherText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            byte[] keyBytes = GetAesKey(key);
            aes.KeySize = 256;
            aes.Key = keyBytes;
            aes.IV = new byte[16];

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] bytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            return Encoding.UTF8.GetString(bytes);
        }
    }
    private static byte[] GetAesKey(string key)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
    }
}