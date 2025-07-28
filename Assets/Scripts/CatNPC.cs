using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatNPC : MonoBehaviour
{
    [Header("Cat Settings")]
    public float moveSpeed = 1.5f;
    public float nodeReachDistance = 0.3f;
    public CatID catID = CatID.Cat1; // Which cat this is
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 180f; // Degrees per second
    
    [Header("Player Detection")]
    public float detectionRange = 2f;
    public LayerMask playerLayer = -1; // Which layer the player is on
    
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
    
    private Quaternion targetRotation = Quaternion.identity;
    private bool isRotating = false;
    
    // Player detection variables
    private bool playerInRange = false;
    private float playerDetectionTimer = 0f;
    private bool qteTriggered = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = false; // Allow rotation for ramp tilting
        
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
        HandleRotation();
        HandleBehaviorLogic();
        HandlePlayerDetection();
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
    
    void HandleRotation()
    {
        if (isRotating)
        {
            // Smoothly rotate towards target rotation
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            
            // Check if rotation is complete
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }
    
    void SetTargetRotation(Quaternion rotation)
    {
        targetRotation = rotation;
        isRotating = true;
    }
    
    void HandlePlayerDetection()
    {
        // Only detect player when not at cat bed (not resting)
        if (currentState == CatState.Resting)
        {
            // Reset detection when at bed
            playerInRange = false;
            playerDetectionTimer = 0f;
            qteTriggered = false;
            return;
        }
        
        // Check if player is in detection range
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        
        if (playerCollider != null)
        {
            // Player is in range
            if (!playerInRange)
            {
                playerInRange = true;
                playerDetectionTimer = 0f;
            }
            
            playerDetectionTimer += Time.deltaTime;
            
            // If player has been in range for more than 1 second and QTE hasn't been triggered yet
            if (playerDetectionTimer >= 1f && !qteTriggered)
            {
                qteTriggered = true;
                OpenQTE();
            }
        }
        else
        {
            // Player left range, reset detection
            if (playerInRange)
            {
                playerInRange = false;
                playerDetectionTimer = 0f;
                qteTriggered = false;
            }
        }
    }
    
    void OpenQTE()
    {
        Debug.Log($"Cat {catID}: Opening QTE! Player has been in range for 1+ seconds");
        // This is where you would implement your QTE system
        // For example: show QTE UI, start mini-game, etc.
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
        // Handle rotation based on next node type
        if (currentPathIndex < currentPath.Count - 1) // Not at final destination
        {
            PathNode currentPathNode = currentPath[currentPathIndex]; // Node we just reached
            PathNode nextNode = currentPath[currentPathIndex + 1]; // Node we're going to next
            
            // Only apply tilt when going from ramp to ramp
            if (currentPathNode != null && currentPathNode.IsRamp() && nextNode.IsRamp())
            {
                SetTargetRotation(nextNode.GetTargetRotation());
            }
            else if (!nextNode.IsRamp())
            {
                // Reset to flat when moving to non-ramp node
                SetTargetRotation(Quaternion.identity);
            }
            else if (currentPathNode.IsRamp() && !nextNode.IsRamp())
            {
                SetTargetRotation(Quaternion.identity);
            }
        }
        
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
        
        // Handle rotation for destinations
        if (targetNode != null && targetNode.IsRamp())
        {
            // Only keep ramp rotation if we came from another ramp
            PathNode previousNode = currentPath.Count > 1 ? currentPath[currentPath.Count - 2] : null;
            if (previousNode != null && previousNode.IsRamp())
            {
                SetTargetRotation(targetNode.GetTargetRotation());
            }
            else
            {
                // Reset to flat if we didn't come from a ramp
                SetTargetRotation(Quaternion.identity);
            }
        }
        else
        {
            // Reset to flat for non-ramp destinations
            SetTargetRotation(Quaternion.identity);
        }
        
        // Reset state timer when reaching destination - start counting down from here
        stateTimer = 0f;
        Debug.Log($"Cat {catID} reached destination, starting {currentState} timer");
        
        switch (currentState)
        {
            case CatState.Roaming:
                // Continue roaming to another random node
                // Debug.Log($"Cat {catID} roaming at destination, will change behavior in {roamTime} seconds");
                break;
                
            case CatState.Resting:
                // Debug.Log($"Cat {catID} is resting in bed, will leave in {restTime} seconds");
                break;
                
            case CatState.VisitingCustomer:
                // Debug.Log($"Cat {catID} is visiting customer, will leave in {customerVisitTime} seconds");
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
        // Clear any previous customer target
        if (currentState == CatState.VisitingCustomer)
        {
            ClearCustomerTarget();
        }
        
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
        // Clear any previous customer target
        if (currentState == CatState.VisitingCustomer)
        {
            ClearCustomerTarget();
        }
        
        currentState = CatState.Roaming;
        stateTimer = 0f; // Will only start counting when destination is reached
        Debug.Log($"Cat {catID} started roaming - timer will start when destination reached");
        MoveToRandomNode();
    }
    
    void GoToBed()
    {
        // Clear any previous customer target
        if (currentState == CatState.VisitingCustomer)
        {
            ClearCustomerTarget();
        }
        
        currentState = CatState.Resting;
        stateTimer = 0f; // Will only start counting when bed is reached
        
        PathNode bedNode = pathfinder.GetRandomNodeOfType(NodeType.CatBed, catID);
        if (bedNode != null)
        {
            MoveToNode(bedNode); // Back to normal pathfinding
            // Debug.Log($"Cat {catID} going to bed - timer will start when bed reached");
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
            Debug.Log($"Cat {catID} is targetting customer at {targetCustomerNode.name}");
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
        // Only use cat-specific RoamNodes for roaming destinations
        List<PathNode> roamNodes = pathfinder.GetNodesByType(NodeType.RoamNode, catID);
        
        // Filter to only include cat-specific access roam nodes
        List<PathNode> privateRoamNodes = new List<PathNode>();
        foreach (PathNode node in roamNodes)
        {
            if (IsCatSpecificNode(node))
            {
                privateRoamNodes.Add(node);
            }
        }
        
        // Debug.Log($"Cat {catID}: Found {privateRoamNodes.Count} private roam destination nodes");
        
        if (privateRoamNodes.Count > 0)
        {
            PathNode randomNode = privateRoamNodes[Random.Range(0, privateRoamNodes.Count)];
            Debug.Log($"Cat {catID}: Moving to roam destination: {randomNode.name}");
            MoveToNode(randomNode); // Back to normal pathfinding
        }
        else
        {
            Debug.LogWarning($"Cat {catID}: No private roam destination nodes found! Make sure you have RoamNode type nodes with cat-specific access.");
        }
    }
    
    bool IsCatSpecificNode(PathNode node)
    {
        // Only return true for cat-specific access types
        return (node.catAccess == CatAccessType.Cat1Only && catID == CatID.Cat1) ||
               (node.catAccess == CatAccessType.Cat2Only && catID == CatID.Cat2) ||
               (node.catAccess == CatAccessType.Cat3Only && catID == CatID.Cat3);
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
            // Debug.Log($"Cat {catID}: Path found with {currentPath.Count} nodes, starting movement");
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
        // Debug.Log($"Cat {catID}: Checking {allNodes.Length} nodes to find closest");
        
        foreach (PathNode node in allNodes)
        {
            // Check if cat can access this node
            if (!node.CanCatAccess(catID))
                continue;
                
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
        
        // Get all shared customer seat nodes
        PathNode[] allNodes = pathfinder.nodeParent.GetComponentsInChildren<PathNode>();
        
        foreach (PathNode node in allNodes)
        {
            // Only include customer seat nodes that are shared or accessible to all
            if (node.nodeType == NodeType.CustomerSeat && 
                (node.catAccess == CatAccessType.Shared || node.catAccess == CatAccessType.All))
            {
                // Check if there's a customer nearby AND no cat is already targeting this node
                if (IsCustomerNearby(node) && !node.isTargetedByCat)
                {
                    occupiedCustomerNodes.Add(node);
                }
            }
        }
        
        if (occupiedCustomerNodes.Count > 0)
        {
            PathNode randomCustomer = occupiedCustomerNodes[Random.Range(0, occupiedCustomerNodes.Count)];
            
            // Mark this node as targeted by a cat
            randomCustomer.isTargetedByCat = true;
            
            Debug.Log($"Cat {catID}: Selected random customer at {randomCustomer.name}");
            return randomCustomer;
        }
        
        Debug.Log($"Cat {catID}: No available customer seats found (either no customers or all customers already have cats)");
        return null;
    }
    
    PathNode GetClosestCustomerGraphNode()
    {
        // Get ALL customer nodes that are shared (not cat-specific)
        List<PathNode> sharedCustomerNodes = new List<PathNode>();
        
        // Get all customer seat nodes
        PathNode[] allNodes = pathfinder.nodeParent.GetComponentsInChildren<PathNode>();
        
        foreach (PathNode node in allNodes)
        {
            // Only include customer seat nodes that are shared or accessible to all
            if (node.catAccess == CatAccessType.All && node.nodeType != NodeType.CustomerSeat)
            {
                sharedCustomerNodes.Add(node);
            }
        }
        
        if (sharedCustomerNodes.Count == 0) 
        {
            Debug.LogWarning($"Cat {catID}: No shared customer nodes found!");
            return null;
        }
        
        // Find physically closest node (straight line distance, not pathfinding)
        PathNode closest = sharedCustomerNodes[0];
        float closestDistance = Vector2.Distance(transform.position, closest.transform.position);
        
        foreach (PathNode node in sharedCustomerNodes)
        {
            float distance = Vector2.Distance(transform.position, node.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = node;
            }
        }
        
        Debug.Log($"Cat {catID}: Physically closest shared customer node is {closest.name} at distance {closestDistance}");
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
        
        // Handle rotation for direct targets - only keep ramp rotation if appropriate
        if (targetNode != null && targetNode.IsRamp())
        {
            // For direct movement, we don't have a previous ramp context, so apply ramp rotation
            SetTargetRotation(targetNode.GetTargetRotation());
        }
        else
        {
            // Reset to flat for non-ramp targets
            SetTargetRotation(Quaternion.identity);
        }
        
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
        // For CustomerSeat nodes, check if there's a CafeSeat component and if it's occupied
        if (node.nodeType == NodeType.CustomerSeat)
        {
            CafeSeat seat = node.GetComponent<CafeSeat>();
            if (seat != null)
            {
                return seat.IsOccupied();
            }
        }
        
        // Fallback: check for seated customers in the area (make sure they're actually seated, not just present)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(node.transform.position, 2f);
        foreach (Collider2D col in colliders)
        {
            CafeNPC customer = col.GetComponent<CafeNPC>();
            if (customer != null && customer.currentState == CafeNPC.NPCState.Seated)
            {
                // Double check that customer is actually seated and not just transitioning
                if (Vector2.Distance(customer.transform.position, node.transform.position) < 1f)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    void ClearCustomerTarget()
    {
        // Clear the targeting flag from the customer node
        if (finalCustomerTarget != null)
        {
            finalCustomerTarget.isTargetedByCat = false;
        }
        finalCustomerTarget = null;
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
        
        // Draw detection range (only when not at cat bed)
        if (currentState != CatState.Resting)
        {
            Gizmos.color = playerInRange ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}

public enum CatState
{
    Roaming,
    Resting,
    VisitingCustomer
}
