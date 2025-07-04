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
        if (isPlayerNearby && Mouse.current.leftButton.wasPressedThisFrame && !InspectManager.Instance.IsInspecting())
        {
            InspectManager.Instance.StartInspect(inspectPrefab);
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