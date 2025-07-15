using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class LanternShakeEffect : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Image redOverlayImage;
    public float shakeDuration = 3f;
    public float maxShakeIntensity = 5f;
    public float maxOverlayAlpha = 0.5f;

    // private float timer = 0f;
    // private bool isShaking = false;
    private CinemachineBasicMultiChannelPerlin perlin;
    private float shakePower;

    void Start()
    {
        if (virtualCamera != null)
            perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // public void TriggerShake()
    // {
    //     redOverlayImage.color = new Color(1f, 0f, 0f, 0.8f); // สีแดงทึบ
    //     isShaking = true;
    //     timer = 0f;
    // }

    // void Update()
    // {
    //     if (!isShaking || perlin == null) return;

    //     timer += Time.deltaTime;
    //     float t = timer / shakeDuration;

    //     if (t >= 1f)
    //     {
    //         perlin.m_AmplitudeGain = 0f;
    //         SetOverlayAlpha(0f);
    //         isShaking = false;
    //         return;
    //     }

    //     float intensity = Mathf.Lerp(0f, maxShakeIntensity, t);
    //     perlin.m_AmplitudeGain = intensity;
    //     SetOverlayAlpha(Mathf.Lerp(0f, maxOverlayAlpha, t));
    // }

    void SetOverlayAlpha(float alpha)
    {
        if (redOverlayImage != null)
        {
            Color color = redOverlayImage.color;
            color.a = Mathf.Clamp01(alpha);
            redOverlayImage.color = color;
        }

        var cg = redOverlayImage.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = alpha;
        }
    }

    public void SetShakePower(float power)
    {
        shakePower = Mathf.Clamp01(power);

        if (perlin != null)
            perlin.m_AmplitudeGain = shakePower * maxShakeIntensity;

        SetOverlayAlpha(shakePower * maxOverlayAlpha);
    }

    public float GetShakePower()
    {
        return shakePower;
    }

    public void SetVirtualCamera(CinemachineVirtualCamera vcam)
    {
        virtualCamera = vcam;
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
}