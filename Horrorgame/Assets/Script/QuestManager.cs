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
    public GameObject jumpscareObject;

    public int totalLanterns = 3;
    private int currentLanternIndex = 0;
    private bool hasSeenClueNote = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mainQuestText.text = "หาทางออกจากบ้าน";
        subQuestText.text = "สำรวจบ้าน";

        if (jumpscareObject != null)
            jumpscareObject.SetActive(false);
    }

    public void OnClueNoteSeen()
    {
        hasSeenClueNote = true;
        currentLanternIndex = 0;
        subQuestText.text = "จุดตะเกียงดวงที่ 1";
    }

    public bool HasSeenClueNote() => hasSeenClueNote;

    public void OnLanternLit(int index)
    {
        if (!hasSeenClueNote) return;

        if (index + 1 < totalLanterns)
        {
            currentLanternIndex = index + 1;
            subQuestText.text = "จุดตะเกียงดวงที่ " + (index + 2);
        }
        else
        {
            TriggerFinalEvent();
        }
    }

    void TriggerFinalEvent()
    {
        subQuestText.text = "คุณจุดไฟครบแล้ว...";
        if (jumpscareObject != null)
        {
            Invoke("ShowJumpscare", 1.5f);
        }
    }

    void ShowJumpscare()
    {
        jumpscareObject.SetActive(true);
    }
}