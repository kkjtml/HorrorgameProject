using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PicturePuzzleTrigger : MonoBehaviour
{
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (QuestManager.Instance != null && QuestManager.Instance.IsSearchingForMysteryPhoto())
            {
                PicturePuzzleUI.Instance?.OpenPuzzle();
            }
            else
            {
                DialogueManager.Instance?.Show("ฉันยังไม่รู้ว่าภาพเหล่านี้คืออะไร...");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = false;
    }
}