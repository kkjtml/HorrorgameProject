using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GlowCondition
{
    Always,
    AfterClue1,
    AfterClue2,
    AfterLanternDone,
    AfterClue3,
    Never
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Collider))]
public class SelectionGlow : MonoBehaviour
{
    [Header("Glow Material (for outline effect)")]
    public Material glowMaterial;

    [Header("OPEN")]
    public GlowCondition glowEnableCondition = GlowCondition.Always;

    [Header("CLOSE")]
    public GlowCondition glowDisableCondition = GlowCondition.Never;

    private GameObject glowObject;
    private GameObject player;

    private bool forceDisable = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        CreateGlowObject();
    }

    void CreateGlowObject()
    {
        // สร้างวัตถุเงาที่จะขยายและเรืองแสง
        glowObject = new GameObject("GlowOutline");
        glowObject.transform.SetParent(transform);
        glowObject.transform.localPosition = Vector3.zero;
        glowObject.transform.localRotation = Quaternion.identity;
        glowObject.transform.localScale = Vector3.one * 1.05f; // ขยายเล็กน้อย

        MeshFilter mf = glowObject.AddComponent<MeshFilter>();
        mf.mesh = GetComponent<MeshFilter>().mesh;

        MeshRenderer mr = glowObject.AddComponent<MeshRenderer>();
        mr.material = glowMaterial;

        glowObject.SetActive(false); // ปิดไว้ก่อน
    }

    bool ShouldAllowGlow()
    {
        return IsGlowEnableConditionMet() && !IsGlowDisableConditionMet();
    }

    bool IsGlowEnableConditionMet()
    {
        return CheckCondition(glowEnableCondition);
    }

    bool IsGlowDisableConditionMet()
    {
        return CheckCondition(glowDisableCondition);
    }

    bool CheckCondition(GlowCondition condition)
    {
        switch (condition)
        {
            case GlowCondition.Always: return true;
            case GlowCondition.AfterClue1: return QuestManager.Instance != null && QuestManager.Instance.HasSeenClue1();
            case GlowCondition.AfterClue2: return QuestManager.Instance != null && QuestManager.Instance.HasSeenClue2();
            case GlowCondition.AfterLanternDone: return QuestManager.Instance != null && QuestManager.Instance.HasFinishedLanternQuest();
            case GlowCondition.AfterClue3: return QuestManager.Instance != null && QuestManager.Instance.HasSeenClue3();
            case GlowCondition.Never: return false;
        }
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (forceDisable) return;

        if (other.CompareTag("Player") && glowObject != null && ShouldAllowGlow())
        {
            glowObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (glowObject != null)
        {
            glowObject.SetActive(false);
        }
    }

    public void SetGlowEnabled(bool enabled)
    {
        if (glowObject == null)
            CreateGlowObject();

        glowObject.SetActive(enabled && ShouldAllowGlow());
    }


    public void ForceDisableGlow(bool disable)
    {
        if (glowObject == null)
            CreateGlowObject();

        forceDisable = disable;
        glowObject.SetActive(!forceDisable && ShouldAllowGlow());
    }
}