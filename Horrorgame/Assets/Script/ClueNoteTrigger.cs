using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClueNoteTrigger : MonoBehaviour
{
    public int clueIndex = 0;
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Mouse.current.leftButton.wasPressedThisFrame && !ClueNoteManager.Instance.IsClueShowing())
        {
            if (clueIndex == 0 && QuestManager.Instance != null && QuestManager.Instance.HasFinishedLanternQuest())
            {
                Debug.Log("🚫 Clue 0 is disabled after lantern quest is done.");
                return;
            }
            ClueNoteManager.Instance.ShowClue(clueIndex);
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