using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PicturePuzzleTrigger : MonoBehaviour
{
    private bool isPlayerNearby = false;

    // void Update()
    // {
    //     if (isPlayerNearby && Input.GetMouseButtonDown(0))
    //     {
    //         if (PicturePuzzleUI.Instance != null && PicturePuzzleUI.Instance.IsPuzzleCompleted())
    //         {
    //             return;
    //         }

    //         if (QuestManager.Instance != null && QuestManager.Instance.IsSearchingForMysteryPhoto())
    //         {
    //             PicturePuzzleUI.Instance?.OpenPuzzle();
    //         }
    //         // else
    //         // {
    //         //     DialogueManager.Instance?.Show("ฉันยังไม่รู้ว่าภาพเหล่านี้คืออะไร...", 2f);
    //         // }
    //     }
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player")) isPlayerNearby = true;
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player")) isPlayerNearby = false;
    // }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!Input.GetMouseButtonDown(0)) return;

        if (PicturePuzzleUI.Instance != null && PicturePuzzleUI.Instance.IsPuzzleCompleted()) return;
        if (QuestManager.Instance != null && QuestManager.Instance.IsSearchingForMysteryPhoto())
        {
            PicturePuzzleUI.Instance?.OpenPuzzle();
        }
    }
}