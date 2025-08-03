using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public CharacterDialogueSO characterDialogueSO;
    CafeNPC targetNPC;
    
    public void SetDialogue(CafeNPC npc)
    {
        targetNPC = npc;
        foreach(var dialogue in characterDialogueSO.dialogues)
        {
            if (dialogue.npcName == npc.npcName)
            {
                npc.npcOrderDialogue = DetermineOrder(npc.currentOrder.drinkName, npc.currentOrder.isIced, dialogue, npc.isImage);
            }
        }
    }

    private string DetermineOrder(string drinkName, bool isIced, Dialogues dialogue, bool isImage)
    {
        switch (drinkName)
        {
            case "Coffee":
                if(isIced)
                {
                    if(isImage)
                    {
                        return dialogue.orderingIcedCoffee[0]; // Return the first string for image orders
                    }
                    targetNPC.SetTextHint(dialogue.orderingIcedCoffee[2]); // Set the text hint for iced coffee
                    return dialogue.orderingIcedCoffee[1];
                }else{
                    if(isImage)
                    {
                        return dialogue.orderingWarmCoffee[0]; // Return the first string for image orders
                    }
                    targetNPC.SetTextHint(dialogue.orderingWarmCoffee[2]); // Set the text hint for warm coffee
                    return dialogue.orderingWarmCoffee[1];
                }

            case "Matcha":
                if(isIced)
                {
                    if(isImage)
                    {
                        return dialogue.orderingIcedMatcha[0]; // Return the first string for image orders
                    }
                    targetNPC.SetTextHint(dialogue.orderingIcedMatcha[2]); // Set the text hint for iced matcha
                    return dialogue.orderingIcedMatcha[1];
                }else{
                    if(isImage)
                    {
                        return dialogue.orderingWarmMatcha[0]; // Return the first string for image orders
                    }
                    targetNPC.SetTextHint(dialogue.orderingWarmMatcha[2]); // Set the text hint for warm matcha
                    return dialogue.orderingWarmMatcha[1];
                }
            case "Chocolate":
                if(isIced)
                {
                    if(isImage)
                    {
                        return dialogue.orderingIcedChocolate[0]; // Return the first string for image orders
                    }
                    targetNPC.SetTextHint(dialogue.orderingIcedChocolate[2]); // Set the text hint for iced chocolate
                    return dialogue.orderingIcedChocolate[1];
                }else{
                    if(isImage)
                    {
                        return dialogue.orderingWarmChocolate[0]; // Return the first string for image orders
                    }
                    targetNPC.SetTextHint(dialogue.orderingWarmChocolate[2]); // Set the text hint for warm chocolate
                    return dialogue.orderingWarmChocolate[1];
                }   
            default:
                return "Boo boo order broken contact discord: nicholas_d_p.";
        }
    }
}