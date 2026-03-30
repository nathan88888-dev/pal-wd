using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private GameObject uiParent;
    private Dictionary<string, MonoBehaviour> _uiCache = new();
    private string lastUI = "";
    private MonoBehaviour uiLoading;
    protected override void Initialize()
    {
        Debug.Log("UIManager Initialize");
        uiParent = GameObject.Find("UIParent");
    }
    public async UniTask<T> ShowUIAsync<T>() where T:MonoBehaviour
    {
        string address = typeof(T).Name;
        if (address == "UILoading" && uiLoading!=null)
        {
            uiLoading.gameObject.SetActive(true);
            return (T)uiLoading;
        }
        if (address == lastUI) return null;
        if(lastUI!=null && _uiCache.ContainsKey(lastUI))
            _uiCache[lastUI].gameObject.SetActive(false);
        if (address != "UILoading")
        {
            Debug.Log($"switchUIAsync:{lastUI}=>{address}");
            lastUI = address;
        }
        // 如果已经加载并缓存，则直接显示
        if (_uiCache.TryGetValue(address, out var com) && com != null)
        {
            com.gameObject.SetActive(true);
            return (T)com;
        }
        GameObject go = (GameObject)await YAssetLoader.Instance.Load<GameObject>(address);
        com = go.GetComponent<T>();
        RectTransform rtrans = go.GetComponent<RectTransform>();
        rtrans.SetParent(uiParent.transform);
        rtrans.sizeDelta = new Vector2(3840, 2160);
        rtrans.localPosition = Vector3.zero;
        rtrans.localScale = Vector3.one;
        if (address == "UILoading")
        {
            uiLoading = com;
        }
        else {
            _uiCache[address] = com;
        }
        return (T)com;
    }

    public void HideUI<T>()
    {
        string address = typeof(T).Name;
        if (address == "UILoading"&& uiLoading!=null)
        {
            uiLoading.gameObject.SetActive(false);
            return;
        }
        if (_uiCache.TryGetValue(address, out var go) && go != null)
            go.gameObject.SetActive(false);
    }

    public void CloseUI<T>()
    {
        string address = typeof(T).Name;
        if (_uiCache.TryGetValue(address, out var go) && go != null)
        {
            GameObject.DestroyImmediate(go);
            _uiCache.Remove(address);
        }
    }

    public void CleanUI() {
        Dictionary<string, MonoBehaviour>.Enumerator detor = _uiCache.GetEnumerator();
        while (detor.MoveNext())
        {
            GameObject.DestroyImmediate(detor.Current.Value.gameObject);
        }
        _uiCache.Clear();
    }

    public async UniTask RefreshUI<T>()where T: MonoBehaviour
    {
        CloseUI<T>();
        await ShowUIAsync<T>();
    }

}
