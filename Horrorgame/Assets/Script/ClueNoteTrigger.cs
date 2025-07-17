using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClueNoteTrigger : MonoBehaviour
{
    public int clueIndex = 0;
    private bool hasInteracted = false;

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
        if (ClueNoteManager.Instance == null || ClueNoteManager.Instance.IsClueShowing()) return;
        if (hasInteracted) return;

        // ✅ รองรับ Mouse Left Click หรือ Gamepad Button South (A / X)
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool gamepadPressed = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;

        if (mousePressed || gamepadPressed)
        {
            if (clueIndex == 0 && QuestManager.Instance?.HasFinishedLanternQuest() == true)
            {
                Debug.Log("🚫 Clue 0 is disabled after lantern quest is done.");
                return;
            }

            ClueNoteManager.Instance.ShowClue(clueIndex);
            hasInteracted = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasInteracted = false; // reset เมื่อออกจาก trigger
        }
    }

    public void ResetInteract()
    {
        hasInteracted = false;
    }

}