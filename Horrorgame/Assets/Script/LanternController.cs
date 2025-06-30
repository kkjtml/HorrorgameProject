using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LanternController : MonoBehaviour
{
    public Light lanternLight;
    // public LanternShakeEffect shakeEffect;
    private bool isLit = false;
    private bool playerInRange = false;

    public int lanternIndex = 0;

    void Start()
    {
        if (lanternLight != null)
            lanternLight.enabled = false;
    }

    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleLantern();
        }
    }

    void ToggleLantern()
    {
        if (isLit) return;

        if (!LanternManager.Instance.CanLightLantern(lanternIndex))
        {
            Debug.Log("❌ Cannot light lantern " + lanternIndex + " yet");
            return;
        }

        isLit = true;
        lanternLight.enabled = true;
        // shakeEffect?.TriggerShake();

        LanternManager.Instance.OnLanternLit(lanternIndex);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}