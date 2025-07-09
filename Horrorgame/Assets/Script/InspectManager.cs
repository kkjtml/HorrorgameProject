using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InspectCondition
{
    Always,             // ดูได้ตลอดเวลา
    AfterClue1,         // ต้องเห็น clue 1 (จุดตะเกียง)
    AfterClue2,         // ต้องเห็น clue 2 (เรียงภาพ)
    AfterLanternDone,    // ต้องจุดตะเกียงครบ
    AfterClue3 // ต้องเห็น clue 3 (ตามหากุญแจ)
}

public class InspectManager : MonoBehaviour
{
    public static InspectManager Instance;

    public GameObject blackBackground;

    [SerializeField] private float zoomSpeed = 1.0f;
    [SerializeField] private float minZoomDistance = 1.0f;
    [SerializeField] private float maxZoomDistance = 1.5f;
    private float currentZoom = 1.0f; // เริ่มต้นที่ขนาดปกติ

    private GameObject spotlightObj;
    private GameObject originalItem;  // ✅ เก็บของจริงที่กด
    private GameObject currentItem;
    private bool isInspecting = false;
    private GameObject keyItemInWorld = null;

    private StarterAssets.ThirdPersonController player;
    private GameObject persistentSpotlightGroup = null;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (blackBackground != null)
            blackBackground.SetActive(false);

        // ✅ สร้างกลุ่มแสงครั้งเดียว
        persistentSpotlightGroup = new GameObject("PersistentLightGroup");
        persistentSpotlightGroup.SetActive(false); // ปิดไว้ก่อน
        persistentSpotlightGroup.transform.SetParent(Camera.main.transform);

        CreateSpotLights(persistentSpotlightGroup.transform); // ใช้เดิมได้เลย

        player = FindObjectOfType<StarterAssets.ThirdPersonController>();
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

            // // 🔍 ซูมด้วย Scroll Wheel (เปลี่ยนเป็นเลื่อนเข้า-ออก)
            // float scroll = Mouse.current.scroll.ReadValue().y;
            // if (Mathf.Abs(scroll) > 0.01f)
            // {
            //     currentZoom -= scroll * zoomSpeed * 0.01f;
            //     currentZoom = Mathf.Clamp(currentZoom, minZoomDistance, maxZoomDistance);

            //     UpdateZoomPosition();
            // }

            // ✅ คลิกขวาออกจาก Inspect
            if (Input.GetMouseButtonDown(1))
            {
                EndInspect();
            }
        }
    }

    public void StartInspect(GameObject prefabToInspect, GameObject originalSource = null)
    {
        if (isInspecting || prefabToInspect == null) return;

        // ✅ บันทึกของจริงใน scene เพื่อซ่อนตอน inspect
        if (originalSource != null && originalSource.scene.IsValid())
        {
            originalItem = originalSource;
            originalItem.SetActive(false);
        }

        // ✅ วาง object หน้า camera
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camPosition = Camera.main.transform.position;
        currentZoom = maxZoomDistance;
        Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 0.1f;

        Quaternion lookAtCam = Quaternion.LookRotation(-camForward);

        currentItem = Instantiate(prefabToInspect, spawnPos, lookAtCam);
        UpdateZoomPosition();

        // ❌ ป้องกัน object ชนกับฉาก
        Collider col = currentItem.GetComponentInChildren<Collider>();
        if (col != null) col.enabled = false;

        // ✅ เปิดแสงที่เตรียมไว้
        persistentSpotlightGroup.SetActive(true);

        blackBackground.SetActive(true);
        isInspecting = true;

        if (player != null) player.enabled = false;
    }

    private void CreateSpotLights(Transform parent)
    {
        Vector3 basePos = Camera.main.transform.position + Camera.main.transform.forward * 0.3f;

        GameObject key = new GameObject("KeyLight");
        key.transform.SetParent(parent);
        key.transform.position = basePos + Camera.main.transform.forward * 0.6f + Vector3.up * 0.1f;
        var light1 = key.AddComponent<Light>();
        light1.intensity = 0.45f;
        light1.range = 1.2f;
        light1.color = new Color(1f, 0.97f, 0.92f);
        light1.shadows = LightShadows.Soft;
        light1.shadowStrength = 0.2f;

        GameObject fill = new GameObject("FillLight");
        fill.transform.SetParent(parent);
        fill.transform.position = basePos + Camera.main.transform.right * 0.3f + Vector3.up * 0.05f;
        var light2 = fill.AddComponent<Light>();
        light2.intensity = 0.2f;
        light2.range = 1.0f;
        light2.color = new Color(0.8f, 0.85f, 1f);
        light2.shadows = LightShadows.None;

        GameObject rim = new GameObject("RimLight");
        rim.transform.SetParent(parent);
        rim.transform.position = basePos - Camera.main.transform.forward * 0.4f + Vector3.up * 0.1f;
        var light3 = rim.AddComponent<Light>();
        light3.intensity = 0.25f;
        light3.range = 0.8f;
        light3.color = Color.white;
        light3.shadows = LightShadows.None;
    }

    public void EndInspect()
    {
        if (currentItem != null) Destroy(currentItem);

        // ✅ ปิด spotlight เดิมแทนการ Destroy
        if (persistentSpotlightGroup != null) persistentSpotlightGroup.SetActive(false);

        // ✅ เปิดของจริงกลับมา
        if (originalItem != null)
        {
            originalItem.SetActive(true);
            originalItem = null;
        }

        if (keyItemInWorld != null)
        {
            DialogueManager.Instance?.Show("เก็บกุญแจแล้ว นำไปปลดล็อคประตู", 3f);
            QuestManager.Instance?.OnFoundKey();

            Destroy(keyItemInWorld); // ❌ ทำให้กุญแจในฉากหาย
            keyItemInWorld = null;
        }

        blackBackground.SetActive(false);
        isInspecting = false;

        if (player != null) player.enabled = true;
    }

    public bool IsInspecting() => isInspecting;

    private void UpdateZoomPosition()
    {
        if (currentItem == null) return;

        Vector3 zoomPos = Camera.main.transform.position + Camera.main.transform.forward * currentZoom;
        currentItem.transform.position = zoomPos;
    }

    public void SetKeyItem(GameObject obj)
    {
        keyItemInWorld = obj;
    }
}