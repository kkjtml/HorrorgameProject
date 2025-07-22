using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PicturePuzzleTrigger : MonoBehaviour, IInteractable
{
    private bool hasInteracted = false;

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

    // private void OnTriggerStay(Collider other)
    // {
    //     if (!other.CompareTag("Player")) return;
    //     if (hasInteracted) return;

    //     // ✅ รองรับ Mouse Left Click หรือ Gamepad Button South (A / X)
    //     bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    //     bool gamepadPressed = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;

    //     if (mousePressed || gamepadPressed)
    //     {
    //         if (PicturePuzzleUI.Instance != null && PicturePuzzleUI.Instance.IsPuzzleCompleted()) return;
    //         if (QuestManager.Instance != null && QuestManager.Instance.IsSearchingForMysteryPhoto())
    //         {
    //             PicturePuzzleUI.Instance?.OpenPuzzle();
    //         }
    //     }
    // }

    public void Interact()
    {
        if (hasInteracted) return;
        hasInteracted = true;

        if (PicturePuzzleUI.Instance != null && PicturePuzzleUI.Instance.IsPuzzleCompleted()) return;

        if (QuestManager.Instance != null && QuestManager.Instance.IsSearchingForMysteryPhoto())
        {
            PicturePuzzleUI.Instance?.OpenPuzzle();
        }
        else
        {
            DialogueManager.Instance?.Show("ฉันยังไม่รู้ว่าภาพเหล่านี้คืออะไร...", 2f);
        }
    }

    public bool CanBeInteracted()
    {
        return !PicturePuzzleUI.Instance.IsPuzzleCompleted(); // ✅ ถ้ายังไม่เสร็จให้ interact ได้
    }

    public void SetHighlighted(bool highlighted)
    {
        var glow = GetComponent<SelectionGlow>();
        if (glow != null)
        {
            glow.SetGlowEnabled(highlighted);
        }
    }

    public void ResetInteract()
    {
        hasInteracted = false;
    }
}