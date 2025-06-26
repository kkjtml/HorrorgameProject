using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public TextMeshProUGUI questText;
    public GameObject jumpscareObject;
    public int totalLanterns = 3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateQuestUI(0);
        if (jumpscareObject != null)
            jumpscareObject.SetActive(false);
    }

    public void OnLanternLit(int index)
    {
        if (index + 1 < totalLanterns)
        {
            UpdateQuestUI(index + 1);
        }
        else
        {
            TriggerFinalEvent();
        }
    }

    void UpdateQuestUI(int nextIndex)
    {
        if (questText != null)
        {
            questText.text = "จุดไฟตะเกียงอันที่ " + (nextIndex + 1).ToString();
        }
    }

    void TriggerFinalEvent()
    {
        if (questText != null)
        {
            questText.text = "คุณจุดไฟครบแล้ว...";
        }

        if (jumpscareObject != null)
        {
            // Delay 1.5 วิแล้วโผล่
            Invoke("ShowJumpscare", 1.5f);
        }
    }

    void ShowJumpscare()
    {
        jumpscareObject.SetActive(true);
        // เสียงกรี๊ด? เล่น animation? กล้อง shake? ใส่เพิ่มได้
    }
}