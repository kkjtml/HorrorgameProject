using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    public Transform doorTransform;
    public Collider doorCollider;
    public Vector3 openRotationOffset = new Vector3(0, -90, 0);
    public float openSpeed = 2f;

    private bool isPlayerNearby = false;
    private bool isOpen = false;
    private bool isUnlocked = false;

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
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("🔁 [TEST] force open");
            isOpen = true;
            targetRotation = openRotation;
            doorCollider.enabled = false; // 👈 ปิด Collider เมื่อเปิด
        }

        if (!isUnlocked && LanternManager.Instance != null && LanternManager.Instance.nextLanternIndex > 0)
        {
            isUnlocked = true;
            Debug.Log("✅ ประตูสามารถเปิดได้หลังจุดตะเกียงดวงที่ 1!");
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("🖱️ Left Click pressed");
        }

        // คลิกซ้ายเปิด/ปิดถ้าอยู่ใกล้
        if (isUnlocked && isPlayerNearby && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ToggleDoor();
        }

        // Smooth animation
        doorTransform.localRotation = Quaternion.Slerp(doorTransform.localRotation, targetRotation, Time.deltaTime * openSpeed);
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        targetRotation = isOpen ? openRotation : closedRotation;

        if (doorCollider != null)
            doorCollider.enabled = !isOpen;

        Debug.Log("🌀 Door toggled to: " + (isOpen ? "OPEN" : "CLOSED"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = true;
        Debug.Log("🟢 Player เข้าบริเวณประตู");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = false;
        Debug.Log("🔴 Player ออกห่างจากประตู");
    }
}