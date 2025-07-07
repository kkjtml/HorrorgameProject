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
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!CanInspectBasedOnCondition())
            {
                // DialogueManager.Instance?.Show("ฉันควรหาคำใบ้ก่อน...", 2f);
                return;
            }

            // ✅ แสดงข้อความทุกบรรทัดแบบต่อเนื่อง
            if (dialogueLines != null && dialogueLines.Length > 0)
            {
                DialogueManager.Instance?.Show(dialogueLines[0], 2f);
                for (int i = 1; i < dialogueLines.Length; i++)
                {
                    DialogueManager.Instance?.Queue(dialogueLines[i], 2f);
                }
            }

            // ✅ ส่งตัวที่อยู่ใน scene จริงเข้าไปเป็น `originalSource`
            InspectManager.Instance?.StartInspect(prefabToInspect, this.gameObject);
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

            default:
                return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = false;
    }
}