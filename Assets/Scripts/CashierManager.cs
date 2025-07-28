using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CashierManager : MonoBehaviour
{
    public Transform cashierPosition;
    public float firstCustomerGap;
    public float customerGap;
    bool onCashier;
    bool isOpeningDialogue = false;
    bool isDialogueFinished = false;
    public GameObject dialoguePanel;
    public List<CafeNPC> customers = new List<CafeNPC>();

    void Update()
    {
        if (onCashier)
        {
            if (dialoguePanel.activeSelf && isDialogueFinished && Input.GetMouseButtonDown(0))
            {
                isDialogueFinished = false;
                isOpeningDialogue = false;
                dialoguePanel.SetActive(false);
                MoveQueue(false);
            }
            if (Input.GetKeyDown(KeyCode.E) && !isOpeningDialogue)
            {
                if(customers.Count == 0)
                {
                    Debug.Log("No customers in queue");
                    return;
                }
                if (customers[0].currentState != CafeNPC.NPCState.WaitingAtCashier)
                {
                    Debug.Log("Customer is not waiting at cashier");
                    return;
                }
                isOpeningDialogue = true;
                dialoguePanel.SetActive(true);
                // Trigger dialogue logic here
                // Debug.Log("Opening dialogue with cashier");
                // Simulate dialogue completion after a delay
                StartCoroutine(TypeDialogue());
            }
        }
    }

    public void AddCustomer(CafeNPC cafeNPC)
    {
        if (customers.Count == 0)
        {
            // Debug.Log("Adding first customer to cashier");
            cafeNPC.SetPathToCashier();
            cafeNPC.currentState = CafeNPC.NPCState.MovingToCashier;
        }
        else
        {
            // Debug.Log("Adding customer to cashier queue");
            Vector2 nextPosition = (Vector2)cashierPosition.position + new Vector2((customers.Count - 1) * customerGap + firstCustomerGap, 0);
            // Debug.Log("Setting customer path to: " + nextPosition);
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
            Debug.Log("Moving queue due to impatience");
            customers[0].SetPathToExit();
            customers[0].currentState = CafeNPC.NPCState.Leaving;

        }
        customers.RemoveAt(0);
        // Debug.Log("Removing customer, Current queue size: " + customers.Count);
        for (int i = 0; i < customers.Count; i++)
        {
            // Debug.Log("Moving customer " + i);
            if (i == 0)
            {
                // Debug.Log("Setting first customer path");
                customers[0].SetPathToCashier();
            }
            else
            {
                Vector2 nextPosition = (Vector2)cashierPosition.position + new Vector2((i - 1) * customerGap + firstCustomerGap, 0);
                // Debug.Log("Setting customer path to: " + nextPosition);
                customers[i].SetPath(nextPosition);
                customers[i].currentState = CafeNPC.NPCState.MovingToQueue;
            }
        }
    }

    private IEnumerator TypeDialogue()
    {
        TMP_Text dialogueText = dialoguePanel.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        string fullText = customers[0].npcOrderDialogue;
        dialogueText.text = "";
        foreach (char letter in fullText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f); // Adjust typing speed here
        }
        isDialogueFinished = true;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        onCashier = true;

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        onCashier = false;
    }
}
