using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public enum PictureType
{
    Skull,
    Crown,
    intestine
}
public class PicturePuzzleUI : MonoBehaviour
{
    public static PicturePuzzleUI Instance;

    public GameObject uiPanel;
    public Image[] pictureSlots;
    public PictureData[] pictureOptions;

    private int[] currentIndices = new int[3]; // เก็บลำดับรูปในแต่ละช่อง
    private float[] currentRotations = new float[3]; // เก็บ rotation ปัจจุบันแต่ละช่อง
    public PictureType[] correctOrder = { PictureType.Skull, PictureType.Crown, PictureType.intestine };

    private int selectedSlot = 0; // 🟦 ตำแหน่งช่องที่เลือกอยู่
    private float lastDpadX = 0f;

    private bool hasInitialized = false;

    private bool puzzleCompleted = false;
    public bool IsPuzzleCompleted() => puzzleCompleted;

    private StarterAssets.ThirdPersonController player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Update()
    {
        if (!uiPanel.activeInHierarchy) return;

        // --------------------------
        // 🎮 Gamepad Input (เช็ค null ทุกตัว)
        Gamepad gamepad = Gamepad.current;

        Vector2 dpad = gamepad != null ? gamepad.dpad.ReadValue() : Vector2.zero;
        bool gamepadLT = gamepad != null && gamepad.leftTrigger.wasPressedThisFrame;
        bool gamepadRT = gamepad != null && gamepad.rightTrigger.wasPressedThisFrame;
        bool gamepadX = gamepad != null && gamepad.buttonWest.wasPressedThisFrame;
        bool gamepadA = gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;
        bool gamepadB = gamepad != null && gamepad.buttonEast.wasPressedThisFrame;
        // --------------------------
        // ✅ Move slot selection (D-Pad Left/Right)
        float dpadX = dpad.x;

        if ((Keyboard.current?.qKey.wasPressedThisFrame ?? false) || (dpadX < -0.5f && lastDpadX >= -0.5f))
            selectedSlot = Mathf.Max(0, selectedSlot - 1);

        if ((Keyboard.current?.eKey.wasPressedThisFrame ?? false) || (dpadX > 0.5f && lastDpadX <= 0.5f))
            selectedSlot = Mathf.Min(2, selectedSlot + 1);

        lastDpadX = dpadX; // ❗ อย่าลืมอัปเดตทุกเฟรม
        // --------------------------
        // 🔄 Swap picture
        if ((Keyboard.current?.aKey.wasPressedThisFrame ?? false) || gamepadLT)
            SwapLeft(selectedSlot);

        if ((Keyboard.current?.dKey.wasPressedThisFrame ?? false) || gamepadRT)
            SwapRight(selectedSlot);

        // --------------------------
        // 🔃 Rotate
        if ((Keyboard.current?.rKey.wasPressedThisFrame ?? false) || gamepadX)
        {
            currentRotations[selectedSlot] = (currentRotations[selectedSlot] + 90f) % 360f;
            UpdateSlots();
        }

        // --------------------------
        // ✅ Check Answer
        if ((Keyboard.current?.spaceKey.wasPressedThisFrame ?? false) || gamepadA)
            CheckAnswer();

        // --------------------------
        // ❌ Exit puzzle
        bool mouseBack = Mouse.current?.rightButton.wasPressedThisFrame ?? false;
        if (mouseBack || gamepadB)
            ClosePuzzle();

        // --------------------------
        HighlightSelectedSlot();
    }

    private void HighlightSelectedSlot()
    {
        for (int i = 0; i < pictureSlots.Length; i++)
        {
            pictureSlots[i].color = (i == selectedSlot) ? Color.yellow : Color.white;
        }
    }

    public void OpenPuzzle()
    {
        uiPanel.SetActive(true);
        Time.timeScale = 0f;
        selectedSlot = 0;

        // 🔒 ปิดการควบคุมตัวละคร
        if (player == null)
            player = FindObjectOfType<StarterAssets.ThirdPersonController>();

        if (player != null)
            player.enabled = false;

        // ✅ สุ่มรูปแค่ครั้งแรกเท่านั้น
        if (!hasInitialized)
        {
            List<int> indices = new List<int> { 0, 1, 2 };
            for (int i = 0; i < 3; i++)
            {
                int rand = Random.Range(0, indices.Count);
                currentIndices[i] = indices[rand];
                indices.RemoveAt(rand);
            }

            // reset rotation ด้วย
            for (int i = 0; i < 3; i++)
                currentRotations[i] = 0f;

            hasInitialized = true;
        }

        UpdateSlots();
    }

    public void ClosePuzzle()
    {
        uiPanel.SetActive(false);
        Time.timeScale = 1f;

        // ✅ เปิดการควบคุมตัวละครอีกครั้ง
        if (player == null)
            player = FindObjectOfType<StarterAssets.ThirdPersonController>();

        if (player != null)
            player.enabled = true;

        ResetTrigger();
    }

    private void ResetTrigger()
    {
        if (player == null)
            player = FindObjectOfType<StarterAssets.ThirdPersonController>();

        if (player == null) return;

        Collider[] nearby = Physics.OverlapSphere(player.transform.position, 1.5f);
        foreach (var col in nearby)
        {
            var puzzleTrigger = col.GetComponent<PicturePuzzleTrigger>();
            if (puzzleTrigger != null)
            {
                puzzleTrigger.ResetInteract();
            }
        }
    }

    public void NextImage(int slotIndex)
    {
        currentIndices[slotIndex] = (currentIndices[slotIndex] + 1) % pictureOptions.Length;
        UpdateSlots();
    }

    private void UpdateSlots()
    {
        for (int i = 0; i < pictureSlots.Length; i++)
        {
            int optionIndex = currentIndices[i];
            pictureSlots[i].sprite = pictureOptions[optionIndex].sprite;

            // 🌀 หมุน UI ตาม currentRotations
            pictureSlots[i].rectTransform.localEulerAngles = new Vector3(0, 0, currentRotations[i]);
        }

        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"Slot {i} showing: {pictureOptions[currentIndices[i]].type} rot={currentRotations[i]}°");
        }
    }

    public void SwapLeft(int index)
    {
        if (index > 0)
        {
            int temp = currentIndices[index];
            currentIndices[index] = currentIndices[index - 1];
            currentIndices[index - 1] = temp;
            UpdateSlots();
        }
    }

    public void SwapRight(int index)
    {
        if (index < currentIndices.Length - 1)
        {
            int temp = currentIndices[index];
            currentIndices[index] = currentIndices[index + 1];
            currentIndices[index + 1] = temp;
            UpdateSlots();
        }
    }

    public void CheckAnswer()
    {
        bool correct = true;

        Debug.Log("✅ Correct order:");
        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"Correct[{i}] = {correctOrder[i]}");
        }

        for (int i = 0; i < 3; i++)
        {
            PictureData data = pictureOptions[currentIndices[i]];
            float expectedRotation = data.correctRotation;
            float actualRotation = currentRotations[i];

            bool rotationCorrect = Mathf.Approximately(expectedRotation, actualRotation);
            bool typeCorrect = data.type == correctOrder[i];

            if (!typeCorrect || !rotationCorrect)
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            DialogueManager.Instance.Show("เรียงถูกแล้ว...", 2f);
            ClosePuzzle();
            puzzleCompleted = true; // ✅ บันทึกสถานะว่าเสร็จแล้ว
            Invoke(nameof(ShowClue3AfterDelay), 2f); // ✅ ดีเลย์ 2 วิ
        }
        else
        {
            DialogueManager.Instance.Show("ยังเรียงไม่ถูก...", 2f);
        }
    }

    void ShowClue3AfterDelay()
    {
        ClueNoteManager.Instance?.ShowClue(2);
    }

}