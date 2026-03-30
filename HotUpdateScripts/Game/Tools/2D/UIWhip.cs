using UnityEngine;
using UnityEngine.UI;

public class UIWhip : MonoBehaviour
{
    public RectTransform[] segments; // 4段鞭子
    public float swingAmplitude = 10; // 摆动角度最大值
    public float swingSpeed = 2; // 摆动速度
    public float segmentDelay = 1; // 相邻段之间的延迟

    private float time;

    private float phaseOffset;

    void Start()
    {
        phaseOffset = Random.Range(0f, 2 * Mathf.PI);  // 随机相位偏移，避免同步
    }
    void Update()
    {
        time += Time.deltaTime;

        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i] == null) continue;

            // 为每一段加上延迟相位，使得像波一样传播
            float angle = Mathf.Sin(time * swingSpeed - i * segmentDelay+ phaseOffset) * swingAmplitude;

            // 旋转该段
            segments[i].localRotation = Quaternion.Euler(0, 0, angle);
        }

        // 保证后续段跟随前一段的末端位置
        for (int i = 1; i < segments.Length; i++)
        {
            segments[i].position = segments[i - 1].GetChild(0).position;
        }
    }
}
