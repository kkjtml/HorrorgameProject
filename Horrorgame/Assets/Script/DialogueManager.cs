using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextMeshProUGUI dialogueText;
    public CanvasGroup dialogueCanvasGroup;
    public float showDuration = 3f;

    private Queue<DialogueEntry> dialogueQueue = new Queue<DialogueEntry>();

    private bool isShowing = false;

    private Coroutine currentCoroutine = null;


    private class DialogueEntry
    {
        public string message;
        public float duration;
        public DialogueEntry(string msg, float dur)
        {
            message = msg;
            duration = dur;
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 0;
            dialogueCanvasGroup.gameObject.SetActive(false);
        }
    }

    public void Show(string message, float duration = -1f)
    {
        // แบบแสดงเดี่ยว → เคลียร์ทุกอย่างก่อน
        ClearCurrent();

        dialogueQueue.Enqueue(new DialogueEntry(message, duration > 0 ? duration : showDuration));
        currentCoroutine = StartCoroutine(ProcessQueue());
    }

    public void Queue(string message, float duration = -1f)
    {
        dialogueQueue.Enqueue(new DialogueEntry(message, duration > 0 ? duration : showDuration));

        if (!isShowing)
        {
            currentCoroutine = StartCoroutine(ProcessQueue());
        }
    }

    private void ClearCurrent()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        dialogueQueue.Clear();
        dialogueText.text = "";
        dialogueCanvasGroup.alpha = 0;
        dialogueCanvasGroup.gameObject.SetActive(false);
        isShowing = false;
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;

        while (dialogueQueue.Count > 0)
        {
            DialogueEntry entry = dialogueQueue.Dequeue();

            dialogueText.text = entry.message;
            dialogueCanvasGroup.gameObject.SetActive(true);
            dialogueCanvasGroup.alpha = 1;

            yield return new WaitForSeconds(entry.duration);

            // ไม่ fade – ให้ค้างไว้ แล้วแสดงข้อความถัดไปทับทันที
            yield return new WaitForSeconds(0.25f); // wait a short gap
        }

        dialogueCanvasGroup.alpha = 0;
        dialogueCanvasGroup.gameObject.SetActive(false);
        isShowing = false;
    }

}