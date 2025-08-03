using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashcanScript : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DiscardItem();
        }
    }
    
    public void DiscardItem()
    {
        // Logic to discard the item
        PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
        if (playerInteraction.isHoldingItem)
        {
            AudioManager.instance.PlaySFX("Trash");
            playerInteraction.isHoldingItem = false;
            playerInteraction.item_Data = null;
            playerInteraction.hold_item.SetActive(false);
            // playerInteraction.hold_item = null;
        }
        else
        {
            Debug.Log("No item to discard");
        }
    }
}