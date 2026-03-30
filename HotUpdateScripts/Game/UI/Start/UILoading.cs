
using System;
using UnityEngine;
using UnityEngine.UI;

public class UILoading:MonoBehaviour
{
    public Image progressUI;
    public bool isSingleForceLoadSame => false;

    protected void OnEnable()
    {
        progressUI.fillAmount = 0;
    }

    public void Progress(float value)
    {
        progressUI.fillAmount = value;
    }
}