using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InspectManager : MonoBehaviour
{
    public static InspectManager Instance;

    public GameObject blackBackground; // Drag UI Panel สีดำใน Inspector

    [SerializeField] private float zoomSpeed = 1.0f;
    [SerializeField] private float minZoomDistance = 1.0f;
    [SerializeField] private float maxZoomDistance = 1.5f;
    private float currentZoom = 1.0f; // เริ่มต้นที่ขนาดปกติ

    private GameObject spotlightObj;
    private Light spotlight;

    private GameObject currentItem;
    private bool isInspecting = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (isInspecting && currentItem != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                float rotX = Mouse.current.delta.x.ReadValue();
                float rotY = Mouse.current.delta.y.ReadValue();

                currentItem.transform.Rotate(Vector3.up, -rotX, Space.World);
                currentItem.transform.Rotate(Camera.main.transform.right, rotY, Space.World);
            }

            // 🔍 ซูมด้วย Scroll Wheel (เปลี่ยนเป็นเลื่อนเข้า-ออก)
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                currentZoom -= scroll * zoomSpeed * 0.01f;
                currentZoom = Mathf.Clamp(currentZoom, minZoomDistance, maxZoomDistance);

                Vector3 zoomPos = Camera.main.transform.position + Camera.main.transform.forward * currentZoom;
                currentItem.transform.position = zoomPos;
            }

            // ✅ คลิกขวาออกจาก Inspect
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                EndInspect();
            }
        }
    }

    public void StartInspect(GameObject prefabToInspect)
    {
        if (isInspecting || prefabToInspect == null) return;

        currentZoom = 1.0f;

        // ✅ วาง object หน้า camera
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camPosition = Camera.main.transform.position;
        Vector3 spawnPos = camPosition + camForward * 1.0f;
        Quaternion lookAtCam = Quaternion.LookRotation(-camForward);

        currentItem = Instantiate(prefabToInspect, spawnPos, lookAtCam);
        currentItem.transform.localScale *= 1.5f; // ขยายวัตถุ 2.5 เท่า

        // ❌ ป้องกัน object ชนกับฉาก
        Collider col = currentItem.GetComponentInChildren<Collider>();
        if (col != null) col.enabled = false;

        // ✅ LightGroup สำหรับจัดแสงรวม
        spotlightObj = new GameObject("LightGroup");
        spotlightObj.transform.SetParent(Camera.main.transform);

        Vector3 basePos = currentItem.transform.position;

        // 🔆 1. Key Light (ด้านหน้า)
        GameObject keyLight = new GameObject("KeyLight");
        keyLight.transform.SetParent(spotlightObj.transform);
        keyLight.transform.position = basePos + Camera.main.transform.forward * 0.6f + Vector3.up * 0.1f;
        Light light1 = keyLight.AddComponent<Light>();
        light1.type = LightType.Point;
        light1.intensity = 0.4f;
        light1.range = 1.2f;
        light1.color = new Color(1f, 0.95f, 0.9f);
        light1.shadows = LightShadows.None;

        // 💡 2. Fill Light (ข้างขวา)
        GameObject fillLight = new GameObject("FillLight");
        fillLight.transform.SetParent(spotlightObj.transform);
        fillLight.transform.position = basePos + Camera.main.transform.right * 0.3f + Vector3.up * 0.05f;
        Light light2 = fillLight.AddComponent<Light>();
        light2.type = LightType.Point;
        light2.intensity = 0.25f;
        light2.range = 1.0f;
        light2.color = new Color(0.8f, 0.85f, 1f);
        light2.shadows = LightShadows.None;

        // ✨ 3. Rim Light (ด้านหลัง)
        GameObject rimLight = new GameObject("RimLight");
        rimLight.transform.SetParent(spotlightObj.transform);
        rimLight.transform.position = basePos - Camera.main.transform.forward * 0.4f + Vector3.up * 0.1f;
        Light light3 = rimLight.AddComponent<Light>();
        light3.type = LightType.Point;
        light3.intensity = 0.3f;
        light3.range = 1.0f;
        light3.color = Color.white;
        light3.shadows = LightShadows.None;

        blackBackground.SetActive(true);
        isInspecting = true;

        // 🔒 ปิดการเคลื่อนไหว
        StarterAssets.ThirdPersonController player = FindObjectOfType<StarterAssets.ThirdPersonController>();
        if (player != null) player.enabled = false;
    }

    public void EndInspect()
    {
        if (currentItem != null) Destroy(currentItem);

        if (spotlightObj != null) Destroy(spotlightObj);

        blackBackground.SetActive(false);
        isInspecting = false;

        StarterAssets.ThirdPersonController player = FindObjectOfType<StarterAssets.ThirdPersonController>();
        if (player != null) player.enabled = true;
    }

    public bool IsInspecting() => isInspecting;
}