using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LanternController : MonoBehaviour, IInteractable
{
    public Light lanternLight;
    private bool isLit = false;
    public int lanternIndex = 0;

    void Start()
    {
        if (lanternLight != null)
            lanternLight.enabled = false;
    }

    // void Update()
    // {
    //     if (playerInRange && Input.GetMouseButtonDown(0))
    //     {
    //         ToggleLantern();
    //     }
    // }

    // private void OnTriggerStay(Collider other)
    // {
    //     if (!other.CompareTag("Player")) return;
    //     if (isLit) return;

    //     // ✅ รองรับ Mouse Left Click หรือ Gamepad Button South (A / X)
    //     bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    //     bool gamepadPressed = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;

    //     if (mousePressed || gamepadPressed)
    //     {
    //         if (ClueNoteManager.Instance?.IsClueShowing() == true) return;
    //         if (!QuestManager.Instance?.HasSeenClueNote() == true)
    //         {
    //             DialogueManager.Instance?.Show("ดูเหมือนจะเป็นตะเกียงเก่าๆ", 2f);
    //             return;
    //         }

    //         if (!LanternManager.Instance.CanLightLantern(lanternIndex))
    //         {
    //             DialogueManager.Instance?.Show("จุดตะเกียงไม่ถูกต้อง...", 2f);
    //             DialogueManager.Instance?.Queue("ต้องจุดตะเกียงเรียงทวนเข็มนาฬิกาเท่านั้นสิ", 3f);
    //             return;
    //         }

    //         isLit = true;
    //         lanternLight.enabled = true;
    //         DialogueManager.Instance?.Show("จุดตะเกียงถูกต้องแล้ว", 2f);
    //         LanternManager.Instance.LightLantern(lanternIndex);
    //     }
    // }

    public void Interact()
    {
        if (isLit) return;

        if (ClueNoteManager.Instance?.IsClueShowing() == true) return;
        if (!QuestManager.Instance || !QuestManager.Instance.HasSeenClueNote())
        {
            DialogueManager.Instance?.Show("ดูเหมือนจะเป็นตะเกียงเก่าๆ", 2f);
            return;
        }

        if (!LanternManager.Instance.CanLightLantern(lanternIndex))
        {
            DialogueManager.Instance?.Show("จุดตะเกียงไม่ถูกต้อง...", 2f);
            DialogueManager.Instance?.Queue("ต้องจุดตะเกียงเรียงทวนเข็มนาฬิกาเท่านั้นสิ", 3f);
            return;
        }

        isLit = true;
        lanternLight.enabled = true;
        DialogueManager.Instance?.Show("จุดตะเกียงถูกต้องแล้ว", 2f);
        LanternManager.Instance.LightLantern(lanternIndex);
    }

    public void SetHighlighted(bool highlighted)
    {
        if (isLit) return;

        SelectionGlow glow = GetComponent<SelectionGlow>();
        if (glow != null)
        {
            glow.SetGlowEnabled(highlighted);
        }
    }

    public bool CanBeInteracted()
    {
        return !isLit;
    }
}