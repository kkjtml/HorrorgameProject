using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotDrop : MonoBehaviour
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped != null)
        {
            dropped.transform.SetParent(transform);
            RectTransform rt = dropped.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
        }
    }
}