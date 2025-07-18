using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HighlightCondition
{
    Always,
    AfterClue1,
    AfterClue2,
    AfterLanternDone,
    AfterClue3,
    Never
}

[RequireComponent(typeof(MeshRenderer))]
public class HighlightController : MonoBehaviour
{
    public Material highlightMaterial;
    public HighlightCondition condition = HighlightCondition.Never;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;
    private bool isHighlighted = false;
    public bool overrideBySelection = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        CheckAndUpdateHighlight();
    }

    void Update()
    {
        if (!overrideBySelection)
            CheckAndUpdateHighlight();
    }

    void CheckAndUpdateHighlight()
    {
        bool shouldHighlight = false;

        switch (condition)
        {
            case HighlightCondition.Always:
                shouldHighlight = true;
                break;
            case HighlightCondition.AfterClue1:
                shouldHighlight = QuestManager.Instance != null && QuestManager.Instance.HasSeenClue1();
                break;
            case HighlightCondition.AfterClue2:
                shouldHighlight = QuestManager.Instance != null && QuestManager.Instance.HasSeenClue2();
                break;
            case HighlightCondition.AfterLanternDone:
                shouldHighlight = QuestManager.Instance != null && QuestManager.Instance.HasFinishedLanternQuest();
                break;
            case HighlightCondition.AfterClue3:
                shouldHighlight = QuestManager.Instance != null && QuestManager.Instance.HasSeenClue3();
                break;
        }

        if (shouldHighlight != isHighlighted)
        {
            isHighlighted = shouldHighlight;
            meshRenderer.material = isHighlighted ? highlightMaterial : originalMaterial;
        }
    }

    public void EnableHighlight()
    {
        isHighlighted = true;
        if (meshRenderer != null && highlightMaterial != null)
            meshRenderer.material = highlightMaterial;
    }

    public void DisableHighlight()
    {
        isHighlighted = false;
        if (meshRenderer != null && originalMaterial != null)
            meshRenderer.material = originalMaterial;
    }
}
