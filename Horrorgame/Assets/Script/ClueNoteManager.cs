using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClueNoteManager : MonoBehaviour
{
    public static ClueNoteManager Instance;

    public GameObject clueUIPanel;   // UI แสดงกระดาษคำใบ้
    public GameObject clueObjectInWorld; // กระดาษในโลกจริง
    private bool isShowing = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (clueUIPanel != null)
            clueUIPanel.SetActive(false);
    }

    void Update()
    {
        if (isShowing && Mouse.current.rightButton.wasPressedThisFrame)
        {
            CloseClue();
        }
    }

    public void ShowClue()
    {
        if (isShowing) return;

        Debug.Log("📝 Showing Clue UI");

        isShowing = true;
        StartCoroutine(ShowClueDelayed());
    }

    private IEnumerator ShowClueDelayed()
    {
        yield return null; // รอ 1 frame ให้แน่ใจว่า UI โหลด

        clueUIPanel.SetActive(true);

        if (clueObjectInWorld != null)
            clueObjectInWorld.SetActive(false); // ซ่อนไว้ชั่วคราว

        // 🔒 ปิดการควบคุมผู้เล่นชั่วคราว
        StarterAssets.ThirdPersonController player = FindObjectOfType<StarterAssets.ThirdPersonController>();
        if (player != null) player.enabled = false;
    }

    void CloseClue()
    {
        clueUIPanel.SetActive(false);
        isShowing = false;

        // ✅ คืน object กระดาษ
        if (clueObjectInWorld != null)
            clueObjectInWorld.SetActive(true);

        // ✅ เริ่มเควสได้
        LanternManager.Instance.nextLanternIndex = 0;

        StarterAssets.ThirdPersonController player = FindObjectOfType<StarterAssets.ThirdPersonController>();
        if (player != null) player.enabled = true;
    }

    public bool IsClueShowing() => isShowing;
}