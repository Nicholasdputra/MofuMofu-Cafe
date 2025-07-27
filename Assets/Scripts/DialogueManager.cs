using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public CharacterDialogueSO characterDialogueSO;
    
    public void SetDialogue(CafeNPC npc)
    {
        foreach(var dialogue in characterDialogueSO.dialogues)
        {
            if (dialogue.npcName == npc.npcName)
            {
                npc.npcOrderDialogue = DetermineOrder(npc.currentOrder, dialogue, npc.isImage);
                npc.npcRepeatedOrderDialogue = dialogue.repeatedDialogue;
            }
        }
    }

    private string DetermineOrder(CafeNPC.Order order, Dialogues dialogue, bool isImage)
    {
        switch (order)
        {
            case CafeNPC.Order.IcedCoffee:
                if(isImage)
                {
                    return dialogue.orderingIcedCoffee[0]; // Return the first string for image orders
                }
                return dialogue.orderingIcedCoffee[1];

            case CafeNPC.Order.WarmCoffee:
                if(isImage)
                {
                    return dialogue.orderingWarmCoffee[0]; // Return the first string for image orders
                }
                return dialogue.orderingWarmCoffee[1];
            case CafeNPC.Order.IcedMatcha:
                if(isImage)
                {
                    return dialogue.orderingIcedMatcha[0]; // Return the first string for image orders
                }
                return dialogue.orderingIcedMatcha[1];
            case CafeNPC.Order.WarmMatcha:
                if(isImage)     
                {
                    return dialogue.orderingWarmMatcha[0]; // Return the first string for image orders
                }
                return dialogue.orderingWarmMatcha[1];
            case CafeNPC.Order.IcedChocolate:
                if(isImage)
                {
                    return dialogue.orderingIcedChocolate[0]; // Return the first string for image orders
                }
                return dialogue.orderingIcedChocolate[1];
            case CafeNPC.Order.WarmChocolate:
                if(isImage)
                {
                    return dialogue.orderingWarmChocolate[0]; // Return the first string for image orders
                }
                return dialogue.orderingWarmChocolate[1];
            default:
                return "Boo boo order broken contact discord: nicholas_d_p.";
        }
    }
}