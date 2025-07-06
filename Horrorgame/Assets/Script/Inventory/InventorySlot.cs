using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    private InspectableItemData itemData;

    public void Setup(InspectableItemData data)
    {
        itemData = data;
        if (iconImage != null)
        {
            iconImage.sprite = data.icon;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (itemData != null && itemData.inspectPrefab != null)
            {
                InspectManager.Instance.StartInspect(itemData.inspectPrefab);
            }
        }
    }
}