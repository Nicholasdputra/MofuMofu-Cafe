using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathNode : MonoBehaviour
{
    [Header("Node Info")]
    public NodeType nodeType = NodeType.General;
    public bool isOccupied = false;
    
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
            case CatAccessType.Cat2Only: return Color.green;
            case CatAccessType.Cat3Only: return Color.blue;
            case CatAccessType.Shared: return Color.yellow;
            default:
                // Use node type colors for "All" access
                switch (nodeType)
                {
                    case NodeType.CatBed: return Color.cyan;
                    case NodeType.CustomerSeat: return Color.magenta;
                    case NodeType.Junction: return Color.gray;
                    default: return Color.white;
                }
        }
    }
}

public enum NodeType
{
    General,        // Regular path nodes
    CatBed,        // Cat's sleeping spot
    CustomerSeat,  // Near customer seats
    Junction       // Path intersections
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
