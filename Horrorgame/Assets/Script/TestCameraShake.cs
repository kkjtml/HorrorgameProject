using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TestCameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var perlin = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = 2;
            Debug.Log("Test shake applied");
        }
    }
}