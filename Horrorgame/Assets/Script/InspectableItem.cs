using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InspectableItem : MonoBehaviour
{
    public GameObject prefabToInspect;
    public InspectCondition condition = InspectCondition.Always;

    [TextArea(2, 4)]
    public string[] dialogueLines;  // ✅ หลายข้อความ

    public bool isKey = false;
    private bool hasCollectedKey = false;
    private bool hasInteracted = false;

    // void Update()
    // {
    //     if (isPlayerNearby && Input.GetMouseButtonDown(0) && !DialogueManager.Instance.IsShowing())
    //     {
    //         if (!CanInspectBasedOnCondition())
    //         {
    //             // DialogueManager.Instance?.Show("ฉันควรหาคำใบ้ก่อน...", 2f);
    //             return;
    //         }

    //         // ✅ แสดงข้อความทุกบรรทัดแบบต่อเนื่อง
    //         if (dialogueLines != null && dialogueLines.Length > 0)
    //         {
    //             DialogueManager.Instance?.Show(dialogueLines[0], 2f);
    //             for (int i = 1; i < dialogueLines.Length; i++)
    //             {
    //                 DialogueManager.Instance?.Queue(dialogueLines[i], 2f);
    //             }
    //         }

    //         if (isKey && !hasCollectedKey)
    //         {
    //             hasCollectedKey = true;
    //             InspectManager.Instance?.SetKeyItem(this.gameObject);
    //         }

    //         // ✅ ส่งตัวที่อยู่ใน scene จริงเข้าไปเป็น `originalSource`
    //         InspectManager.Instance?.StartInspect(prefabToInspect, this.gameObject);
    //     }
    // }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (DialogueManager.Instance.IsShowing()) return;
        if (hasInteracted) return;

        // ✅ รองรับ Mouse Left Click หรือ Gamepad Button South (A / X)
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool gamepadPressed = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;

        if (mousePressed || gamepadPressed)
        {
            if (!CanInspectBasedOnCondition()) return;

            if (dialogueLines != null && dialogueLines.Length > 0)
            {
                DialogueManager.Instance?.Show(dialogueLines[0], 2f);
                for (int i = 1; i < dialogueLines.Length; i++)
                    DialogueManager.Instance?.Queue(dialogueLines[i], 2f);
            }

            if (isKey && !hasCollectedKey)
            {
                hasCollectedKey = true;
                InspectManager.Instance?.SetKeyItem(this.gameObject);
            }

            InspectManager.Instance?.StartInspect(prefabToInspect, this.gameObject);

            // ✅ ให้ InspectManager บอกให้ reset ตอนปิด
            InspectManager.Instance?.SetInspectable(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasInteracted = false; // reset เมื่อออกจาก trigger
        }
    }

    private bool CanInspectBasedOnCondition()
    {
        switch (condition)
        {
            case InspectCondition.Always:
                return true;

            case InspectCondition.AfterClue1:
                return QuestManager.Instance != null && QuestManager.Instance.HasSeenClue1();

            case InspectCondition.AfterClue2:
                return QuestManager.Instance != null && QuestManager.Instance.HasSeenClue2();

            case InspectCondition.AfterLanternDone:
                return QuestManager.Instance != null && QuestManager.Instance.HasFinishedLanternQuest();

            case InspectCondition.AfterClue3:
                return QuestManager.Instance != null && QuestManager.Instance.HasSeenClue3();

            default:
                return false;
        }
    }

    public void ResetInteract()
    {
        hasInteracted = false;
    }


    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //         isPlayerNearby = true;
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //         isPlayerNearby = false;
    // }
}