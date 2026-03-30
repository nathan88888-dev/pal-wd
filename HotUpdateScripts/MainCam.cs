using UnityEngine;

[System.Serializable]
public class MainCam : MonoBehaviour
{
    public Transform followTarget { set; get; }
    private float distance = 15f;    // 与目标之间的直线距离
    private float angle = 60f;      // 俯视角度（单位为度）
    private Camera cam;

    void Awake()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        followTarget = null;
        cam = Camera.main;
    }

    public void SetFollowUp(Transform follow) {
        followTarget = follow;
    }

    void LateUpdate()
    {
        if (followTarget == null) return;

        float radians = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(0, Mathf.Sin(radians), -Mathf.Cos(radians)) * distance;
        transform.position = followTarget.position + offset;
        transform.LookAt(followTarget);
    }
    public Texture2D CaptureCameraScreenshot(int width = 430, int height=241)
    {
        // 创建一个RenderTexture
        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;

        // 让摄像机渲染
        cam.Render();

        // 激活RenderTexture，读取像素到Texture2D
        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // 清理
        cam.targetTexture = null;
        RenderTexture.active = null;
        UnityEngine.Object.Destroy(rt);

        return screenshot;
    }
}
