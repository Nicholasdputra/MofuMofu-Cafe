using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    public GameObject sunlightOverlay;
    private SpriteRenderer sunlightOverlayRenderer;
    public Sprite[] overlaySprites;
    public Sprite[] clockSprites;
    public int hour;
    public int minutes;
    public CashierManager cashierManager;
    int npcThisHour = 60;

    // Start is called before the first frame update
    void Start()
    {
        sunlightOverlayRenderer = sunlightOverlay.GetComponent<SpriteRenderer>();
        hour = 7;
        minutes = 0;
        GetComponent<SpriteRenderer>().sprite = clockSprites[hour-1]; // Set initial clock sprite
        StartCoroutine(StartTime());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartTime()
    {
        while (true){
            yield return new WaitForSeconds(1f); // Wait for 1 second
            minutes++;
            if (minutes >= 60)
            {
                hour++;
                minutes = 0;
                GetComponent<SpriteRenderer>().sprite = clockSprites[hour-1]; // Update clock sprite
            }
            // hour 7-10: 4-6 NPC/hour
            // hour 11-14: 2-3 NPC/hour
            // hour 15-19: 4-6 NPC/hour
            if(hour >= 7 && hour < 11)
            {
                sunlightOverlayRenderer.sprite = overlaySprites[0];
                npcThisHour = Random.Range(4, 7);
            }
            else if (hour >= 11 && hour < 15)
            {
                sunlightOverlayRenderer.sprite = overlaySprites[1]; 
                npcThisHour = Random.Range(2, 4);
            }
            else if (hour >= 15 && hour < 20)
            {
                sunlightOverlayRenderer.sprite = overlaySprites[2]; 
                npcThisHour = Random.Range(4, 7);
            }

            if(minutes % (60/ npcThisHour) == 0 && hour < 20){
                cashierManager.TrySpawnNPC();
            }
            
        }
    }
}
