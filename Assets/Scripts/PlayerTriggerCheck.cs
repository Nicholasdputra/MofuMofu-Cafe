using UnityEngine;

public class PlayerTriggerCheck : MonoBehaviour
{
    public bool collide;
    public GameObject targetNPC;

    void OnTriggerStay2D(Collider2D collision)
    {
        collide = true;
        if (collision.CompareTag("Customer"))
        {
            targetNPC = collision.gameObject;
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        collide = false;
        targetNPC = null;
    }
}
