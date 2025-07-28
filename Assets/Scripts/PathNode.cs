using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathNode : MonoBehaviour
{
    [Header("Node Info")]
    public NodeType nodeType = NodeType.General;
    public bool isOccupied = false;
    public bool isTargetedByCat = false; // Track if a cat is currently targeting this node
    
    [Header("Ramp Settings")]
    [Tooltip("Tilt angle in degrees for ramp nodes (0 = flat, positive = upward slope, negative = downward slope)")]
    public float tiltAngle = 0f;
    
    [Header("Cat Access")]
    public CatAccessType catAccess = CatAccessType.All;
    
    [Header("Auto-Generated Connections")]
    [SerializeField] private List<PathNode> connectedNodes = new List<PathNode>();
    
    public List<PathNode> GetConnectedNodes() 
    { 
        return connectedNodes; 
    }
    
    public void AddConnection(PathNode node)
    {
        if (!connectedNodes.Contains(node))
        {
            connectedNodes.Add(node);
        }
    }
    
    public void ClearConnections()
    {
        connectedNodes.Clear();
    }
    
    public bool CanCatAccess(CatID catID)
    {
        return catAccess == CatAccessType.All || 
               (catAccess == CatAccessType.Cat1Only && catID == CatID.Cat1) ||
               (catAccess == CatAccessType.Cat2Only && catID == CatID.Cat2) ||
               (catAccess == CatAccessType.Cat3Only && catID == CatID.Cat3) ||
               catAccess == CatAccessType.Shared;
    }
    
    public Quaternion GetTargetRotation()
    {
        // Return rotation based on tilt angle (Z-axis rotation for 2D)
        return Quaternion.AngleAxis(tiltAngle, Vector3.forward);
    }
    
    public bool IsRamp()
    {
        return nodeType == NodeType.RampNode;
    }
    
    void OnDrawGizmos()
    {
        // Draw node
        Gizmos.color = GetNodeColor();
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        
        // Draw connections
        Gizmos.color = Color.white;
        foreach (PathNode connected in connectedNodes)
        {
            if (connected != null)
            {
                Gizmos.DrawLine(transform.position, connected.transform.position);
            }
        }
    }
    
    Color GetNodeColor()
    {
        switch (catAccess)
        {
            case CatAccessType.Cat1Only: return Color.red;
            case CatAccessType.Cat2Only: return Color.blue;
            case CatAccessType.Cat3Only: return Color.green;
            case CatAccessType.Shared: return Color.yellow;
            default:
                // Use node type colors for "All" access
                switch (nodeType)
                {
                    case NodeType.RoamNode: return Color.black;
                    case NodeType.RampNode: return Color.magenta;
                    case NodeType.CatBed: return Color.cyan;
                    case NodeType.CustomerSeat: return Color.yellow;
                    case NodeType.Junction: return Color.gray;
                    default: return Color.white; // General nodes
                }
        }
    }
}

public enum NodeType
{
    General,        // Regular path nodes (transit only)
    RoamNode,      // Destination nodes for cat roaming
    RampNode,      // Ramp sections with tilt angles
    CatBed,        // Cat's sleeping spot
    CustomerSeat,  // Near customer seats
    Junction       // Path intersections (transit only)
}

public enum CatAccessType
{
    All,           // Any cat can use this path
    Cat1Only,      // Only Cat1's specific path
    Cat2Only,      // Only Cat2's specific path  
    Cat3Only,      // Only Cat3's specific path
    Shared         // Shared areas (customer seats, beds)
}

public enum CatID
{
    Cat1,
    Cat2,
    Cat3
}
