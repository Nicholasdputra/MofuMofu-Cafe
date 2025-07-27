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
    public GameObject npcPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        // Spawn an NPC at the start
        SpawnNPC();
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
    void SpawnNPC()
    {
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
                cafeNPC.SetupCustomerOrder();
            }
        }
    }
}

