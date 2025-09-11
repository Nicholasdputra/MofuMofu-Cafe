using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;

    [Header("Destination Positions")]
    public Transform cashierPosition;
    public Transform exitPosition;

    [Header("NPC Prefabs")]
    public GameObject businessWomanPrefab;
    public GameObject nerdyPersonPrefab;
    public GameObject alternativeGuyPrefab;
    public GameObject cutesyInfluencerPrefab;
    public GameObject kidPrefab;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of NPCManager detected: " + transform + " is an NPC Manager!");
        }
    }

    void Start()
    {
        StartCoroutine(SpawnNPC());
    }

    [ContextMenu("Spawn NPC")]
    public IEnumerator SpawnNPC()
    {
        Invoke("PlayStartButtonSound", 1f);
        GameObject npcPrefab = null;
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

        if (npcPrefab != null && SeatManager.instance != null)
        {
            GameObject npc = Instantiate(npcPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            CafeNPC cafeNPC = npc.GetComponent<CafeNPC>();

            if (cafeNPC != null)
            {
                cafeNPC.cashierPosition = cashierPosition.position;
                cafeNPC.exitPosition = exitPosition.position;

                CashierManager.instance.AddCustomer(cafeNPC);
                cafeNPC.SetupCustomerOrder(npcNames[randomIndex]);
            }
        }
    }

    private void PlayStartButtonSound()
    {
        AudioManager.instance.PlaySFX("StartButton");
    }
    
    // Move Try Spawn NPC Here from Cashier Manager! REMOVE LATER (just this debug line i mean.)
    public void TrySpawnNPC()
    {
        if(CashierManager.instance.customers.Count > 3)
        {
            Debug.Log("Cashier queue is full, cannot add more customers");
            CashierManager.instance.bufferedNPC++;
        }
        else
        {
            StartCoroutine(SpawnNPC());
        }
    }
}