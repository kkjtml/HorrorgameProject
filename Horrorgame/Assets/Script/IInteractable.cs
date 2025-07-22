using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
    void SetHighlighted(bool highlighted); // 🆕 เพิ่มไว้ให้ Raycast สั่งเปิด/ปิด glow
    bool CanBeInteracted();
}
