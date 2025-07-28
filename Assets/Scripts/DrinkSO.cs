using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drinks", menuName = "ScriptableObjects/DrinkObjects")]
public class DrinkSO : ScriptableObject
{
    public string itemName;
    public Sprite coldSprite;
    public Sprite hotSprite;
    public bool isCold;
}

