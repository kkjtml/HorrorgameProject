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
    }

    void Update()
    {
        if (isShowing && Mouse.current.rightButton.wasPressedThisFrame)
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
        if (clueObjectInWorld != null)
            clueObjectInWorld.SetActive(false);

        var player = FindObjectOfType<StarterAssets.ThirdPersonController>();
        if (player != null) player.enabled = false;
    }

    void CloseClue()
    {
        int closedIndex = currentClueIndex;
        if (currentClueIndex >= 0 && currentClueIndex < clueUIPanels.Count)
            clueUIPanels[currentClueIndex].SetActive(false);

        isShowing = false;
        currentClueIndex = -1;

        if (closedIndex == 0 && clueObjectInWorld != null)
            clueObjectInWorld.SetActive(false);

        QuestManager.Instance?.OnClueNoteClosed(closedIndex);

        var player = FindObjectOfType<StarterAssets.ThirdPersonController>();
        if (player != null) player.enabled = true;
    }

    public bool IsClueShowing() => isShowing;
}