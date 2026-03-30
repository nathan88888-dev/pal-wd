using Mono.Cecil.Cil;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMsgBox : MonoBehaviour
{
    public Text msgUI;
    public Button okBtn;
    public Button cancelBtn;

    private Action onOKUI;
    private Action onCancelUI;

    private void Awake()
    {
        okBtn.onClick.AddListener(() =>
        {
            if (onOKUI != null) onOKUI();
            gameObject.SetActive(false);
        });
        cancelBtn.onClick.AddListener(() =>
        {
            if (onCancelUI != null) onCancelUI();
            gameObject.SetActive(false);
        });
    }

    public void ShowMsg(string msg, Action onOK, Action onCancel) {
        msgUI.text = msg;
        gameObject.SetActive(true);
        onOKUI = onOK;
        onCancelUI = onCancel;
    }
}
