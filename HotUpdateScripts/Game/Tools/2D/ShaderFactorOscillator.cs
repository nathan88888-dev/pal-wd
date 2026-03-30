using UnityEngine;
using UnityEngine.UI;

public class ShaderFactorOscillator : MonoBehaviour
{
    private string shaderProperty = "_Factor";  // Shader中要控制的参数名
    private float min = 0f;
    private float max = 2f;
    private float speed = 3f;  // 周期速度

    public Material material;
    private float t;  // 时间累加器


    void Update()
    {
        t += Time.deltaTime * speed;

        // 使用sin波形做周期摆动，范围映射到[min, max]
        float value = Mathf.Lerp(min, max, (Mathf.Sin(t) + 1f) / 2f);

        // 将值设置到shader中
        material.SetFloat(shaderProperty, value);
    }
}
