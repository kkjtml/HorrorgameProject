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

    private bool hasSeenClue1 = false;
    private bool hasSeenClue2 = false;
    private bool hasSeenClue3 = false;

    public bool HasSeenClue1() => hasSeenClue1;
    public bool HasSeenClue2() => hasSeenClue2;
    public bool HasSeenClue3() => hasSeenClue3;

    private bool hasFoundKey = false;
    public bool HasFoundKey() => hasFoundKey;

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
        subQuestText.text = "จุดไฟครบแล้ว...";

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
        if (clueIndex == 0 && !hasSeenClue1)
        {
            hasSeenClue1 = true;
            OnClueNoteSeen(); // จุดตะเกียง
        }
        else if (clueIndex == 1 && !hasSeenClue2)
        {
            hasSeenClue2 = true;
            subQuestText.text = "ตามหารูปภาพปริศนา";
        }
        else if (clueIndex == 2 && !hasSeenClue3)
        {
            hasSeenClue3 = true;
            subQuestText.text = "ตามหากุญแจ";
        }
    }

    public bool IsSearchingForMysteryPhoto()
    {
        return subQuestText != null && subQuestText.text == "ตามหารูปภาพปริศนา";
    }

    public void OnFoundKey()
    {
        if (!hasFoundKey)
        {
            hasFoundKey = true;
            subQuestText.text = "ปลดล็อคประตู";
            Debug.Log("🗝️ ได้กุญแจแล้ว → เปลี่ยนเควสเป็น 'ปลดล็อคประตู'");
        }
    }

    public void SetEscapeForestQuest()
    {
        mainQuestText.text = "หาทางกลับบ้าน";
        subQuestText.text = "สำรวจป่าทึบ";
    }


}