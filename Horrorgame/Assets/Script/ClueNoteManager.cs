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

    // void Update()
    // {
    //     if (isShowing && Input.GetMouseButtonDown(1))
    //     {
    //         CloseClue();
    //     }
    // }

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

        if (player != null)
        {
            player.enabled = false;
            Time.timeScale = 0f; // ✅ หยุดเวลา
        }

        // ✅ รอจนกว่าจะกดคลิกขวาหรือ B
        yield return StartCoroutine(WaitForCloseInput());

        CloseClue();

    }

    private IEnumerator WaitForCloseInput()
    {
        while (true)
        {
            bool rightClick = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
            bool gamepadB = Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame;

            if (rightClick || gamepadB)
                yield break;

            yield return null;
        }
    }

    void CloseClue()
    {
        int closedIndex = currentClueIndex;
        if (currentClueIndex >= 0 && currentClueIndex < clueUIPanels.Count)
            clueUIPanels[currentClueIndex].SetActive(false);

        isShowing = false;
        currentClueIndex = -1;

        QuestManager.Instance?.OnClueNoteClosed(closedIndex);

        if (player != null)
        {
            player.enabled = true;
            Time.timeScale = 1f; // ✅ กลับเวลาเป็นปกติ
        }

        // ✅ Reset trigger
        ResetNearbyTrigger();
    }

    private void ResetNearbyTrigger()
    {
        Collider[] nearby = Physics.OverlapSphere(player.transform.position, 1.5f);
        foreach (var col in nearby)
        {
            var clueTrigger = col.GetComponent<ClueNoteTrigger>();
            if (clueTrigger != null)
            {
                clueTrigger.ResetInteract();
            }
        }
    }

    public bool IsClueShowing() => isShowing;
}