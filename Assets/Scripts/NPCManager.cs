using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public SeatManager seatManager;
    public CashierManager cashierManager;
    public DialogueManager dialogueManager;
    public Transform cashierPosition;
    public Transform exitPosition;
    public GameObject businessWomanPrefab;
    public GameObject nerdyPersonPrefab;
    public GameObject alternativeGuyPrefab;
    public GameObject cutesyInfluencerPrefab;
    public GameObject kidPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        // Spawn an NPC at the start
        // SpawnNPC();
    }

    void Update()
    {
        // Check for input to spawn a new NPC
        if (Input.GetKeyDown(KeyCode.N))
        {
            SpawnNPC();
        }
    }
    [ContextMenu("Spawn NPC")]
    public void SpawnNPC()
    {
        AudioManager.instance.PlaySFX("StartButton");
        // Randomly select an NPC prefab
        GameObject npcPrefab = null;
        //Copied the list of names from CafeNPC so we can just pass the index
        List<string> npcNames = new List<string> {
            "BusinessWoman",
            "Kid",
            "CutesyInfluencer",
            "AlternativeGuy",
            "NerdyPerson"
        };

        int randomIndex = UnityEngine.Random.Range(0, npcNames.Count);
        switch (npcNames[randomIndex])
        {
            case "BusinessWoman":
                npcPrefab = businessWomanPrefab;
                break;
            case "Kid":
                npcPrefab = kidPrefab;
                break;
            case "CutesyInfluencer":
                npcPrefab = cutesyInfluencerPrefab;
                break;
            case "AlternativeGuy":
                npcPrefab = alternativeGuyPrefab;
                break;
            case "NerdyPerson":
                npcPrefab = nerdyPersonPrefab;
                break;
        }

        if (npcPrefab != null && seatManager != null)
        {
            GameObject npc = Instantiate(npcPrefab, transform.position, Quaternion.identity);
            CafeNPC cafeNPC = npc.GetComponent<CafeNPC>();
            if (cafeNPC != null)
            {
                cafeNPC.npcManager = this;
                cafeNPC.seatManager = seatManager;
                cafeNPC.cashierPosition = cashierPosition.position;
                cafeNPC.exitPosition = exitPosition.position;
                cafeNPC.cashierManager = cashierManager;
                cafeNPC.dialogueManager = dialogueManager;
                // Debug.Log("Adding Customer to CashierManager");
                cashierManager.AddCustomer(cafeNPC);
                cafeNPC.SetupCustomerOrder(npcNames[randomIndex]);
            }
        }
    }
}

