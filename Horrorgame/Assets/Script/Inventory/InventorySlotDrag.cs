using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Transform originalParent;
    private Vector2 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        transform.SetParent(canvas.transform); // ลอยอิสระ
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject target = eventData.pointerEnter;

        if (target != null && target.GetComponent<InventorySlotDrop>() != null)
        {
            transform.SetParent(target.transform);
            rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }

        canvasGroup.blocksRaycasts = true;
    }
}

