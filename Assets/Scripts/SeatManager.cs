using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    public static SeatManager instance;

    [Header("Seat Management")]
    public CafeSeat[] cafeSeats;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of SeatManager detected: " + transform + " is a SeatManager!");
        }
    }

    void Start()
    {
        if (cafeSeats == null || cafeSeats.Length == 0)
        {
            cafeSeats = FindObjectsOfType<CafeSeat>();
        }
    }
    
    public CafeSeat GetEmptyCafeSeat()
    {
        List<CafeSeat> emptySeats = new List<CafeSeat>();
        
        // Collect all empty seats
        foreach (CafeSeat seat in cafeSeats)
        {
            if (!seat.IsOccupied())
            {
                emptySeats.Add(seat);
            }
        }
        
        // Return random empty seat
        if (emptySeats.Count > 0)
        {
            int randomIndex = Random.Range(0, emptySeats.Count);
            return emptySeats[randomIndex];
        }
        
        return null;
    }
    
    public Vector2 GetEmptySeat()
    {
        CafeSeat emptySeat = GetEmptyCafeSeat();
        return emptySeat.transform.position;
    }
    
    public void OccupySeat(CafeSeat seat, CafeNPC npc)
    {
        seat.SetOccupied(true, npc);
    }
    
    public void FreeSeat(CafeSeat seat)
    {
        seat.SetOccupied(false);
    }
    
    public bool IsAnySeatAvailable()
    {
        foreach (CafeSeat seat in cafeSeats)
        {
            if (!seat.IsOccupied())
            {
                return true;
            }
        }
        return false;
    }
}
