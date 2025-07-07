using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/PictureData")]
public class PictureData : ScriptableObject
{
    public PictureType type;
    public Sprite sprite;
    public float correctRotation = 0f; // เช่น 0°, 90°, 180°, 270°
}
