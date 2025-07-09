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

    // void Update()
    // {
    //     if (playerInRange && Input.GetMouseButtonDown(0))
    //     {
    //         ToggleLantern();
    //     }
    // }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (isLit) return;

        if (ClueNoteManager.Instance?.IsClueShowing() == true) return;
        if (!QuestManager.Instance?.HasSeenClueNote() == true)
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

    // void ToggleLantern()
    // {
    //     if (isLit) return;

    //     // ตรวจว่ากำลังดู Clue อยู่ → ห้ามจุด
    //     if (ClueNoteManager.Instance != null && ClueNoteManager.Instance.IsClueShowing())
    //         return;

    //     // 🟥 ยังไม่ได้อ่านโน้ต Clue 0
    //     if (!QuestManager.Instance.HasSeenClueNote())
    //     {
    //         DialogueManager.Instance?.Show("ดูเหมือนจะเป็นตะเกียงเก่าๆ", 2f);
    //         return;
    //     }

    //     // ✅ อ่านโน้ตแล้ว แต่จุดผิดดวง
    //     if (!LanternManager.Instance.CanLightLantern(lanternIndex))
    //     {
    //         DialogueManager.Instance?.Show("จุดตะเกียงไม่ถูกต้อง...", 2f);
    //         DialogueManager.Instance?.Queue("ต้องจุดตะเกียงเรียงทวนเข็มนาฬิกาเท่านั้นสิ", 3f);
    //         Debug.Log("❌ Cannot light lantern " + lanternIndex + " yet");
    //         return;
    //     }

    //     isLit = true;
    //     lanternLight.enabled = true;

    //     DialogueManager.Instance?.Show("จุดตะเกียงถูกต้องแล้ว", 2f);

    //     LanternManager.Instance.LightLantern(lanternIndex);
    // }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         playerInRange = true;
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         playerInRange = false;
    //     }
    // }
}