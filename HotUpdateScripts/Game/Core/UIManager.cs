using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private GameObject uiParent;
    private Dictionary<string, MonoBehaviour> _uiCache = new();
    private string lastUI = "";
    private UIMsgBox uiMsgBox;
    private UILoading uiLoading;
    protected override async UniTask Initialize()
    {
        uiParent = GameObject.Find("UIParent");

        uiMsgBox = await LoadUI<UIMsgBox>();
        uiMsgBox.gameObject.SetActive(false);
        uiLoading = await LoadUI<UILoading>();
        uiLoading.gameObject.SetActive(false);
    }

    public void ShowMsg(string msg, Action onok, Action oncancel) 
    {
        uiMsgBox.ShowMsg(msg, onok, oncancel);
    }

    public async UniTask<UILoading> GetLoadingUI()
    {
        await UniTask.WaitUntil(()=>uiLoading != null);
        uiLoading.gameObject.SetActive(true);
        return uiLoading;
    }

    private async UniTask<T> LoadUI<T>() where T : MonoBehaviour
    {
        string address = typeof(T).Name;
        GameObject go = (GameObject)await YAssetLoader.Instance.Load<GameObject>(address);
        T com = go.GetComponent<T>();
        RectTransform rtrans = go.GetComponent<RectTransform>();
        rtrans.SetParent(uiParent.transform);
        rtrans.SetAsFirstSibling();
        rtrans.sizeDelta = new Vector2(0, 0);
        rtrans.localPosition = Vector3.zero;
        rtrans.localScale = Vector3.one;
        return com;
    }

    public async UniTask ShowUIAsync<T>() where T:MonoBehaviour
    {
        string address = typeof(T).Name;
        if (address == lastUI) return;
        if(lastUI!=null && _uiCache.ContainsKey(lastUI))
            _uiCache[lastUI].gameObject.SetActive(false);
        Debug.Log($"switchUIAsync:{lastUI}=>{address}");
        lastUI = address;
        // 如果已经加载并缓存，则直接显示
        if (_uiCache.TryGetValue(address, out var com) && com != null)
        {
            com.gameObject.SetActive(true);
            return;
        }
        _uiCache[address] = await LoadUI<T>();
    }

    public void HideUI<T>()
    {
        string address = typeof(T).Name;
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
