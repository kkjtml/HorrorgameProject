using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InspectableObject : MonoBehaviour
{
    public GameObject inspectPrefab;
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            InspectManager.Instance.StartInspect(inspectPrefab);
        }

        else if (Keyboard.current.eKey.wasPressedThisFrame && InspectManager.Instance.IsInspecting())
        {
            InspectManager.Instance.EndInspect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}