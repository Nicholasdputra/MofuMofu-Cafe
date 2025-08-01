using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Trigger Check")]
    public PlayerTriggerCheck playerTriggerCheck;
    public ScoreManager scoreManager;

    [Header("Settings")]
    public DrinkSO item_Data;
    public bool isHoldingItem;
    [SerializeField] public GameObject hold_item;
    [SerializeField] private bool isInteractable;
    [SerializeField] private GameObject targetNPC;

    // private void OnTriggerStay2D(Collider2D collision)
    // {
    //     // Debug.Log("Player Interaction Trigger Entered with: " + collision.gameObject.name);
    //     isInteractable = true;
    //     if(collision.CompareTag("Customer"))
    //     {
    //         targetNPC = collision.gameObject;
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D collision)
    // {
    //     // Debug.Log("Player Inside: " + collision.gameObject.name);
    //     isInteractable = false;
    //     targetNPC = null;
    // }

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        isHoldingItem = false;
        hold_item.SetActive(false);
    }
    
    private void Update()
    {
        GetData();

        if (item_Data != null)
        {
            hold_item.SetActive(true);
            if (item_Data.isIced)
            {
                hold_item.GetComponent<SpriteRenderer>().sprite = item_Data.coldSprite;
            }
            else
            {
                hold_item.GetComponent<SpriteRenderer>().sprite = item_Data.hotSprite;
            }
        }

        if (isInteractable && Input.GetKeyDown(KeyCode.E) && targetNPC != null)
        {
            CafeNPC npcData = targetNPC.GetComponent<CafeNPC>();
            if (npcData.currentState != CafeNPC.NPCState.Seated) return;
            Debug.Log("Interacting with NPC: " + npcData.name);
            if (npcData.currentOrder.itemName.Contains(item_Data.itemName))
            {
                npcData.FinishOrder();
                scoreManager.AddScore((int)npcData.patience);
                isHoldingItem = false;
                hold_item.SetActive(false);
                item_Data = null;
            }
            else
            {
                Debug.Log("This NPC does not want this item.");
            }
        }
    }


    private void GetData()
    {
        isInteractable = playerTriggerCheck.collide;
        targetNPC = playerTriggerCheck.targetNPC;
    }
}
