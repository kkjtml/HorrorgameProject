using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClueNoteManager : MonoBehaviour
{
    public static ClueNoteManager Instance;

    public List<GameObject> clueUIPanels;
    public GameObject clueObjectInWorld;
    private int currentClueIndex = -1;
    private bool isShowing = false;

    private StarterAssets.ThirdPersonController player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        foreach (var panel in clueUIPanels)
        {
            if (panel != null) panel.SetActive(false);
        }

        player = FindObjectOfType<StarterAssets.ThirdPersonController>();

        if (!QuestManager.Instance.HasFinishedLanternQuest())
        {
            if (clueObjectInWorld != null)
                clueObjectInWorld.SetActive(true);
        }
    }

    void Update()
    {
        if (isShowing && Input.GetMouseButtonDown(1))
        {
            CloseClue();
        }
    }

    public void ShowClue(int index)
    {
        if (isShowing || index < 0 || index >= clueUIPanels.Count) return;

        currentClueIndex = index;
        isShowing = true;
        StartCoroutine(ShowClueDelayed(index));
    }

    private IEnumerator ShowClueDelayed(int index)
    {
        yield return null;

        clueUIPanels[index].SetActive(true);

        if (index == 0)
        {
            DialogueManager.Instance?.Show("ฉันต้องจุดตะเกียงเรียงทวนเข็มนาฬิกา", 3f);
            if (QuestManager.Instance != null && QuestManager.Instance.HasFinishedLanternQuest())
            {
                if (clueObjectInWorld != null)
                    clueObjectInWorld.SetActive(false);
            }
        }

        if (player != null) player.enabled = false;
    }

    void CloseClue()
    {
        int closedIndex = currentClueIndex;
        if (currentClueIndex >= 0 && currentClueIndex < clueUIPanels.Count)
            clueUIPanels[currentClueIndex].SetActive(false);

        isShowing = false;
        currentClueIndex = -1;

        QuestManager.Instance?.OnClueNoteClosed(closedIndex);

        if (player != null) player.enabled = true;
    }

    public bool IsClueShowing() => isShowing;
}