using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public DrinkData item_Data;
    public bool isHoldingItem;
    [SerializeField] private GameObject hold_item;
    [SerializeField] private bool isInteractable;

    private void Start()
    {
        isHoldingItem = false;
        hold_item.SetActive(false);
    }

    private void Update()
    {
        if(item_Data != null)
        {
            hold_item.SetActive(true);
            if(item_Data.isCold)
            {
                hold_item.GetComponent<SpriteRenderer>().sprite = item_Data.coldSprite;
            }
            else
            {
                hold_item.GetComponent<SpriteRenderer>().sprite = item_Data.hotSprite;
            }
        }
    }
}
