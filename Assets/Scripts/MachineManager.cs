using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrinkData
{
    public string drinkName;
    public Sprite coldSprite;
    public Sprite hotSprite;
    public bool isIced;
}

public class MachineManager : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite selectedSprite;

    [SerializeField] private bool isInteractable;
    [SerializeField] private bool isItemReady;
    [SerializeField] private bool isItemGenerator;
    [SerializeField] private DrinkData DrinkObjects;
    [SerializeField] private GameObject displayedObject;
    [SerializeField] private PlayerInteraction player;

    private void Start()
    {
        isInteractable = false;
        player = FindObjectOfType<PlayerInteraction>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isInteractable = true;
            this.GetComponent<SpriteRenderer>().sprite = selectedSprite;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInteractable = false;
            this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        }
    }

    private void Update()
    {
        if (isInteractable && Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("E key pressed on machine");
            if(isItemGenerator)
            {
                if (isItemReady)
                {
                    Debug.Log("Item is ready to be given to player");
                    GiveItemToPlayer();
                }
                else
                {
                    StartCoroutine(GenerateItem());
                }
            }
            else
            {
                UpdateItemToCold();
            }
        }
    }

    private void GiveItemToPlayer()
    {
        if (!player.isHoldingItem)
        {
            displayedObject.SetActive(false);
            player.item_Data = DrinkObjects;
            isItemReady = false;
            player.isHoldingItem = true;
        }
        else
        {
            Debug.Log("Hand is Full");
        }
    }
    private IEnumerator GenerateItem()
    {
        yield return new WaitForSeconds(2f);
        isItemReady = true;
        displayedObject.SetActive(true);
    }

    private void UpdateItemToCold()
    {
        if (player.item_Data != null)
        {
            player.item_Data.isIced = true;
        }
    }
}
