
using System;
using UnityEngine;
using UnityEngine.UI;

public class UILoading:MonoBehaviour
{
    public Slider progressUI;
    public Text tipUI;
    public bool isSingleForceLoadSame => false;

    protected void OnEnable()
    {
        progressUI.value = 0;
    }

    public void setTip(string text)
    {
        tipUI.text = text;
    }

    public void SetProgress(float value)
    {
        if (value < 0) {
            gameObject.SetActive(false);
        }
        progressUI.value = value;
    }
}