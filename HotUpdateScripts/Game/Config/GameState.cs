using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable] // 让它可序列化
public class GameState : ICloneable
{
    public Scene CurrentScene { get; set; }

    // 历史累计时间（秒）
    private double hisGameTimeSeconds;
    private double curGameTimeSeconds;

    // 本次启动的起始时间
    [NonSerialized]
    private DateTime sessionStartTime;

    public Character[] chaList;
    public int MainCharacterIndex { get; set; } = 0;
    public Vector3 position { get; set; }
    public List<EnemyPathEntry> enemyData { get; set; }
    //截图
    private string screenshotBase64;

    public GameState()
    {
        hisGameTimeSeconds = 0;
    }

    public string allGameTime => FormatTime(curGameTimeSeconds + hisGameTimeSeconds);
    public string curGameTime => FormatTime(curGameTimeSeconds);


    public void StartNewSession()
    {
        sessionStartTime = DateTime.UtcNow;
    }

    public void CommitSessionTime()
    {
        if (sessionStartTime != null)
        {
            curGameTimeSeconds = (DateTime.UtcNow - sessionStartTime).TotalSeconds;
        }
    }
    public string FormatTime(double timeInSeconds)
    {
        int hours = Mathf.FloorToInt((float)timeInSeconds / 3600);
        int minutes = Mathf.FloorToInt((float)(timeInSeconds % 3600) / 60);
        int seconds = Mathf.FloorToInt((float)timeInSeconds % 60);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
    public void SetScreenshot(Texture2D tex)
    {
        if (tex == null)
        {
            screenshotBase64 = null;
            return;
        }
        byte[] pngData = tex.EncodeToPNG();
        screenshotBase64 = Convert.ToBase64String(pngData);
    }

    public Texture2D GetScreenshotTexture()
    {
        if (string.IsNullOrEmpty(screenshotBase64))
            return null;

        try
        {
            byte[] pngData = Convert.FromBase64String(screenshotBase64);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(pngData))
                return tex;
        }
        catch
        {
            // 解码失败
        }
        return null;
    }
}