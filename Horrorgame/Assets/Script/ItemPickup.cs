using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour
{
    public Transform itemHoldPoint; 
    private GameObject heldItem;
    private bool isHolding = false;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!isHolding)
            {
                TryPickup();
            }
            else
            {
                DropItem();
            }
        }

        if (isHolding)
        {
            RotateHeldItem();
        }
    }

    void TryPickup()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                heldItem = hit.collider.gameObject;
                heldItem.GetComponent<Rigidbody>().isKinematic = true;
                heldItem.transform.SetParent(itemHoldPoint);
                heldItem.transform.localPosition = Vector3.zero;
                heldItem.transform.localRotation = Quaternion.identity;
                isHolding = true;
            }
        }
    }

    void DropItem()
    {
        heldItem.transform.SetParent(null);
        heldItem.GetComponent<Rigidbody>().isKinematic = false;
        heldItem = null;
        isHolding = false;
    }

    void RotateHeldItem()
    {
        float rotX = Mouse.current.delta.ReadValue().x;
        float rotY = Mouse.current.delta.ReadValue().y;
        heldItem.transform.Rotate(Vector3.up, -rotX, Space.World);
        heldItem.transform.Rotate(Vector3.right, rotY, Space.World);
    }
}
