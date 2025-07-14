using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class HideableCabinet : MonoBehaviour
{
    public CabinetDoorController doorController;
    public Transform hidePoint;
    public Transform exitPoint;

    private StarterAssets.ThirdPersonController playerController;
    private CharacterController charController;
    private GameObject player;

    private bool isHiding = false;

    void Start()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO;
            playerController = player.GetComponent<ThirdPersonController>();
            charController = player.GetComponent<CharacterController>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!Input.GetMouseButtonDown(0)) return;

        if (isHiding)
        {
            ExitHiding();
            doorController?.Close();
        }
        else if (!doorController.IsOpen())
        {
            doorController?.Open();
        }
        else
        {
            EnterHiding();
        }
    }

    private void EnterHiding()
    {
        if (playerController == null || charController == null) return;

        charController.enabled = false;
        player.transform.position = hidePoint.position;
        charController.enabled = true;

        playerController.enabled = false;

        GhostAI.Instance?.SetPlayerHidden(true);
        isHiding = true;

        Debug.Log("🛏️ Player is hiding");
    }

    private void ExitHiding()
    {
        if (playerController == null || charController == null) return;

        charController.enabled = false;

        if (exitPoint != null)
        {
            player.transform.position = exitPoint.position;
            Debug.Log("📍 Move to exit point: " + exitPoint.position);
        }

        charController.enabled = true;
        playerController.enabled = true;

        GhostAI.Instance?.SetPlayerHidden(false);
        isHiding = false;

        Debug.Log("🚪 Player exited hiding");
    }

    public bool IsPlayerHiding() => isHiding;
}
