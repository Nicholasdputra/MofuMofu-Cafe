using System.Collections;
using Unity.VisualScripting;

// using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] public float timeToGenerateItem; // Time to generate item in seconds
    
    public Canvas canvas;
    public Image progressBar;
    public bool isGenerating;

    private void Start()
    {
        isInteractable = false;
        player = FindObjectOfType<PlayerInteraction>();
        
        // if (progressBar != null && gameObject.name != "Fridge")
        // {
        //     progressBar.fillAmount = 0f; // Initialize progress bar to empty
        // }
        // else
        // {
        //     Debug.LogError("Progress Bar is not assigned in the inspector for " + gameObject.name);
        //     return;
        // }
        // progressBar.fillAmount = 0f;
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
            if (isItemGenerator)
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
        
        if (isGenerating && progressBar != null)
        {
            FillProgressBar();
        }
    }

    private void GiveItemToPlayer()
    {
        if (!player.isHoldingItem)
        {
            displayedObject.SetActive(false);
            player.item_Data = DrinkObjects;
            player.item_Data.isIced = false; 
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
        canvas.gameObject.SetActive(true);
        isGenerating = true;
        yield return new WaitForSeconds(timeToGenerateItem);
        isGenerating = false;
        isItemReady = true;
        displayedObject.SetActive(true);
        progressBar.fillAmount = 0f;
        canvas.gameObject.SetActive(false);
    }

    private void FillProgressBar()
    {
        Debug.Log("Filling progress bar");
        progressBar.fillAmount += 1 / timeToGenerateItem * Time.deltaTime;
    }

    private void UpdateItemToCold()
    {
        if (player.item_Data != null)
        {
            player.item_Data.isIced = true;
        }
    }
}
