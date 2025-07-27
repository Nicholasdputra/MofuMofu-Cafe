using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatNPC : MonoBehaviour
{
    [Header("Cat Settings")]
    public float moveSpeed = 1.5f;
    public float nodeReachDistance = 0.3f;
    public CatID catID = CatID.Cat1; // Which cat this is
    
    [Header("Behavior Timers")]
    public float restTime = 5f;
    public float roamTime = 8f;
    public float customerVisitTime = 3f;
    
    [Header("References")]
    public CatPathfinder pathfinder;
    public SeatManager seatManager;
    
    [Header("Current State")]
    public CatState currentState = CatState.Roaming;
    
    private Rigidbody2D rb;
    private List<PathNode> currentPath = new List<PathNode>();
    private int currentPathIndex = 0;
    private PathNode targetNode;
    private PathNode currentNode;
    
    private bool isMoving = false;
    private float stateTimer = 0f;
    private bool useDirectMovement = false; // Flag for direct vs pathfinded movement
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        
        if (pathfinder == null)
            pathfinder = FindObjectOfType<CatPathfinder>();
            
        if (seatManager == null)
            seatManager = FindObjectOfType<SeatManager>();
        
        // Start with random roaming
        StartRoaming();
    }
    
    void Update()
    {
        HandleMovement();
        HandleBehaviorLogic();
    }
    
    void HandleMovement()
    {
        if (isMoving)
        {
            if (useDirectMovement)
            {
                // Direct linear movement to target node
                Vector2 targetPos = targetNode.transform.position;
                Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
                rb.velocity = direction * moveSpeed;
                
                // Check if reached target node
                if (Vector2.Distance(transform.position, targetPos) < nodeReachDistance)
                {
                    OnReachedDirectTarget();
                }
            }
            else if (currentPath.Count > 0 && currentPathIndex < currentPath.Count)
            {
                // Normal pathfinding movement
                Vector2 targetPos = currentPath[currentPathIndex].transform.position;
                Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
                rb.velocity = direction * moveSpeed;
                
                // Check if reached current node
                if (Vector2.Distance(transform.position, targetPos) < nodeReachDistance)
                {
                    OnReachedNode();
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    
    void HandleBehaviorLogic()
    {
        // Only count down timer if not currently moving
        if (!isMoving)
        {
            stateTimer += Time.deltaTime;
        }
        
        switch (currentState)
        {
            case CatState.Roaming:
                if (stateTimer >= roamTime && !isMoving)
                {
                    DecideNextBehavior();
                }
                break;
                
            case CatState.Resting:
                if (stateTimer >= restTime && !isMoving)
                {
                    StartRoaming();
                }
                break;
                
            case CatState.VisitingCustomer:
                if (stateTimer >= customerVisitTime && !isMoving)
                {
                    DecideNextBehavior();
                }
                break;
        }
    }
    
    void OnReachedNode()
    {
        currentPathIndex++;
        
        // Check if reached destination
        if (currentPathIndex >= currentPath.Count)
        {
            OnReachedDestination();
        }
    }
    
    void OnReachedDestination()
    {
        isMoving = false;
        currentNode = targetNode;
        
        // Reset state timer when reaching destination - start counting down from here
        stateTimer = 0f;
        Debug.Log($"Cat {catID} reached destination, starting {currentState} timer");
        
        switch (currentState)
        {
            case CatState.Roaming:
                // Continue roaming to another random node
                Debug.Log($"Cat {catID} roaming at destination, will change behavior in {roamTime} seconds");
                break;
                
            case CatState.Resting:
                Debug.Log($"Cat {catID} is resting in bed, will leave in {restTime} seconds");
                break;
                
            case CatState.VisitingCustomer:
                Debug.Log($"Cat {catID} is visiting customer, will leave in {customerVisitTime} seconds");
                break;
        }
    }
    
    IEnumerator ContinueRoamingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentState == CatState.Roaming)
        {
            MoveToRandomNode();
        }
    }
    
    void DecideNextBehavior()
    {
        // Check if there are customers to visit
        bool hasCustomers = HasOccupiedSeats();
        
        float randomValue = Random.Range(0f, 1f);
        
        if (hasCustomers && randomValue < 0.4f) // 40% chance to visit customer
        {
            VisitCustomer();
        }
        else if (randomValue < 0.7f) // 30% chance to rest
        {
            GoToBed();
        }
        else // 30% chance to continue roaming
        {
            StartRoaming();
        }
    }
    
    void StartRoaming()
    {
        currentState = CatState.Roaming;
        stateTimer = 0f; // Will only start counting when destination is reached
        Debug.Log($"Cat {catID} started roaming - timer will start when destination reached");
        MoveToRandomNode();
    }
    
    void GoToBed()
    {
        currentState = CatState.Resting;
        stateTimer = 0f; // Will only start counting when bed is reached
        
        PathNode bedNode = pathfinder.GetRandomNodeOfType(NodeType.CatBed, catID);
        if (bedNode != null)
        {
            MoveToNode(bedNode);
            Debug.Log($"Cat {catID} going to bed - timer will start when bed reached");
        }
        else
        {
            Debug.LogWarning($"No cat bed found for {catID}! Continuing to roam");
            StartRoaming();
        }
    }
    
    void VisitCustomer()
    {
        currentState = CatState.VisitingCustomer;
        stateTimer = 0f; // Will only start counting when customer is reached
        
        // Step 1: Pick a random occupied customer seat as target
        PathNode targetCustomerNode = GetRandomOccupiedCustomerSeat();
        if (targetCustomerNode == null)
        {
            Debug.Log($"Cat {catID}: No customers to visit, roaming instead");
            StartRoaming();
            return;
        }
        
        // Step 2: Find closest customer graph entry point
        PathNode entryNode = GetClosestCustomerGraphNode();
        if (entryNode != null)
        {
            // Step 3: Move straight to entry point, then pathfind to customer
            MoveDirectlyToNode(entryNode, targetCustomerNode);
            Debug.Log($"Cat {catID} moving directly to customer area - timer will start when customer reached");
        }
        else
        {
            Debug.Log($"Cat {catID}: No customer graph entry found, roaming instead");
            StartRoaming();
        }
    }
    
    void MoveToRandomNode()
    {
        List<PathNode> generalNodes = pathfinder.GetNodesByType(NodeType.General, catID);
        List<PathNode> junctionNodes = pathfinder.GetNodesByType(NodeType.Junction, catID);
        
        List<PathNode> availableNodes = new List<PathNode>();
        availableNodes.AddRange(generalNodes);
        availableNodes.AddRange(junctionNodes);
        
        Debug.Log($"Cat {catID}: Found {generalNodes.Count} General nodes, {junctionNodes.Count} Junction nodes");
        
        if (availableNodes.Count > 0)
        {
            PathNode randomNode = availableNodes[Random.Range(0, availableNodes.Count)];
            Debug.Log($"Cat {catID}: Moving to random node: {randomNode.name}");
            MoveToNode(randomNode);
        }
        else
        {
            Debug.LogWarning($"Cat {catID}: No available nodes found for roaming!");
        }
    }
    
    void MoveToNode(PathNode destination)
    {
        if (destination == null) 
        {
            Debug.LogWarning($"Cat {catID}: Destination node is null!");
            return;
        }
        
        PathNode startNode = GetClosestNode();
        if (startNode == null) 
        {
            Debug.LogWarning($"Cat {catID}: No closest node found!");
            return;
        }
        
        Debug.Log($"Cat {catID}: Finding path from {startNode.name} to {destination.name}");
        currentPath = pathfinder.FindPath(startNode, destination, catID);
        
        if (currentPath.Count > 0)
        {
            currentPathIndex = 0;
            targetNode = destination;
            isMoving = true;
            useDirectMovement = false; // Use pathfinding, not direct movement
            Debug.Log($"Cat {catID}: Path found with {currentPath.Count} nodes, starting movement");
        }
        else
        {
            Debug.LogWarning($"Cat {catID}: No path found from {startNode.name} to {destination.name}!");
        }
    }
    
    PathNode GetClosestNode()
    {
        PathNode closest = null;
        float closestDistance = float.MaxValue;
        
        // Get all nodes from the pathfinder's node parent
        if (pathfinder.nodeParent == null)
        {
            Debug.LogError($"Cat {catID}: Pathfinder nodeParent is not assigned!");
            return null;
        }
        
        PathNode[] allNodes = pathfinder.nodeParent.GetComponentsInChildren<PathNode>();
        Debug.Log($"Cat {catID}: Checking {allNodes.Length} nodes to find closest");
        
        foreach (PathNode node in allNodes)
        {
            float distance = Vector2.Distance(transform.position, node.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = node;
            }
        }
        
        if (closest != null)
        {
            Debug.Log($"Cat {catID}: Closest node is {closest.name} at distance {closestDistance}");
        }
        else
        {
            Debug.LogWarning($"Cat {catID}: No closest node found!");
        }
        
        return closest;
    }
    
    bool HasOccupiedSeats()
    {
        if (seatManager == null) return false;
        
        foreach (CafeSeat seat in seatManager.cafeSeats)
        {
            if (seat.IsOccupied()) return true;
        }
        return false;
    }
    
    PathNode GetRandomOccupiedCustomerSeat()
    {
        List<PathNode> occupiedCustomerNodes = new List<PathNode>();
        List<PathNode> customerNodes = pathfinder.GetNodesByType(NodeType.CustomerSeat, catID);
        
        foreach (PathNode node in customerNodes)
        {
            // Check if there's a customer nearby
            if (IsCustomerNearby(node))
            {
                occupiedCustomerNodes.Add(node);
            }
        }
        
        if (occupiedCustomerNodes.Count > 0)
        {
            return occupiedCustomerNodes[Random.Range(0, occupiedCustomerNodes.Count)];
        }
        
        return null;
    }
    
    PathNode GetClosestCustomerGraphNode()
    {
        List<PathNode> customerNodes = pathfinder.GetNodesByType(NodeType.CustomerSeat, catID);
        
        if (customerNodes.Count == 0) return null;
        
        PathNode closest = customerNodes[0];
        float closestDistance = Vector2.Distance(transform.position, closest.transform.position);
        
        foreach (PathNode node in customerNodes)
        {
            float distance = Vector2.Distance(transform.position, node.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = node;
            }
        }
        
        return closest;
    }
    
    void MoveDirectlyToNode(PathNode entryNode, PathNode finalTarget)
    {
        // Clear any existing path
        currentPath.Clear();
        currentPathIndex = 0;
        
        // Set up direct movement to entry node
        targetNode = entryNode;
        isMoving = true;
        useDirectMovement = true;
        
        // Store the final target for after we reach entry
        this.finalCustomerTarget = finalTarget;
    }
    
    void OnReachedDirectTarget()
    {
        // We've reached the customer graph entry point
        useDirectMovement = false;
        isMoving = false;
        
        // Now pathfind to the actual customer from here
        if (finalCustomerTarget != null)
        {
            MoveToNode(finalCustomerTarget);
            Debug.Log($"Cat {catID} reached customer area, now pathfinding to specific customer");
        }
        else
        {
            Debug.Log($"Cat {catID} reached customer area but no target customer");
        }
    }
    
    // Store final customer target
    private PathNode finalCustomerTarget;
    
    bool IsCustomerNearby(PathNode node)
    {
        // Check if there's an occupied seat within a certain radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(node.transform.position, 2f);
        foreach (Collider2D col in colliders)
        {
            CafeSeat seat = col.GetComponent<CafeSeat>();
            if (seat != null && seat.IsOccupied())
            {
                return true;
            }
        }
        return false;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw current path
        if (currentPath.Count > 1)
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i].transform.position, currentPath[i + 1].transform.position);
            }
        }
        
        // Draw current target
        if (targetNode != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(targetNode.transform.position, Vector3.one * 0.5f);
        }
    }
}

public enum CatState
{
    Roaming,
    Resting,
    VisitingCustomer
}
