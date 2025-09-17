using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    [Header("Sunlight Ambience")]
    public GameObject sunlightOverlay;
    private SpriteRenderer sunlightOverlayRenderer;
    public Sprite[] overlaySprites;

    [Header("Clock Settings")]
    public Sprite[] clockSprites;
    public int hour;
    public int minute;

    int npcsToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        sunlightOverlayRenderer = sunlightOverlay.GetComponent<SpriteRenderer>();
        hour = 7;
        minute = 1;
        GetComponent<SpriteRenderer>().sprite = clockSprites[(hour - 1) % 12]; // Set initial clock sprite
        StartCoroutine(StartTime());
    }

    IEnumerator StartTime()
    {
        while (true)
        {
            if (minute % 60 == 0)
            {
                hour++;
                minute = 0;
                GetComponent<SpriteRenderer>().sprite = clockSprites[(hour - 1) % 12]; // Update clock sprite
                SetNPCThisHour(); // Set the number of NPCs based on the current hour
            }

            if (minute % (60 / npcsToSpawn) == 0 && hour < 20)
            {
                NPCManager.instance.TrySpawnNPC();
            }
            else if (hour >= 20)
            {
                ScoreManager.instance.isEnding = true; // Trigger end of game if hour is 20 or more
                yield break;
            }
            yield return new WaitForSeconds(1f); // Wait for 1 second
            minute++;
        }
    }

    public void SetNPCThisHour()
    {
        if(hour >= 7 && hour < 11)
        {
            sunlightOverlayRenderer.sprite = overlaySprites[0];
            npcsToSpawn = Random.Range(4, 7);
        }
        else if (hour >= 11 && hour < 15)
        {
            sunlightOverlayRenderer.sprite = overlaySprites[1]; 
            npcsToSpawn = Random.Range(2, 4);
        }
        else if (hour >= 15 && hour < 20)
        {
            sunlightOverlayRenderer.sprite = overlaySprites[2]; 
            npcsToSpawn = Random.Range(4, 7);
        }
    }
}
