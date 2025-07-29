using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QTEScript : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Slider quickTimeSlider;
    [SerializeField] float sliderDrainSpeed = 0.5f;
    [SerializeField] float sliderIncrement = 10f; // Amount to increase slider on key press
    bool isQuickTimeActive = false;
    // Start is called before the first frame update
    void Start()
    {
        quickTimeSlider.maxValue = 100; // Initialize slider to full
        quickTimeSlider.value = 0; // Initialize slider to full
        gameObject.SetActive(false); // Hide the QTE UI initially
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
        }

        if (isQuickTimeActive)
        {
            playerMovement.canMove = false; // Disable player movement during QTE
        }
    }

    [ContextMenu("Start Quick Time Event")]
    public void StartQuickTimeEvent()
    {
        if (isQuickTimeActive) return; // Prevent multiple QTEs from starting
        isQuickTimeActive = true;
        quickTimeSlider.value = 0; // Reset slider to full
        gameObject.SetActive(true); // Show the QTE UI
        StartCoroutine(QuickTimeEvent());
    }
    IEnumerator QuickTimeEvent()
    {
        while(true){
            yield return new WaitForSecondsRealtime(sliderDrainSpeed);
            quickTimeSlider.value--;
            quickTimeSlider.value = Mathf.Clamp(quickTimeSlider.value, 0, 100);
            yield return null;
        }
    }

    void EndQuickTimeEvent()
    {
        //resume time
        Debug.Log("QuickTime event ended");
        Time.timeScale = 1;
        isQuickTimeActive = false;
        gameObject.SetActive(false);
        playerMovement.canMove = true; // Re-enable player movement
    }
}
