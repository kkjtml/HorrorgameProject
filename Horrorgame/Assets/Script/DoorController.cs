using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DoorUnlockCondition
{
    AfterLantern2,
    AfterMysteryPhotoQuest
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

                case DoorUnlockCondition.AfterMysteryPhotoQuest:
                    if (QuestManager.Instance != null && QuestManager.Instance.IsSearchingForMysteryPhoto())
                    {
                        isUnlocked = true;
                        Debug.Log("✅ ปลดล็อกเพราะเริ่มเควสภาพปริศนาแล้ว");
                    }
                    break;
            }
        }

        if (isUnlocked && isPlayerNearby && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ToggleDoor();
        }
        else if (isPlayerNearby && Mouse.current.leftButton.wasPressedThisFrame)
        {
            DialogueManager.Instance?.Show("ประตูล็อค ฉันเปิดไม่ได้",1f);
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

}