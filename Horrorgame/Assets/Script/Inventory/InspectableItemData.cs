using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Inspectable Item")]
public class InspectableItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public GameObject inspectPrefab;
    public bool isClueNote;
}
