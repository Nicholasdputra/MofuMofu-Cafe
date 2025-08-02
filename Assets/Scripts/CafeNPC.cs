using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D.Animation;

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
    Sprite standingSprite;
    GameObject orderBubble;
    GameObject emoteBubble;
    [SerializeField] Sprite seatedSprite;

    [Header("Order Variables")]
    [TextArea(3, 10)]
    List<string> npcNames = new List<string> {
        "BusinessWoman",
        "Kid",
        "CutesyInfluencer",
        "AlternativeGuy",
        "NerdyPerson"
    };
    public string npcName;
    public string npcOrderDialogue;
    public bool isImage = false;
    public DrinkSO[] AllDrinkData;
    public DrinkSO currentOrder;
    public bool hasReceivedOrder = false;

    [Header("Patience Variables")]
    [SerializeField] public float patience = 100f;
    Image heartImage;
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

    SpriteRenderer spriteRenderer;
    Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        seatManager = FindObjectOfType<SeatManager>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        standingSprite = spriteRenderer.sprite;

        emoteBubble = transform.GetChild(0).GetChild(2).gameObject;
        orderBubble = transform.GetChild(0).GetChild(0).gameObject;
        heartImage = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        heartImage.fillAmount = patience / 100f;

        

        StartCoroutine(DrainPatience());
    }

    public void SetupCustomerOrder(string npcName)
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        this.npcName = npcName;
        currentOrder = AllDrinkData[Random.Range(0, AllDrinkData.Length)];
        currentOrder.isIced = Random.Range(0, 2) == 0; // Randomly choose if the drink is iced or not
        isImage = Random.Range(0, 2) == 0; // Randomly choose if the order is an image or not
        Debug.Log(isImage ? "Image order" : "Text order");
        // Set the dialogue for the NPC
        dialogueManager.SetDialogue(this);
    }

    public void SetTextHint(string hint)
    {
        Debug.Log("Setting text hint: " + hint);
        TMP_Text textComponent = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        // Set the text hint for the NPC
        //TMP Text renders special characters like \n, so we need to unescape it
        string formattedHint = System.Text.RegularExpressions.Regex.Unescape(hint);

        textComponent.text = formattedHint;

    }
    public void ShowOrder()
    {
        Image drinkImage = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        TMP_Text drinkClues = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        if (isImage)
        {
            foreach (DrinkSO drink in AllDrinkData)
            {
                if (drink.itemName == currentOrder.itemName)
                {
                    if (currentOrder.isIced)
                    {
                        drinkImage.sprite = drink.coldSprite;
                    }
                    else
                    {
                        drinkImage.sprite = drink.hotSprite;
                    }
                    break;
                }
            }
            drinkImage.gameObject.SetActive(true);
            drinkClues.gameObject.SetActive(false);
        }
        else
        {
            // Hint is already set alongside NPC dialogue
            drinkImage.gameObject.SetActive(false);
            drinkClues.gameObject.SetActive(true);
        }
        //Order bubble
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    public void FinishOrder()
    {
        orderBubble.SetActive(false);
        heartImage.transform.parent.gameObject.SetActive(false);
        patience = 5f;
        hasReceivedOrder = true;
        SetEmote("Happy");
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
        animator.SetFloat("Velocity", rb.velocity.magnitude);
    }

    void MoveToTarget()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Vector2 targetPosition = currentPath[currentPathIndex];
        Vector2 currentPosition = transform.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        rb.velocity = direction * moveSpeed;

        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
        if (distanceToTarget < 0.05f)
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
                spriteRenderer.sprite = seatedSprite; // Change sprite to seated
                GetComponent<SpriteSkin>().enabled = false; // Disable sprite skinning if used
                // StartCoroutine(SitAndLeave());
                break;
            case NPCState.Leaving:
                Destroy(gameObject);
                break;
        }
    }

    public void FindSeat()
    {
        // Debug.Log("Finding seat for NPC: " + gameObject.name);
        // Find empty seat
        currentSeat = seatManager.GetEmptyCafeSeat();
        if (currentSeat != null)
        {
            seatManager.OccupySeat(currentSeat, this);
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
                    spriteRenderer.sprite = standingSprite; // Change sprite back to standing
                    GetComponent<SpriteSkin>().enabled = true; // Re-enable sprite skinning if used
                    currentState = NPCState.Leaving;
                    if (hasReceivedOrder)
                    {
                        SetEmote("Happy");
                    }
                    else
                    {
                        FinishOrder();
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

    public void SetEmote(string emoteType)
    {
        if (!hasReceivedOrder && emoteType != "Angry") return;
        emoteBubble.SetActive(true);
        Image emoteImage = emoteBubble.transform.GetChild(0).GetComponent<Image>();
        switch (emoteType)
        {
            case "Happy":
                emoteImage.sprite = happyEmote;
                break;
            case "Angry":
                emoteImage.sprite = angryEmote;
                break;
            case "Cat":
                emoteImage.sprite = catEmote;
                break;
        }
        Invoke(nameof(HideEmote), 2f); // Hide emote after 1 second
    }
    
    void HideEmote()
    {
        emoteBubble.SetActive(false);
    }
}
