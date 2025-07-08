using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerTrigger : MonoBehaviour
{
    public DrawerController drawer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            drawer.SetPlayerNearby(true);
            Debug.Log("✅ Player เข้าใกล้ลิ้นชัก");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            drawer.SetPlayerNearby(false);
            Debug.Log("⛔ Player ออกห่างลิ้นชัก");
        }
    }
}
