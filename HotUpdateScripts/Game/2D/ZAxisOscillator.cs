using UnityEngine;

public class ZAxisOscillator : MonoBehaviour
{
    public float amplitude = 10f;    // 摆动幅度 [-amplitude, amplitude]
    public float frequency = 1f;     // 摆动速度 Hz（次数/秒）

    private float initialZ;
    private float phaseOffset;

    void Start()
    {
        initialZ = transform.localEulerAngles.z;
        phaseOffset = Random.Range(0f, 2 * Mathf.PI);  // 随机相位偏移，避免同步
    }

    void Update()
    {
        float zOffset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2 + phaseOffset) * amplitude;
        Vector3 currentPosition = transform.localEulerAngles;
        currentPosition.z = initialZ + zOffset;
        transform.localEulerAngles = currentPosition;
    }
}
