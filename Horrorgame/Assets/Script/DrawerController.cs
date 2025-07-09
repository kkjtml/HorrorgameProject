using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawerController : MonoBehaviour
{
    public Transform drawerTransform;
    public Vector3 openOffset = new Vector3(-0.6f, 0f, 0f);
    public float openSpeed = 2f;

    private bool isPlayerNearby = false;
    private bool isOpen = false;
    private Vector3 closedPos;
    private Vector3 openPos;
    private Vector3 targetPos;

    public bool requireClue3 = false;

    void Start()
    {
        closedPos = drawerTransform.localPosition;
        openPos = closedPos + openOffset;
        targetPos = closedPos;
    }

    void Update()
    {
        // // ให้กดปุ่ม K เพื่อเปิด/ปิดแบบ hard
        // if (Keyboard.current.kKey.wasPressedThisFrame)
        // {
        //     ToggleDrawer();
        //     Debug.Log($"👀 isPlayerNearby = {isPlayerNearby}, HasSeenClue3 = {QuestManager.Instance.HasSeenClue3()}");
        // }

        if (isPlayerNearby && Input.GetMouseButtonDown(0))
        {
            if (requireClue3 && !QuestManager.Instance.HasSeenClue3())
            {
                Debug.Log("🚫 ยังไม่ได้ดู Clue 3 → ห้ามเปิดลิ้นชัก");
                return;
            }

            ToggleDrawer();
        }

        drawerTransform.localPosition = Vector3.Lerp(drawerTransform.localPosition, targetPos, Time.deltaTime * openSpeed);
    }

    void ToggleDrawer()
    {
        isOpen = !isOpen;
        targetPos = isOpen ? openPos : closedPos;
        Debug.Log("🌀 Drawer toggled. New target = " + targetPos);
    }

    public void SetPlayerNearby(bool state)
    {
        isPlayerNearby = state;
    }
}