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
    //
    // is called once before the first execution of Update after the MonoBehaviour is created
    async UniTask Awake()
    {
        if (await CheckWebsite())
        {
            DontDestroyOnLoad(gameObject);
            ConfigLoader.Init().Forget();
            SwitchScene<UIStart>().Forget();
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

    public static async UniTask SwitchScene<T>(string name = null)where T:MonoBehaviour
    {
        if (name != null) {
            UILoading loading = await UIManager.Instance.ShowUIAsync<UILoading>();
            UIManager.Instance.CleanUI();
            await YAssetLoader.Instance.LoadScene(PathConstant.PackScenePath+ name,
                loading.Progress);
        }
        await UIManager.Instance.ShowUIAsync<T>();
        UIManager.Instance.HideUI<UILoading>();
    }

    async UniTask<bool> CheckWebsite()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log(json);
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
