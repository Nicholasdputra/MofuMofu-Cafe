using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CashierManager : MonoBehaviour
{
    public static CashierManager instance;

    public Transform cashierPosition;
    public float firstCustomerGap;
    public float customerGap;
    bool onCashier;
    [SerializeField] bool isOpeningDialogue = false;
    [SerializeField] bool isDialogueFinished = false;
    public GameObject dialoguePanel;
    public List<CafeNPC> customers = new List<CafeNPC>();
    public int bufferedNPC = 0; // Number of NPCs waiting to be added to the queue

    [Header("Dialogue Settings")]
    public Image customerImage;
    public Sprite customerSprite;
    TMP_Text dialogueText;

    [Header("Cashier Looks")]
    public SpriteRenderer cashierSpriteRenderer;
    public Sprite baseSprite;
    public Sprite selectedSprite;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of CashierManager detected: " + transform + " is an CashierManager!");
        }
    }

    private void Start()
    {
        customerImage = dialoguePanel.transform.GetComponentInChildren<Image>();
        dialogueText = dialoguePanel.transform.GetChild(1).GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        if (isOpeningDialogue)
        {
            PlayerMovement.instance.canMove = false;
        }
        
        if (onCashier)
        {
            if (dialoguePanel.activeSelf && isDialogueFinished && Input.GetKeyDown(KeyCode.E))
            {
                CloseOrderDialogue();
            }
            if (Input.GetKeyDown(KeyCode.E) && !isOpeningDialogue)
            {
                if (customers.Count == 0)
                {
                    Debug.Log("No customers in queue");
                    return;
                }
                if (customers[0].currentState != CafeNPC.NPCState.WaitingAtCashier)
                {
                    Debug.Log("Customer is not waiting at cashier");
                    return;
                }
                OpenOrderDialogue();
            }
        }
    }

    void OpenOrderDialogue()
    {
        isOpeningDialogue = true;
        dialoguePanel.SetActive(true);
        customerImage.sprite = customers[0].GetComponent<SpriteRenderer>().sprite;
        AudioManager.instance.PlaySFX(customers[0].npcName);
        StartCoroutine(TypeDialogue());
    }

    void CloseOrderDialogue()
    {
        isDialogueFinished = false;
        isOpeningDialogue = false;
        dialoguePanel.SetActive(false);
        PlayerMovement.instance.canMove = true;
        MoveQueue(false);
    }
    
    public void AddCustomer(CafeNPC cafeNPC)
    {
        if (customers.Count == 0)
        {
            cafeNPC.SetPathToCashier();
            cafeNPC.currentState = CafeNPC.NPCState.MovingToCashier;
        }
        else
        {
            Vector2 nextPosition = (Vector2)cashierPosition.position + new Vector2((customers.Count - 1) * customerGap + firstCustomerGap, 0);
            cafeNPC.SetPath(nextPosition);
            cafeNPC.currentState = CafeNPC.NPCState.MovingToQueue;
        }
        customers.Add(cafeNPC);
    }

    public void MoveQueue(bool impatience)
    {
        if (!impatience)
        {
            customers[0].AddPatience(10f);
            customers[0].ShowOrder();
            customers[0].FindSeat();
        }
        else
        {
            customers[0].SetPathToExit();
            customers[0].currentState = CafeNPC.NPCState.Leaving;

        }
        customers.RemoveAt(0);
        for (int i = 0; i < customers.Count; i++)
        {
            if (i == 0)
            {
                customers[0].SetPathToCashier();
            }
            else
            {
                Vector2 nextPosition = (Vector2)cashierPosition.position + new Vector2((i - 1) * customerGap + firstCustomerGap, 0);
                customers[i].SetPath(nextPosition);
                customers[i].currentState = CafeNPC.NPCState.MovingToQueue;
            }
        }
        if (bufferedNPC > 0){
            NPCManager.instance.SpawnNPC();
            bufferedNPC--;
        }
    }

    private IEnumerator TypeDialogue()
    {
        string fullText = customers[0].npcOrderDialogue;
        dialogueText.text = "";

        foreach (char letter in fullText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
        isDialogueFinished = true;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(cashierSpriteRenderer == null)
        {
            cashierSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        onCashier = true;
        cashierSpriteRenderer.sprite = selectedSprite;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(cashierSpriteRenderer == null)
        {
            cashierSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        onCashier = false;
        cashierSpriteRenderer.sprite = baseSprite;
    }
}
