using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QTEScript : MonoBehaviour
{
    [SerializeField] GameObject qtePanel;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Slider quickTimeSlider;
    [SerializeField] float sliderDrainSpeed = 0.5f;
    [SerializeField] float sliderIncrement = 10f; // Amount to increase slider on key press
    bool isQuickTimeActive = false;
    CatNPC currentCatNPC;
    // Start is called before the first frame update
    void Start()
    {
        quickTimeSlider.maxValue = 100; // Initialize slider to full
        quickTimeSlider.value = 0; // Initialize slider to full
        qtePanel.SetActive(false); // Hide the QTE UI initially
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
            playerMovement.canMove = false; // Disable player movement during QTE
        }
    }

    [ContextMenu("Start Quick Time Event")]
    public void StartQuickTimeEvent(CatNPC catNPC)
    {
        Debug.Log("Starting QuickTime Event for CatNPC: " + catNPC.name);
        if (playerMovement == null)
        {
            playerMovement.canMove = false; // Disable player movement during QTE
        }
        currentCatNPC = catNPC; // Set the current cat NPC
        currentCatNPC.canMove = false; // Disable cat NPC movement during QTE
        currentCatNPC.qteTriggered = true; // Set the QTE triggered flag for the cat NPC
        if (isQuickTimeActive) return; // Prevent multiple QTEs from starting
        isQuickTimeActive = true;
        quickTimeSlider.value = 5; // Reset slider to 5
        qtePanel.SetActive(true); // Show the QTE UI
        StartCoroutine(QuickTimeEvent());
    }
    IEnumerator QuickTimeEvent()
    {
        while(true){
            yield return new WaitForSeconds(sliderDrainSpeed);
            quickTimeSlider.value--;
            quickTimeSlider.value = Mathf.Clamp(quickTimeSlider.value, 0, 100);
            if (quickTimeSlider.value <= 0)
            {
                PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
                playerInteraction.isHoldingItem = false; // Reset item holding state
                playerInteraction.hold_item.SetActive(false);
                playerInteraction.item_Data = null; // Clear item data
                Debug.Log("QuickTime event failed, slider reached 0");
                EndQuickTimeEvent();

                yield break; // Exit the coroutine if the slider reaches 0
            }
            yield return null;
        }
    }

    void EndQuickTimeEvent()
    {
        StopCoroutine(QuickTimeEvent()); // Stop the QTE coroutine
        if (playerMovement == null)
        {
            playerMovement.canMove = false; // Disable player movement during QTE
        }
        currentCatNPC.canMove = true; // Re-enable cat NPC movement
        currentCatNPC.qteTriggered = false; // Reset the QTE triggered flag for the cat NPC
        currentCatNPC.justFinishedQTE = true; // Set flag to indicate QTE just finished
        currentCatNPC = null; // Clear the current cat NPC reference
        //resume time
        Debug.Log("QuickTime event ended");
        Time.timeScale = 1;
        isQuickTimeActive = false;
        qtePanel.SetActive(false);
        playerMovement.canMove = true; // Re-enable player movement
    }
}
