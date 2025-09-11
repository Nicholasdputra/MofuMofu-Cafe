using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Trigger Check")]
    public PlayerTriggerCheck playerTriggerCheck;

    [Header("Settings")]
    public DrinkData item_Data;
    public bool isHoldingItem;
    [SerializeField] public GameObject hold_item;
    [SerializeField] private bool isInteractable;
    [SerializeField] private GameObject targetNPC;
    
    SpriteRenderer spriteRenderer;
    public Sprite normalSprite;
    public Sprite holdingItemSprite;
    public Sprite holdingItemFaceLeftSprite;
    Vector2 lastMove;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            if (!isHoldingItem) return;
            CafeNPC npcData = targetNPC.GetComponent<CafeNPC>();
            if (npcData.currentState != CafeNPC.NPCState.Seated) return;
            // Debug.Log("Interacting with NPC: " + npcData.name);
            if (npcData.currentOrder.drinkName.Contains(item_Data.drinkName) && npcData.currentOrder.isIced == item_Data.isIced)
            {
                CalculateScore(npcData);
                npcData.FinishOrder();
                isHoldingItem = false;
                hold_item.SetActive(false);
                item_Data = null;
            }
            else
            {
                Debug.Log("This NPC does not want this item.");
            }
        }
        if (isHoldingItem)
        {
            spriteRenderer.sprite = holdingItemSprite;
            spriteRenderer.flipX = false;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Create movement vector
            Vector2 moveInput = new Vector2(horizontal, vertical).normalized;

            // Update last move direction if there's input
            if (moveInput != Vector2.zero)
            {
                lastMove = moveInput;
            }

            if (lastMove.x > 0)
            {
                spriteRenderer.sprite = holdingItemSprite; // Facing right
            }
            else if (lastMove.x < 0)
            {
                spriteRenderer.sprite = holdingItemFaceLeftSprite; // Facing left
            }
        }
        else
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    private void GetData()
    {
        isInteractable = playerTriggerCheck.collide;
        targetNPC = playerTriggerCheck.targetNPC;
    }

    void CalculateScore(CafeNPC npcData)
    {
        ScoreManager.instance.AddScore((int)npcData.patience);
        if (npcData.patience > 50)
        {
            ScoreManager.instance.happyCustomers++;
            Debug.Log("Happy Customer! Happy customer: " + ScoreManager.instance.happyCustomers);
        }
        else if (npcData.patience > 0)
        {
            ScoreManager.instance.normalCustomers++;
            Debug.Log("Normal Customer! Normal customer: " + ScoreManager.instance.normalCustomers);
        }
        else
        {
            ScoreManager.instance.sadCustomers++;
        }
    }
}
