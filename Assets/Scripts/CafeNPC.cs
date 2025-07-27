using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CafeNPC : MonoBehaviour
{
    [Header("Managers")]
    public NPCManager npcManager;
    public SeatManager seatManager;
    public CashierManager cashierManager;
    public DialogueManager dialogueManager;
    [Header("Important Nodes")]
    public Vector2 cashierPosition;
    public Vector2 exitPosition;

    [Header("Movement Settings")]
    public NPCState currentState = NPCState.MovingToCashier;
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    private Rigidbody2D rb;
    private int currentPathIndex = 0;
    private List<Vector2> currentPath = new List<Vector2>();
    private CafeSeat currentSeat;

    [Header("Emoticons")]
    public Sprite happyEmote;
    public Sprite angryEmote;
    public Sprite catEmote;

    [Header("Order Variables")]
    [TextArea(3, 10)]
    List<string> npcNames = new List<string> {
        "BusinessWoman",
        "AltGuy",
        "NerdyGuy",
        "Kid",
        "Influencer"
    };
    public string npcName;
    public string npcOrderDialogue = "I would like to order a coffee, please.";
    bool hasRepeatedOrder = false;
    public string npcRepeatedOrderDialogue = "I would like to order a coffee, please.";
    public bool isImage = false;
    public Order currentOrder;
    public bool hasReceivedOrder = false;

    [Header("Patience Variables")]
    [SerializeField] float patience = 100f;
    Image heartImage;

    public enum Order
    {
        IcedCoffee,
        WarmCoffee,
        IcedChocolate,
        WarmChocolate,
        IcedMatcha,
        WarmMatcha
    }

    public enum NPCState
    {
        //Moving within queue
        MovingToQueue,
        //Nunggu di queue, basically to stop movement
        WaitingInQueue,
        //Jalan ke cashier, so ikutin path cashier dari GetPathFromCashier
        MovingToCashier,
        //Nunggu di cashier, basically to stop movement
        WaitingAtCashier,
        MovingToSeat,
        Seated,
        Leaving
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seatManager = FindObjectOfType<SeatManager>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        heartImage = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        heartImage.fillAmount = patience / 100f;
        StartCoroutine(DrainPatience());
    }

    public void SetupCustomerOrder()
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        npcName = npcNames[Random.Range(0, npcNames.Count)];
        npcName = npcNames[0];
        currentOrder = (Order)Random.Range(0, System.Enum.GetValues(typeof(Order)).Length);
        isImage = Random.Range(0, 2) == 0; // Randomly choose if the order is an image or not
        Debug.Log(isImage ? "Image order" : "Text order");
        // Set the dialogue for the NPC
        dialogueManager.SetDialogue(this);
    }
    public void ShowOrder()
    {
        if (isImage)
        {
            // transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = GetDrinkImage(currentOrder);
            transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
            // transform.GetChild(0).GetChild(1).GetComponent<Text>().text = npcOrderDialogue;
            transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    public void HideOrder()
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    #region Movement
    void Update()
    {
        switch (currentState)
        {
            case NPCState.MovingToQueue:
                MoveToTarget();
                break;
            case NPCState.MovingToCashier:
                MoveToTarget();
                break;
            case NPCState.WaitingAtCashier:
                // NPC waits at cashier and automatically orders
                break;
            case NPCState.MovingToSeat:
                MoveToTarget();
                break;
            case NPCState.Seated:
                // NPC sits and waits
                break;
            case NPCState.Leaving:
                MoveToTarget();
                break;
        }
    }

    void MoveToTarget()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Vector2 targetPosition = currentPath[currentPathIndex];
        Vector2 currentPosition = transform.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        rb.velocity = direction * moveSpeed;

        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
        if (distanceToTarget < 0.1f)
        {
            currentPathIndex++;
            if (currentPathIndex >= currentPath.Count)
            {
                rb.velocity = Vector2.zero;
                OnPathCompleted();
            }
        }
    }

    void OnPathCompleted()
    {
        switch (currentState)
        {
            case NPCState.MovingToQueue:
                currentState = NPCState.WaitingInQueue;
                break;
            case NPCState.MovingToCashier:
                currentState = NPCState.WaitingAtCashier;
                // StartCoroutine(OrderCoroutine());
                break;
            case NPCState.MovingToSeat:
                currentState = NPCState.Seated;
                // StartCoroutine(SitAndLeave());
                break;
            case NPCState.Leaving:
                Destroy(gameObject);
                break;
        }
    }

    public void FindSeat()
    {
        Debug.Log("Finding seat for NPC: " + gameObject.name);
        // Find empty seat
        currentSeat = seatManager.GetEmptyCafeSeat();
        if (currentSeat != null)
        {
            seatManager.OccupySeat(currentSeat);
            SetPathToSeat();
            currentState = NPCState.MovingToSeat;
        }
        else
        {
            SetPathToExit();
            currentState = NPCState.Leaving;
        }
        // Debug.Log("Moving queue");
        // GameObject.Find("CashierManager").GetComponent<CashierManager>().MoveQueue();
    }

    public void SetPath(Vector2 targetPosition)
    {
        currentPath.Clear();
        currentPath.Add(targetPosition);
        currentPathIndex = 0;
    }

    public void SetPathToCashier()
    {
        currentState = NPCState.MovingToCashier;
        currentPath.Clear();
        currentPath.Add(cashierPosition);
        currentPathIndex = 0;
    }

    void SetPathToSeat()
    {
        if (currentSeat != null)
        {
            currentPath = currentSeat.GetPathFromCashier();
            currentPathIndex = 0;
        }
    }

    public void SetPathToExit()
    {
        if (currentSeat != null)
        {
            currentPath = currentSeat.GetPathToExit();
        }
        else
        {
            // If no seat was reserved, use a default exit path
            currentPath.Clear();
            GameObject exitPoint = GameObject.FindGameObjectWithTag("Exit");
            if (exitPoint != null)
            {
                currentPath.Add(exitPoint.transform.position);
            }
        }
        currentPathIndex = 0;
    }
    #endregion

    #region Patience
    private IEnumerator DrainPatience()
    {
        while (patience > 0)
        {
            if (rb.velocity == Vector2.zero)
            {
                patience -= Time.deltaTime;
            }

            heartImage.fillAmount = patience / 100f;

            if (patience <= 0)
            {
                if (currentState == NPCState.Seated)
                {
                    seatManager.FreeSeat(currentSeat);
                    SetPathToExit();
                    currentState = NPCState.Leaving;
                    if (hasReceivedOrder)
                    {
                        SetEmote("Happy");
                    }
                    else
                    {
                        HideOrder();
                        SetEmote("Angry");
                    }
                    yield break;
                }

                if (currentState == NPCState.WaitingAtCashier || currentState == NPCState.MovingToCashier)
                {
                    Debug.Log(npcName + " has left due to impatience.");
                    cashierManager.MoveQueue(true);
                }
            }

            yield return null;
        }
    }

    public void AddPatience(float amount)
    {
        patience += amount;
        if (patience > 100f) patience = 100f;
        heartImage.fillAmount = patience / 100f;
    }
    #endregion
    
    void SetEmote(string emoteType)
    {
        transform.GetChild(0).GetChild(2).GetComponent<Image>().gameObject.SetActive(true);
    }
}
