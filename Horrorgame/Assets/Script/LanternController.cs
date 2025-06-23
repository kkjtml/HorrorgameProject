using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LanternController : MonoBehaviour
{
    public Light lanternLight;
    public LanternShakeEffect shakeEffect;
    private bool isLit = false;
    private bool playerInRange = false;

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

        isLit = true;
        lanternLight.enabled = true;
        shakeEffect?.TriggerShake(); 
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