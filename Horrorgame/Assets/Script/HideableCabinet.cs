using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Cinemachine;
using UnityEngine.InputSystem;

public class HideableCabinet : MonoBehaviour
{
    public CabinetDoorController doorController;
    public Transform hidePoint;
    public Transform exitPoint;

    public GameObject mainCamera;
    public GameObject cabinetCamera;

    private StarterAssets.ThirdPersonController playerController;
    private CharacterController charController;
    private GameObject player;

    private bool isHiding = false;
    private bool isPlayerNearby = false;

    public LanternShakeEffect sharedShakeEffect;
    public CinemachineVirtualCamera mainVirtualCam;
    public CinemachineVirtualCamera cabinetVirtualCam;

    private SelectionGlow[] allGlowItems;

    void Start()
    {
        allGlowItems = FindObjectsOfType<SelectionGlow>();

        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO;
            playerController = player.GetComponent<ThirdPersonController>();
            charController = player.GetComponent<CharacterController>();
        }

        if (cabinetCamera != null)
            cabinetCamera.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = false;
    }

    // void Update()
    // {
    //     if (!isPlayerNearby) return;
    //     if (!Input.GetMouseButtonDown(0)) return;

    //     if (isHiding)
    //     {
    //         ExitHiding();
    //         doorController?.Close();
    //     }
    //     else if (!doorController.IsOpen())
    //     {
    //         doorController?.Open();
    //     }
    //     else
    //     {
    //         EnterHiding();
    //     }
    // }

    void Update()
    {

        if (!isPlayerNearby) return;

        // ✅ รองรับ Mouse Left Click และ Gamepad A (buttonSouth)
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool gamepadPressed = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;
        if (!mousePressed && !gamepadPressed) return;

        if (isHiding)
        {
            ExitCabinet();
        }
        else
        {
            EnterCabinet();
        }
    }

    private void EnterCabinet()
    {
        if (playerController == null || charController == null) return;

        // ✅ ปิดประตูทันทีเมื่อซ่อน
        doorController?.Close();

        doorController?.SaveCurrentRotation(); // ✅ จำสถานะไว้ก่อนปิด

        charController.enabled = false;
        player.transform.position = hidePoint.position;
        player.transform.rotation = Quaternion.LookRotation(hidePoint.forward);
        charController.enabled = true;

        playerController.enabled = false;
        playerController.ResetAnimation();

        // ปิด Glow ทั้งหมดเมื่อซ่อน
        if (allGlowItems != null)
        {
            foreach (var glow in allGlowItems)
                glow.ForceDisableGlow(true);
        }

        // 🎯 ปิดไฟฉายตอนซ่อน
        var flashlight = playerController.flashlight;
        if (flashlight != null)
            flashlight.enabled = false;

        // 🎥 เปลี่ยนกล้องทันที
        if (mainCamera != null) mainCamera.SetActive(false);
        if (cabinetCamera != null) cabinetCamera.SetActive(true);

        // ✅ เปลี่ยน virtualCamera ที่ shakeEffect ใช้ให้เป็นของตู้
        if (sharedShakeEffect != null && cabinetVirtualCam != null)
            sharedShakeEffect.SetVirtualCamera(cabinetVirtualCam);

        GhostAI.Instance?.SetPlayerHidden(true);
        isHiding = true;

        Debug.Log("🛏️ Player is now hiding");
    }

    private void ExitCabinet()
    {
        if (playerController == null || charController == null) return;

        doorController?.RestorePreviousRotation(); // ✅ คืน rotation กลับ

        // ✅ เปิดประตูทันทีเมื่อออกจากการซ่อน
        doorController?.Open();

        charController.enabled = false;
        player.transform.position = exitPoint.position;
        player.transform.rotation = Quaternion.LookRotation(-exitPoint.forward);
        charController.enabled = true;

        playerController.enabled = true;

        // เปิด Glow กลับเมื่อออกจากตู้
        if (allGlowItems != null)
        {
            foreach (var glow in allGlowItems)
                glow.ForceDisableGlow(false); // เปิดกลับ ถ้าเข้าเงื่อนไข
        }

        // เปิดไฟฉายเมื่อออกจากตู้
        var flashlight = playerController.flashlight;
        if (flashlight != null)
            flashlight.enabled = true;

        // 🎥 สลับกลับกล้องหลัก
        if (cabinetCamera != null) cabinetCamera.SetActive(false);
        if (mainCamera != null) mainCamera.SetActive(true);

        // ✅ เปลี่ยนกลับไปใช้กล้องหลัก
        if (sharedShakeEffect != null && mainVirtualCam != null)
            sharedShakeEffect.SetVirtualCamera(mainVirtualCam);

        GhostAI.Instance?.SetPlayerHidden(false);
        isHiding = false;

        Debug.Log("🚪 Player exited cabinet");
    }

    // public bool IsPlayerHiding() => isHiding;
}
