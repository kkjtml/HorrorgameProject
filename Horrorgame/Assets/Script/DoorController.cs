using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public enum DoorUnlockCondition
{
    AfterLantern2,
    PuzzlePictureQuest,
    AfterFoundKey
}

public class DoorController : MonoBehaviour
{
    public Transform doorTransform;
    public Vector3 openRotationOffset = new Vector3(0, -90, 0);
    public float openSpeed = 2f;

    private bool isPlayerNearby = false;
    private bool isOpen = false;
    private bool isUnlocked = false;

    public DoorUnlockCondition unlockCondition;

    private Quaternion closedRotation; // Y = 90
    private Quaternion openRotation;   // Y = 0 (หรือ 180 ถ้าเปลี่ยน)
    private Quaternion targetRotation;

    private bool hasAutoOpened = false;

    void Start()
    {
        closedRotation = doorTransform.localRotation;
        openRotation = Quaternion.Euler(closedRotation.eulerAngles + openRotationOffset);
        targetRotation = closedRotation;
    }

    void Update()
    {
        if (!isUnlocked)
        {
            switch (unlockCondition)
            {
                case DoorUnlockCondition.AfterLantern2:
                    if (LanternManager.Instance != null && LanternManager.Instance.nextLanternIndex > 1)
                    {
                        isUnlocked = true;
                        Debug.Log("✅ ปลดล็อกเพราะจุดตะเกียงดวงที่ 2 แล้ว");
                    }
                    break;

                case DoorUnlockCondition.PuzzlePictureQuest:
                    if (QuestManager.Instance != null && QuestManager.Instance.IsSearchingOppositeRoom())
                    {
                        isUnlocked = true;
                        Debug.Log("✅ ปลดล็อกเพราะเริ่มเควสภาพปริศนาแล้ว");
                    }
                    break;

                case DoorUnlockCondition.AfterFoundKey:
                    if (QuestManager.Instance != null && QuestManager.Instance.HasFoundKey())
                    {
                        isUnlocked = true;
                        Debug.Log("✅ ปลดล็อกเพราะพบกุญแจแล้ว");
                    }
                    break;
            }
        }

        if (isUnlocked && !hasAutoOpened)
        {
            // แง้ม 30 องศาโดยอัตโนมัติ
            Quaternion slightlyOpen = Quaternion.Euler(closedRotation.eulerAngles + new Vector3(0, openRotationOffset.y * 0.33f, 0));
            targetRotation = slightlyOpen;
            hasAutoOpened = true;
        }

        // ✅ รองรับ Mouse Left Click หรือ Gamepad Button South (A / X)
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool gamepadPressed = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;

        if (isUnlocked && isPlayerNearby && (mousePressed || gamepadPressed))
        {
            ToggleDoor();

            if (unlockCondition == DoorUnlockCondition.AfterFoundKey)
            {
                DialogueManager.Instance?.Show("ฉันออกจากที่นี่ได้แล้ว", 2f);

                // ✅ อัปเดตเควส
                QuestManager.Instance?.SetEscapeForestQuest();
            }
        }
        else if (isPlayerNearby && (mousePressed || gamepadPressed))
        {
            DialogueManager.Instance?.Show("ประตูล็อค ฉันเปิดไม่ได้", 1f);
        }

        doorTransform.localRotation = Quaternion.Slerp(
            doorTransform.localRotation, targetRotation, Time.deltaTime * openSpeed);
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        targetRotation = isOpen ? openRotation : closedRotation;

        Debug.Log("🌀 Door toggled to: " + (isOpen ? "OPEN" : "CLOSED"));
    }

    public void SetPlayerNearby(bool state)
    {
        isPlayerNearby = state;
    }

    public void UnlockManually()
    {
        isUnlocked = true;
        Debug.Log("🔓 ประตูนี้ถูกปลดล็อกด้วยกุญแจ inspect");
    }

    public bool IsOpen() => isOpen;
    public void OpenByGhost()
    {
        if (!isOpen)
        {
            isOpen = true;
            targetRotation = openRotation;

            Debug.Log("👻 ผีเปิดประตู");
        }
    }

    public void CloseByGhost()
    {
        if (isOpen)
        {
            isOpen = false;
            targetRotation = closedRotation;
            Debug.Log("🚪 ผีปิดประตู");
        }
    }

    public bool IsUnlocked() => isUnlocked;

    // public void DisableObstacle()
    // {
    //     if (navObstacle != null)
    //     {
    //         navObstacle.enabled = false;
    //         navObstacle.carving = false;
    //         Debug.Log("🚪 Obstacle ถูกปิดโดยผี");
    //     }

    //     if (doorBlocker != null)
    //     {
    //         doorBlocker.SetActive(false); // ถ้ามีตัว block ธรรมดา
    //     }
    // }

}