using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public TextMeshProUGUI mainQuestText;
    public TextMeshProUGUI subQuestText;

    public int totalLanterns = 3;
    private int currentLanternIndex = 0;
    private bool hasSeenClueNote = false;
    private bool clue2Triggered = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mainQuestText.text = "หาทางออกจากบ้าน";
        subQuestText.text = "สำรวจบ้าน";
    }

    public void OnClueNoteSeen()
    {
        hasSeenClueNote = true;
        currentLanternIndex = 0;
        subQuestText.text = "จุดตะเกียงดวงที่ 1";
    }

    public bool HasSeenClueNote() => hasSeenClueNote;

    public void LightLantern(int index)
    {
        if (!hasSeenClueNote) return;

        if (index + 1 < totalLanterns)
        {
            currentLanternIndex = index + 1;
            subQuestText.text = "จุดตะเกียงดวงที่ " + (index + 2);
        }
        else
        {
            LightLanternFinish();
        }
    }

    void LightLanternFinish()
    {
        subQuestText.text = "คุณจุดไฟครบแล้ว...";

        if (!clue2Triggered)
        {
            clue2Triggered = true;

            // ✅ ลบโน้ตแผ่นแรกในฉากออกไปเลย
            if (ClueNoteManager.Instance != null && ClueNoteManager.Instance.clueObjectInWorld != null)
            {
                ClueNoteManager.Instance.clueObjectInWorld.SetActive(false);
                Debug.Log("🗑️ ลบ clueObjectInWorld แล้วหลังจุดตะเกียงครบ");
            }

            Invoke(nameof(ShowSecondClue), 1.5f);
        }
    }

    public bool HasFinishedLanternQuest()
    {
        return clue2Triggered;
    }

    void ShowSecondClue()
    {
        ClueNoteManager.Instance?.ShowClue(1);
    }

    public void OnClueNoteClosed(int clueIndex)
    {
        if (clueIndex == 0 && !hasSeenClueNote)
        {
            OnClueNoteSeen();
        }
        else if (clueIndex == 1)
        {
            subQuestText.text = "ตามหารูปภาพปริศนา";
        }
    }

    public bool IsSearchingForMysteryPhoto()
    {
        return subQuestText != null && subQuestText.text == "ตามหารูปภาพปริศนา";
    }


}