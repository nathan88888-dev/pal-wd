using UnityEngine;

public class YAxisOscillator : MonoBehaviour
{
    public float amplitude = 20f;    // 摆动幅度 [-amplitude, amplitude]
    public float frequency = 2f;     // 摆动速度 Hz（次数/秒）

    private float initialZ;
    private float phaseOffset;

    void Start()
    {
        initialZ = transform.localPosition.y;
        phaseOffset = Random.Range(0f, 2 * Mathf.PI);  // 随机相位偏移，避免同步
    }

    void Update()
    {
        float zOffset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2 + phaseOffset) * amplitude;
        Vector3 currentPosition = transform.localPosition;
        currentPosition.y = initialZ + zOffset;
        transform.localPosition = currentPosition;
    }
}
