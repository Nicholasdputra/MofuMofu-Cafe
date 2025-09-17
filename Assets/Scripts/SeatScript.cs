using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeSeat : MonoBehaviour
{
    public CafeNPC currentNPC; 
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

    public void SetOccupied(bool occupied, CafeNPC npc = null)
    {
        isOccupied = occupied;
        currentNPC = npc;
    }
    
    public List<Vector2> GetPathFromCashier()
    {
        List<Vector2> path = new List<Vector2>();
        
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
        
        foreach (Transform node in exitPathNodes)
        {
            if (node != null)
            {
                path.Add(node.position);
            }
        }
        return path;
    }
}