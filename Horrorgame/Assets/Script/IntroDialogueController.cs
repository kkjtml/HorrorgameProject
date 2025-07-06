using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogueController : MonoBehaviour
{
    private StarterAssets.ThirdPersonController player;

    void Start()
    {
        player = FindObjectOfType<StarterAssets.ThirdPersonController>();

        if (player != null)
        {
            player.enabled = false; 
        }

        StartCoroutine(PlayIntroDialogue());
    }

    private IEnumerator PlayIntroDialogue()
    {
        yield return null;
        
        DialogueManager.Instance?.Show("ที่นี่ที่ไหน...", 2f);
        DialogueManager.Instance?.Queue("ฉันต้องหาทางออกจากบ้านนี้", 2.5f);

        // ⏳ รอรวมทั้งหมดก่อนปลดล็อก
        yield return new WaitForSeconds(2f + 2.5f + 0.25f);

        if (player != null)
        {
            player.enabled = true; // ✅ เปิดควบคุม
        }

        DialogueManager.Instance.Show("ก่อนอื่น...ฉันต้องสำรวจบ้านนี้ก่อน", 3f);
    }
}