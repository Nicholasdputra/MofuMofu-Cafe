using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDialogueSO", menuName = "ScriptableObjects/CharacterDialogueSO", order = 1)]
public class CharacterDialogueSO : ScriptableObject
{
    public Dialogues[] dialogues;
}

[System.Serializable]
public class Dialogues
{
    public string npcName;
    public string[] orderingIcedCoffee;
    public string[] orderingWarmCoffee;
    public string[] orderingIcedMatcha;
    public string[] orderingWarmMatcha;
    public string[] orderingIcedChocolate;
    public string[] orderingWarmChocolate;
    public string repeatedDialogue;
}