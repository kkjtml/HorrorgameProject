using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinetDoorController : MonoBehaviour
{
    public Transform doorTransform;
    public Vector3 openRotationOffset = new Vector3(0, -100, 0);
    public float openSpeed = 2f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Quaternion targetRotation;

    private bool isOpen = false;

    void Start()
    {
        closedRotation = doorTransform.localRotation;
        openRotation = Quaternion.Euler(closedRotation.eulerAngles + openRotationOffset);
        targetRotation = closedRotation;
    }

    void Update()
    {
        doorTransform.localRotation = Quaternion.Slerp(
            doorTransform.localRotation, targetRotation, Time.deltaTime * openSpeed);
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        targetRotation = isOpen ? openRotation : closedRotation;
        Debug.Log(isOpen ? "🟤 Cabinet opened" : "🟥 Cabinet closed");
    }

    public void Open()
    {
        isOpen = true;
        targetRotation = openRotation;
    }

    public void Close()
    {
        isOpen = false;
        targetRotation = closedRotation;
    }

    public bool IsOpen() => isOpen;
}