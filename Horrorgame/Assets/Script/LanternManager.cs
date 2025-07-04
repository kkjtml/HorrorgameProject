using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternManager : MonoBehaviour
{
    public static LanternManager Instance;

    public int nextLanternIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool CanLightLantern(int index)
    {
        if (!ClueNoteManager.Instance || ClueNoteManager.Instance.IsClueShowing())
            return false;

        return index == nextLanternIndex;
    }

    public void OnLanternLit(int index)
    {
        if (index == nextLanternIndex)
        {
            nextLanternIndex++;
            Debug.Log("✅ Lantern " + index + " lit successfully");

            QuestManager.Instance?.OnLanternLit(index);
        }
        else
        {
            Debug.Log("❌ Wrong order! Lantern " + index + " is not allowed yet");
        }
    }
}