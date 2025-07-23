using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorController door;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.SetPlayerNearby(true);
            // ✅ ผีจะจำประตูนี้ ถ้า "กำลัง Chase"
            if (GhostAI.Instance != null && GhostAI.Instance.IsChasing())
            {
                GhostAI.Instance.SetPlayerTargetDoor(door);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.SetPlayerNearby(false);
        }
    }
}