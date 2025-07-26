using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeNPC : MonoBehaviour
{
    public NPCManager npcManager;
    public SeatManager seatManager;
    public Vector2 cashierPosition;
    public Vector2 exitPosition;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    private Rigidbody2D rb;
    private int currentPathIndex = 0;
    private List<Vector2> currentPath = new List<Vector2>();
    private CafeSeat currentSeat;

    public enum NPCState
    {
        MovingToCashier,
        WaitingAtCashier,
        MovingToSeat,
        Seated,
        Leaving
    }

    public NPCState currentState = NPCState.MovingToCashier;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seatManager = FindObjectOfType<SeatManager>();
        rb.gravityScale = 0f;
        
        // Start by moving to cashier
        SetPathToCashier();
    }

    void Update()
    {
        switch (currentState)
        {
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
            case NPCState.MovingToCashier:
                currentState = NPCState.WaitingAtCashier;
                StartCoroutine(OrderCoroutine());
                break;
            case NPCState.MovingToSeat:
                currentState = NPCState.Seated;
                StartCoroutine(SitAndLeave());
                break;
            case NPCState.Leaving:
                Destroy(gameObject);
                break;
        }
    }

    private IEnumerator OrderCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        
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
    }

    private IEnumerator SitAndLeave()
    {
        yield return new WaitForSeconds(5f);
        
        if (currentSeat != null)
        {
            seatManager.FreeSeat(currentSeat);
        }
        
        SetPathToExit();
        currentState = NPCState.Leaving;
    }

    void SetPathToCashier()
    {
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

    void SetPathToExit()
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
}
