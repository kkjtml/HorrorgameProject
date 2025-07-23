using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public DoorController linkedDoor;

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player") && GhostAI.Instance != null && GhostAI.Instance.IsChasing())
    //     {
    //         if (linkedDoor != null && linkedDoor.IsUnlocked())
    //         {
    //             Debug.Log("✅ RoomTrigger เข้าแล้ว ผีจะจำประตูนี้: " + linkedDoor.name);
    //             GhostAI.Instance.SetPlayerTargetDoor(linkedDoor);
    //         }
    //     }
    // }
}
