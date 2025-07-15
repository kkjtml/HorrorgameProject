using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Cinemachine;

public class HideableCabinet : MonoBehaviour
{
    // public CabinetDoorController doorController;
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

    void Start()
    {
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
        if (!Input.GetMouseButtonDown(0)) return;

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

        charController.enabled = false;
        player.transform.position = hidePoint.position;
        player.transform.rotation = Quaternion.LookRotation(hidePoint.forward);
        charController.enabled = true;

        playerController.enabled = false;
        playerController.ResetAnimation();

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

        charController.enabled = false;
        player.transform.position = exitPoint.position;
        player.transform.rotation = Quaternion.LookRotation(exitPoint.forward);
        charController.enabled = true;

        playerController.enabled = true;

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
