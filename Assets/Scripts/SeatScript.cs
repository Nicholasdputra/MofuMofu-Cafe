using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeSeat : MonoBehaviour
{
    [Header("Seat Settings")]
    public bool isOccupied = false;
    
    [Header("Path to Seat")]
    [Tooltip("Path nodes from cashier to this seat. First node should be near cashier, last node should be this seat.")]
    public Transform[] pathNodes;
    
    [Header("Path to Exit")]
    [Tooltip("Path nodes from this seat to exit. First node should be this seat, last node should be near exit.")]
    public Transform[] exitPathNodes;
    
    public bool IsOccupied()
    {
        return isOccupied;
    }
    
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
    
    public List<Vector2> GetPathFromCashier()
    {
        List<Vector2> path = new List<Vector2>();
        
        // Add all path nodes
        foreach (Transform node in pathNodes)
        {
            if (node != null)
            {
                path.Add(node.position);
            }
        }
        
        return path;
    }
    
    public List<Vector2> GetPathToExit()
    {
        List<Vector2> path = new List<Vector2>();
        
        // Add all exit path nodes
        foreach (Transform node in exitPathNodes)
        {
            if (node != null)
            {
                path.Add(node.position);
            }
        }
        
        // If no exit path defined, return empty list (fallback will be used)
        return path;
    }
    
    void OnDrawGizmos()
    {
        // Visual indicator for seat status
        Gizmos.color = isOccupied ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        
        // Draw path to seat
        if (pathNodes != null && pathNodes.Length > 0)
        {
            Gizmos.color = Color.cyan;
            
            // Draw path lines
            for (int i = 0; i < pathNodes.Length - 1; i++)
            {
                if (pathNodes[i] != null && pathNodes[i + 1] != null)
                {
                    Gizmos.DrawLine(pathNodes[i].position, pathNodes[i + 1].position);
                }
            }
            
            // Draw nodes
            foreach (Transform node in pathNodes)
            {
                if (node != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(node.position, 0.2f);
                }
            }
        }
        
        // Draw exit path
        if (exitPathNodes != null && exitPathNodes.Length > 0)
        {
            Gizmos.color = Color.magenta;
            
            // Draw exit path lines
            for (int i = 0; i < exitPathNodes.Length - 1; i++)
            {
                if (exitPathNodes[i] != null && exitPathNodes[i + 1] != null)
                {
                    Gizmos.DrawLine(exitPathNodes[i].position, exitPathNodes[i + 1].position);
                }
            }
            
            // Draw exit nodes
            foreach (Transform node in exitPathNodes)
            {
                if (node != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(node.position, 0.15f);
                }
            }
        }
    }
}
