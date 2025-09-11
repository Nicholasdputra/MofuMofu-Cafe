using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QTEScript : MonoBehaviour
{
    [Header("QTE Settings")]
    [SerializeField] GameObject qtePanel;

    [Header("To Display")]
    Image qteDrinkImage;
    Image qteSliderPawImage;
    Image qteSpaceTooltipImage;

    [Header("Slider Settings")]
    [SerializeField] Slider quickTimeSlider;
    [SerializeField] float quickTimeStartValue = 10f; // Initial value for the slider when QTE starts
    [SerializeField] float sliderDecrement = 0.5f;
    [SerializeField] float sliderIncrement = 10f; // Amount to increase slider on key press

    [Header("QTE Variables")]
    public Sprite[] catPaws; // Array of sprites for cat paws
    public Sprite[] spaceTooltipSprites; // Array of sprites for space tooltip
    [SerializeField] DrinkSO[] AllDrinkData;
    
    PlayerInteraction playerInteraction;
    bool isQuickTimeActive = false;
    bool canStartQTE = true; // Flag to control if QTE can start
    [SerializeField] float qteCooldown = 3f; // Cooldown time before another QTE can start
    CatNPC currentCatNPC;
    Coroutine qteCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        playerInteraction = PlayerMovement.instance.GetComponent<PlayerInteraction>();
        quickTimeSlider.maxValue = 100;
        quickTimeSlider.value = quickTimeStartValue;
        qtePanel.SetActive(false); 
        qteDrinkImage = qtePanel.transform.GetChild(0).GetComponent<Image>();
        qteSliderPawImage = qtePanel.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
        qteSpaceTooltipImage = qtePanel.transform.GetChild(2).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            quickTimeSlider.value += sliderIncrement;
        }
        
        if (quickTimeSlider.value >= 100)
        {
            EndQuickTimeEvent();
            quickTimeSlider.value = 5; // Reset slider after successful QTE
        }

        if (isQuickTimeActive)
        {
            PlayerMovement.instance.canMove = false; // Disable player movement during QTE
        }
    }

    [ContextMenu("Start Quick Time Event")]
    public void StartQuickTimeEvent(CatNPC catNPC)
    {
        if(!canStartQTE) return;
        canStartQTE = false;
        // Debug.Log("Starting QuickTime Event for CatNPC: " + catNPC.name);
        switch(catNPC.catID)
        {
            case CatID.Cat1:
                qteSliderPawImage.sprite = catPaws[0]; // Set sprite for Cat1
                qteSpaceTooltipImage.sprite = spaceTooltipSprites[0]; // Set sprite for space tooltip
                break;
            case CatID.Cat2:
                qteSliderPawImage.sprite = catPaws[1]; // Set sprite for Cat2
                qteSpaceTooltipImage.sprite = spaceTooltipSprites[1]; // Set sprite for space tooltip
                break;
            case CatID.Cat3:
                qteSliderPawImage.sprite = catPaws[2]; // Set sprite for Cat3
                qteSpaceTooltipImage.sprite = spaceTooltipSprites[2]; // Set sprite for space tooltip
                break;
            default:
                Debug.LogWarning("Unknown Cat Type: " + catNPC.catID);
                return; // Exit if cat type is unknown
        }

        AudioManager.instance.PlaySFX("CatQTE"); // Play QTE start sound
        PlayerMovement.instance.canMove = false; // Disable player movement during QTE
        currentCatNPC = catNPC; // Set the current cat NPC
        // Debug.Log("Set Current CatNPC");
        currentCatNPC.canMove = false; // Disable cat NPC movement during QTE
        currentCatNPC.qteTriggered = true; // Set the QTE triggered flag for the cat NPC
        if (isQuickTimeActive) return; // Prevent multiple QTEs from starting
        isQuickTimeActive = true;
        quickTimeSlider.value = quickTimeStartValue; // Reset slider 
        qtePanel.SetActive(true); // Show the QTE UI

        qteDrinkImage.sprite = playerInteraction.hold_item.GetComponent<SpriteRenderer>().sprite; // Use the sprite from the player's held item
        qteCoroutine = StartCoroutine(QuickTimeEvent());
    }

    IEnumerator QuickTimeEvent()
    {
        if (quickTimeSlider.value <= 0)
        {
            quickTimeSlider.value = quickTimeStartValue;
        } 

        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            quickTimeSlider.value -= sliderDecrement;
            quickTimeSlider.value = Mathf.Clamp(quickTimeSlider.value, 0, 100);

            if (quickTimeSlider.value <= 0)
            {
                PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
                playerInteraction.isHoldingItem = false; // Reset item holding state
                playerInteraction.hold_item.SetActive(false);
                playerInteraction.item_Data = null; // Clear item data

                EndQuickTimeEvent();

                yield break; // Exit the coroutine if the slider reaches 0
            }
        }
    }

    void EndQuickTimeEvent()
    {
        StopCoroutine(qteCoroutine); // Stop the QTE coroutine
        PlayerMovement.instance.canMove = false; // Disable player movement during QTE

        if (currentCatNPC == null)
        {
            Debug.LogWarning("Current CatNPC is null, cannot end QTE.");
            return;
        }

        currentCatNPC.canMove = true; // Re-enable cat NPC movement
        currentCatNPC.qteTriggered = false; // Reset the QTE triggered flag for the cat NPC
        currentCatNPC.justFinishedQTE = true; // Set flag to indicate QTE just finished
        currentCatNPC = null; // Clear the current cat NPC reference
        Debug.Log("Setting cat NPC to null after QTE");
        //resume time
        Debug.Log("QuickTime event ended");
        Time.timeScale = 1;
        isQuickTimeActive = false;
        qtePanel.SetActive(false);
        PlayerMovement.instance.canMove = true; // Re-enable player movement
        Invoke(nameof(ResetQTECooldown), qteCooldown); // Cooldown before allowing another QTE
    }

    void ResetQTECooldown()
    {
        canStartQTE = true; // Allow QTE to start again
    }
}
