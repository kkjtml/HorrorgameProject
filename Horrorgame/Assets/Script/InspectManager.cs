using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InspectManager : MonoBehaviour
{
    public static InspectManager Instance;

    public GameObject inspectUI; // UI ที่ใช้ดูของ เช่น กล่องใส่ของ
    public GameObject objectToInspect; // Prefab ของของที่หยิบมาดู
    public Transform inspectSpawnPoint; // จุดที่วางของให้ดู

    [SerializeField] private float zoomSpeed = 2.0f;
    [SerializeField] private float minZoomDistance = 1f;  
    [SerializeField] private float maxZoomDistance = 1.5f;  
    private float currentZoom = 1.0f; // เริ่มต้นที่ขนาดปกติ

    private GameObject spotlightObj;
    private Light spotlight;

    private GameObject currentItem;
    private bool isInspecting = false;

    void Start()
    {
        if (inspectUI != null)
            inspectUI.SetActive(false);
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (isInspecting && currentItem != null)
        {
            // หมุนด้วยเมาส์
            float rotX = Mouse.current.delta.x.ReadValue();
            float rotY = Mouse.current.delta.y.ReadValue();
            currentItem.transform.Rotate(Vector3.up, -rotX, Space.World);
            currentItem.transform.Rotate(Vector3.right, rotY, Space.World);

            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                currentZoom += scroll * zoomSpeed * 0.01f; // ✅ Scroll up = Zoom in
                currentZoom = Mathf.Clamp(currentZoom, minZoomDistance, maxZoomDistance);

                currentItem.transform.localScale = Vector3.one * currentZoom;
            }

        }
    }

    public void StartInspect(GameObject prefabToInspect)
    {
        if (isInspecting || prefabToInspect == null || inspectSpawnPoint == null) return;

        currentZoom = 1.0f; // ระยะเริ่มต้น

        currentItem = Instantiate(prefabToInspect, inspectSpawnPoint.position, inspectSpawnPoint.rotation);
        currentItem.transform.localScale = Vector3.one * currentZoom;

        spotlightObj = new GameObject("InspectLight");
        spotlightObj.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.2f;
        spotlightObj.transform.rotation = Camera.main.transform.rotation;

        spotlight = spotlightObj.AddComponent<Light>();
        spotlight.type = LightType.Spot;
        spotlight.range = 3f;
        spotlight.intensity = 6f;
        spotlight.spotAngle = 45f;
        spotlight.shadows = LightShadows.Soft;

        spotlightObj.transform.SetParent(Camera.main.transform);

        inspectUI.SetActive(true);
        isInspecting = true;

        StarterAssets.ThirdPersonController player = FindObjectOfType<StarterAssets.ThirdPersonController>();
        if (player != null) player.enabled = false;
    }

    public void EndInspect()
    {
        if (currentItem != null) Destroy(currentItem);

        if (spotlightObj != null) Destroy(spotlightObj);

        inspectUI.SetActive(false);
        isInspecting = false;

        StarterAssets.ThirdPersonController player = FindObjectOfType<StarterAssets.ThirdPersonController>();
        if (player != null) player.enabled = true;
    }

    public bool IsInspecting() => isInspecting;
}