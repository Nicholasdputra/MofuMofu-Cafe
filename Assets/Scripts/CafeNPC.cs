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
    [Header("Important Nodes")]
    public Vector2 cashierPosition;
    public Vector2 exitPosition;

    [Header("Movement Settings")]
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
    public Transform orderBubble;
    GameObject emoteBubble;
    [SerializeField] Sprite seatedSprite;

    [Header("Order Variables")]
    public NPCState currentState = NPCState.MovingToCashier;
    
    public string npcName;
    public string npcOrderDialogue;
    public bool isOrderFormImage = false;
    public DrinkSO[] AllDrinkData;
    public DrinkData currentOrder;
    public bool hasReceivedOrder = false;

    [Header("Patience Variables")]
    [SerializeField] public float patience = 100f;
    Image heartImage;

    public enum NPCState
    {
        MovingToQueue,
        WaitingInQueue,
        MovingToCashier,
        WaitingAtCashier,
        MovingToSeat,
        Seated,
        Leaving
    }

    [Header("Other Components")]
    SpriteRenderer spriteRenderer;
    Animator animator;
    Transform NPCCanvas;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        standingSprite = spriteRenderer.sprite;
        NPCCanvas = transform.GetChild(0);

        orderBubble = NPCCanvas.GetChild(0);
        heartImage = NPCCanvas.GetChild(1).GetChild(0).GetComponent<Image>();
        emoteBubble = NPCCanvas.GetChild(2).gameObject;
        heartImage.fillAmount = patience / 100f;    

        StartCoroutine(DrainPatience());
    }
    
    public void ShowOrder()
    {
        Image drinkImage = orderBubble.GetChild(0).GetComponent<Image>();
        TMP_Text drinkClues = orderBubble.GetChild(1).GetComponent<TMP_Text>();

        if (isOrderFormImage)
        {
            foreach (DrinkSO drink in AllDrinkData)
            {
                if (drink.itemName == currentOrder.drinkName)
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

        orderBubble.gameObject.SetActive(true);
    }

    public void FinishOrder()
    {
        orderBubble.gameObject.SetActive(false);
        heartImage.transform.parent.gameObject.SetActive(false);
        patience = 5f;
        hasReceivedOrder = true;
        SetEmote("Happy");
    }

    #region Pathing
    void Update()
    {
        if (currentState != NPCState.Seated || currentState != NPCState.WaitingAtCashier)
        {
            MoveToTarget();
        }
        animator.SetFloat("Velocity", rb.velocity.magnitude);
    }

    void MoveToTarget()
    {
        if (currentPath.Count == 0 || currentPathIndex >= currentPath.Count)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        
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
                break;
            case NPCState.MovingToSeat:
                currentState = NPCState.Seated;
                spriteRenderer.sprite = seatedSprite; // Change sprite to seated
                GetComponent<SpriteSkin>().enabled = false; // Disable sprite skinning if used
                break;
            case NPCState.Leaving:
                Destroy(gameObject);
                break;
        }
    }

    public void FindSeat()
    {
        // Find empty seat
        currentSeat = SeatManager.instance.GetEmptyCafeSeat();
        if (currentSeat != null)
        {
            SeatManager.instance.OccupySeat(currentSeat, this);
            SetPathToSeat();
            currentState = NPCState.MovingToSeat;
        }
        else
        {
            SetPathToExit();
            currentState = NPCState.Leaving;
        }
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
                    SeatManager.instance.FreeSeat(currentSeat);
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
                        ScoreManager.instance.sadCustomers++;
                        SetEmote("Angry");
                    }
                    yield break;
                }

                if (currentState == NPCState.WaitingAtCashier || currentState == NPCState.MovingToCashier)
                {
                    Debug.Log(npcName + " has left due to impatience.");
                    SetEmote("Angry");
                    ScoreManager.instance.sadCustomers++;
                    CashierManager.instance.MoveQueue(true);
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
        if (!hasReceivedOrder && emoteType != "Angry")
        {
            return;
        }
            
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