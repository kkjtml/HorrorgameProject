using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLightPulse : MonoBehaviour
{
    public Light redLight;
    public float maxIntensity = 5f;
    public float blinkSpeed = 4f;

    private float intensityFactor = 0f;

    void Update()
    {
        if (redLight == null) return;

        float pulse = Mathf.Sin(Time.time * blinkSpeed) * 0.5f + 0.5f;
        redLight.intensity = intensityFactor * maxIntensity * pulse;

        // เปลี่ยนสีจากอ่อน → เข้ม (แดงอ่อน → แดงสด)
        Color color = Color.Lerp(new Color(1f, 0.6f, 0.6f), Color.red, intensityFactor);
        redLight.color = color;
    }

    public void SetLightIntensity(float value)
    {
        intensityFactor = Mathf.Clamp01(value);
    }
}