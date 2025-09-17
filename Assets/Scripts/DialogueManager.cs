using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public CharacterDialogueSO characterDialogueSO;
    CafeNPC targetNPC;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of DialogueManager detected: " + transform + " is a DialogueManager!");
        }
    }

    public void SetDialogue(CafeNPC npc)
    {
        targetNPC = npc;
        foreach (var dialogue in characterDialogueSO.dialogues)
        {
            if (dialogue.npcName == npc.npcName)
            {
                npc.npcOrderDialogue = DetermineOrder(targetNPC, dialogue);
            }
        }
    }

    private string DetermineOrder(CafeNPC npc, Dialogues dialogue)
    {
        string drinkName = npc.currentOrder.drinkName;
        bool isIced = npc.currentOrder.isIced;
        bool isImage = npc.isOrderFormImage;
        GameObject npcGameObject = npc.gameObject;
        switch (drinkName)
        {
            case "Coffee":
                if (isIced)
                {
                    if (isImage)
                    {
                        return dialogue.orderingIcedCoffee[0]; // Return the first string for image orders
                    }
                    SetTextHint(dialogue.orderingIcedCoffee[2], npcGameObject); // Set the text hint for iced coffee
                    return dialogue.orderingIcedCoffee[1];
                }
                else
                {
                    if (isImage)
                    {
                        return dialogue.orderingWarmCoffee[0]; // Return the first string for image orders
                    }
                    SetTextHint(dialogue.orderingWarmCoffee[2], npcGameObject); // Set the text hint for warm coffee
                    return dialogue.orderingWarmCoffee[1];
                }
            case "Matcha":
                if (isIced)
                {
                    if (isImage)
                    {
                        return dialogue.orderingIcedMatcha[0]; // Return the first string for image orders
                    }
                    SetTextHint(dialogue.orderingIcedMatcha[2], npcGameObject); // Set the text hint for iced matcha
                    return dialogue.orderingIcedMatcha[1];
                }
                else
                {
                    if (isImage)
                    {
                        return dialogue.orderingWarmMatcha[0]; // Return the first string for image orders
                    }
                    SetTextHint(dialogue.orderingWarmMatcha[2], npcGameObject); // Set the text hint for warm matcha
                    return dialogue.orderingWarmMatcha[1];
                }
            case "Chocolate":
                if (isIced)
                {
                    if (isImage)
                    {
                        return dialogue.orderingIcedChocolate[0]; // Return the first string for image orders
                    }
                    SetTextHint(dialogue.orderingIcedChocolate[2], npcGameObject); // Set the text hint for iced chocolate
                    return dialogue.orderingIcedChocolate[1];
                }
                else
                {
                    if (isImage)
                    {
                        return dialogue.orderingWarmChocolate[0]; // Return the first string for image orders
                    }
                    SetTextHint(dialogue.orderingWarmChocolate[2], npcGameObject); // Set the text hint for warm chocolate
                    return dialogue.orderingWarmChocolate[1];
                }
            default:
                return "An unexpected error has occurred.";
        }
    }
    
    public void SetTextHint(string hint, GameObject npc)
    {
        Debug.Log("Setting text hint: " + hint);
        TMP_Text textComponent = npc.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        // Set the text hint for the NPC
        string formattedHint = System.Text.RegularExpressions.Regex.Unescape(hint);
        textComponent.text = formattedHint;
    }
}