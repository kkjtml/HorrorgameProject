using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectUIManager : MonoBehaviour
{
    public static InspectUIManager Instance;

    [Header("UI")]
    public GameObject inspectUIRoot;
    public Button keepButton;
    public Button discardButton;

    private InspectableItemData currentItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inspectUIRoot.SetActive(false);

        keepButton.onClick.AddListener(OnKeepItem);
        discardButton.onClick.AddListener(OnDiscardItem);
    }

    public void ShowInspectOptions(InspectableItemData item)
    {
        currentItem = item;

        // แสดง UI ปุ่ม ถ้าไม่ใช่กระดาษคำใบ้
        inspectUIRoot.SetActive(!item.isClueNote);
    }

    public void HideInspectOptions()
    {
        inspectUIRoot.SetActive(false);
        currentItem = null;
    }

    void OnKeepItem()
    {
        if (currentItem != null)
        {
            InventoryManager.Instance.AddItemToInventory(currentItem);
        }

        InspectManager.Instance.EndInspect();
        HideInspectOptions();
    }

    void OnDiscardItem()
    {
        InspectManager.Instance.EndInspect();
        HideInspectOptions();
    }
}