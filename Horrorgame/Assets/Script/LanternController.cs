using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LanternController : MonoBehaviour
{
    public Light lanternLight;
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
        if (playerInRange && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ToggleLantern();
        }
    }

    void ToggleLantern()
    {
        if (isLit) return;

        if (!LanternManager.Instance.CanLightLantern(lanternIndex))
        {
            DialogueManager.Instance?.Show("จุดตะเกียงไม่ถูกต้อง...", 2f); 
            DialogueManager.Instance?.Queue("ต้องจุดตะเกียงเรียงทวนเข็มนาฬิกาเท่านั้นสิ", 3f); 
            Debug.Log("❌ Cannot light lantern " + lanternIndex + " yet");
            return;
        }

        isLit = true;
        lanternLight.enabled = true;

        LanternManager.Instance.LightLantern(lanternIndex);
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