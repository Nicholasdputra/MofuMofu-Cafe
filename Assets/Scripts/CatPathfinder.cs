using System.Collections.Generic;
using UnityEngine;

public class CatPathfinder : MonoBehaviour
{
    [Header("Auto-Generated Node Graph")]
    public Transform nodeParent;
    
    private Dictionary<PathNode, List<PathNode>> nodeGraph = new Dictionary<PathNode, List<PathNode>>();
    private List<PathNode> allNodes = new List<PathNode>();
    
    [ContextMenu("Build Graph from Hierarchy")]
    public void BuildGraphFromHierarchy()
    {
        if (nodeParent == null)
        {
            Debug.LogError("Node Parent not assigned!");
            return;
        }
        
        nodeGraph.Clear();
        allNodes.Clear();
        
        // Get all PathNode components in hierarchy
        PathNode[] nodes = nodeParent.GetComponentsInChildren<PathNode>();
        allNodes.AddRange(nodes);
        
        // Clear all existing connections
        foreach (PathNode node in nodes)
        {
            node.ClearConnections();
        }
        
        // Build connections based on hierarchy
        foreach (PathNode node in nodes)
        {
            
            List<PathNode> connections = new List<PathNode>();
            
            // Connect to parent (if it has PathNode)
            if (node.transform.parent != null)
            {
                PathNode parentNode = node.transform.parent.GetComponent<PathNode>();
                if (parentNode != null)
                {
                    connections.Add(parentNode);
                    node.AddConnection(parentNode);
                    parentNode.AddConnection(node); // Bidirectional
                }
            }
            
            // Connect to children (if they have PathNode)
            foreach (Transform child in node.transform)
            {
                PathNode childNode = child.GetComponent<PathNode>();
                if (childNode != null)
                {
                    connections.Add(childNode);
                    node.AddConnection(childNode);
                    childNode.AddConnection(node); // Bidirectional
                }
            }
            
            nodeGraph[node] = connections;
        }
    }
    
    public List<PathNode> FindPath(PathNode start, PathNode end, CatID catID)
    {
        if (start == null || end == null) return new List<PathNode>();
        if (start == end) return new List<PathNode> { start };
        
        // Simple A* pathfinding with cat access restrictions
        Dictionary<PathNode, PathNode> cameFrom = new Dictionary<PathNode, PathNode>();
        Dictionary<PathNode, float> gScore = new Dictionary<PathNode, float>();
        Dictionary<PathNode, float> fScore = new Dictionary<PathNode, float>();
        
        List<PathNode> openSet = new List<PathNode> { start };
        HashSet<PathNode> closedSet = new HashSet<PathNode>();
        
        gScore[start] = 0;
        fScore[start] = Vector2.Distance(start.transform.position, end.transform.position);
        
        while (openSet.Count > 0)
        {
            // Get node with lowest fScore
            PathNode current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (fScore.ContainsKey(openSet[i]) && fScore[openSet[i]] < fScore[current])
                {
                    current = openSet[i];
                }
            }
            
            if (current == end)
            {
                // Reconstruct path
                List<PathNode> path = new List<PathNode>();
                while (current != null)
                {
                    path.Add(current);
                    cameFrom.TryGetValue(current, out current);
                }
                path.Reverse();
                return path;
            }
            
            openSet.Remove(current);
            closedSet.Add(current);
            
            // Check neighbors
            if (nodeGraph.ContainsKey(current))
            {
                foreach (PathNode neighbor in nodeGraph[current])
                {
                    if (closedSet.Contains(neighbor)) continue;
                    
                    // Check if this cat can access this neighbor node
                    if (!neighbor.CanCatAccess(catID)) continue;
                    
                    float tentativeGScore = gScore[current] + Vector2.Distance(current.transform.position, neighbor.transform.position);
                    
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (gScore.ContainsKey(neighbor) && tentativeGScore >= gScore[neighbor])
                    {
                        continue;
                    }
                    
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Vector2.Distance(neighbor.transform.position, end.transform.position);
                }
            }
        }
        
        return new List<PathNode>(); // No path found
    }
    
    public List<PathNode> FindPath(PathNode start, PathNode end)
    {
        return FindPath(start, end, CatID.Cat1); // Default to Cat1
    }
    
    public List<PathNode> GetNodesByType(NodeType type, CatID catID)
    {
        List<PathNode> result = new List<PathNode>();
        foreach (PathNode node in allNodes)
        {
            if (node.nodeType == type && node.CanCatAccess(catID))
            {
                result.Add(node);
            }
        }
        return result;
    }
    
    public PathNode GetRandomNodeOfType(NodeType type, CatID catID)
    {
        List<PathNode> nodes = GetNodesByType(type, catID);
        if (nodes.Count > 0)
        {
            return nodes[Random.Range(0, nodes.Count)];
        }
        return null;
    }
    
    // Legacy methods for backward compatibility
    public List<PathNode> GetNodesByType(NodeType type)
    {
        return GetNodesByType(type, CatID.Cat1); // Default to Cat1
    }
    
    public PathNode GetRandomNodeOfType(NodeType type)
    {
        return GetRandomNodeOfType(type, CatID.Cat1); // Default to Cat1
    }
    
    void Start()
    {
        // Auto-build graph on start
        BuildGraphFromHierarchy();
    }
}
