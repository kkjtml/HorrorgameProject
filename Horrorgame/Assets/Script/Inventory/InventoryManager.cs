using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI Panels")]
    public GameObject inventoryUI;
    public GameObject inspectUI;

    [Header("Item Containers")]
    public Transform itemSlotParent;     // ช่องเก็บของทั่วไป
    public Transform storyNoteParent;    // กระดาษโน้ต
    public Transform clueNoteParent;     // กระดาษคำใบ้

    [Header("Prefabs")]
    public GameObject itemSlotPrefab;
    public GameObject noteSlotPrefab;

    private bool isInventoryOpen = false;

    private List<InventorySlot> itemSlots = new List<InventorySlot>();
    private List<InventorySlot> storyNoteSlots = new List<InventorySlot>();
    private List<InventorySlot> clueNoteSlots = new List<InventorySlot>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        inventoryUI.SetActive(false);
        inspectUI.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        var player = FindObjectOfType<StarterAssets.ThirdPersonController>();
        if (player != null)
        {
            player.enabled = !isInventoryOpen;
            Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isInventoryOpen;
        }
    }

    public void AddItemToInventory(InspectableItemData itemData)
    {
        GameObject slotGO = Instantiate(itemSlotPrefab, itemSlotParent);
        InventorySlot slot = slotGO.GetComponent<InventorySlot>();
        slot.Setup(itemData);
        itemSlots.Add(slot);
    }

    public void AddNote(InspectableItemData noteData, bool isClue)
    {
        GameObject slotGO = Instantiate(noteSlotPrefab, isClue ? clueNoteParent : storyNoteParent);
        InventorySlot slot = slotGO.GetComponent<InventorySlot>();
        slot.Setup(noteData);

        if (isClue) clueNoteSlots.Add(slot);
        else storyNoteSlots.Add(slot);
    }
}