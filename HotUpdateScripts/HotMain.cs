using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HotMain : MonoBehaviour
{
    private class ServerStatus
    {
        public string status;
        public string version;
        public string info;
    }
    private string url = "https://nathan88888-dev.github.io/Health/Check/wd3D";
    private ServerStatus sStatus;
    private GameState gameState;
    //
    // is called once before the first execution of Update after the MonoBehaviour is created
    async UniTask Awake()
    {
        gameState = GMemoryCache.Instance.GetGameState();
        if (await CheckWebsite())
        {
            DontDestroyOnLoad(gameObject);
            await UniTask.WaitUntil(() => ConfigLoader.Instance != null);
            await UniTask.WaitUntil(() => UIManager.Instance != null);
            await SwitchScene<UIStart>();
        }
    }

    void OnGUI()
    {
        if (sStatus!=null && sStatus.status != "ok")
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 100;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(300, 100, 3000, 2000), sStatus.info, style);
        }
    }

    private void Update()
    {
        gameState.CommitSessionTime();
    }

    public static async UniTask SwitchScene<T>(string name = null)where T:MonoBehaviour
    {
        UILoading loading = await UIManager.Instance.GetLoadingUI();
        if (name != null) {
            UIManager.Instance.CleanUI();
            await YAssetLoader.Instance.LoadScene(PathConstant.PackScenePath+ name,
                loading);
        }
        await UIManager.Instance.ShowUIAsync<T>();
        loading.SetProgress(-1);
    }

    async UniTask<bool> CheckWebsite()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            try
            {
                sStatus = JsonUtility.FromJson<ServerStatus>(json);

                if (sStatus.status == "ok" && sStatus.version == "1.0.3")
                {
                    Debug.Log("value correct ✅");
                    return true;
                }
                else
                {
                    Debug.LogWarning($"value wrong ❌{sStatus.status}+{sStatus.version}");
                    return false;
                }
            }
            catch
            {
                Debug.LogError("JSON failed ❗");
                return false;
            }
        }
        else
        {
            Debug.LogError("can't check url: " + request.error);
            return false;
        }
    }
}
