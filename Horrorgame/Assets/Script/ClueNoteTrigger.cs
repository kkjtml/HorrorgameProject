using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClueNoteTrigger : MonoBehaviour
{
    public int clueIndex = 0;
    private bool isPlayerNearby = false;

    // void Update()
    // {
    //     if (isPlayerNearby && Input.GetMouseButtonDown(0) && !ClueNoteManager.Instance.IsClueShowing())
    //     {
    //         if (clueIndex == 0 && QuestManager.Instance != null && QuestManager.Instance.HasFinishedLanternQuest())
    //         {
    //             Debug.Log("🚫 Clue 0 is disabled after lantern quest is done.");
    //             return;
    //         }
    //         ClueNoteManager.Instance.ShowClue(clueIndex);
    //     }
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //         isPlayerNearby = true;
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //         isPlayerNearby = false;
    // }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (ClueNoteManager.Instance.IsClueShowing()) return;

        if (clueIndex == 0 && QuestManager.Instance?.HasFinishedLanternQuest() == true)
        {
            Debug.Log("🚫 Clue 0 is disabled after lantern quest is done.");
            return;
        }

        ClueNoteManager.Instance.ShowClue(clueIndex);
    }
}