using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClueNoteTrigger : MonoBehaviour
{
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Mouse.current.leftButton.wasPressedThisFrame && !ClueNoteManager.Instance.IsClueShowing())
        {
            Debug.Log("🔍 Player clicked to read clue");
            ClueNoteManager.Instance.ShowClue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = false;
    }
}