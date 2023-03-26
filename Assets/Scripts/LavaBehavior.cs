using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LavaBehavior : MonoBehaviour
{
    private PlayerMovementBehavior m_PlayerMovementBehavior;
    private Coroutine m_WaitAndDecrementPlayerSnowInventoryCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        m_PlayerMovementBehavior = GameObject.FindWithTag("Player").GetComponent<PlayerMovementBehavior>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && m_WaitAndDecrementPlayerSnowInventoryCoroutine == null)
        {
            Debug.Log("Player entered lava");
            // wait for 3 seconds before decrementing the player's snow inventory
            m_WaitAndDecrementPlayerSnowInventoryCoroutine = StartCoroutine(WaitAndDecrementPlayerSnowInventory());
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && m_WaitAndDecrementPlayerSnowInventoryCoroutine != null)
        {
            Debug.Log("Player exited lava");
            // stop the coroutine
            StopCoroutine(m_WaitAndDecrementPlayerSnowInventoryCoroutine);
            m_WaitAndDecrementPlayerSnowInventoryCoroutine = null;
        }
    }
    
    private IEnumerator WaitAndDecrementPlayerSnowInventory()
    {
        DoDecrement();
        // set a timer that triggers every 1 second.
        // when the timer triggers, increment the player's heat by one
        // when the player's heat >= 3, decrement the player's snow inventory, reset the player's heat to 0, and start the timer again
        // when the player's snow inventory <= 0, end the game
        while (true)
        {
            yield return new WaitForSeconds(0.75f);
            if (m_PlayerMovementBehavior.heat >= 3)
            {
                m_PlayerMovementBehavior.heat = 0;
                DoDecrement();
            }
            else
            {
                m_PlayerMovementBehavior.heat++;
            }
        }
        
        
    }

    private void DoDecrement()
    {
        m_PlayerMovementBehavior.snowInventory.Decrement();
        m_PlayerMovementBehavior.UpdatePlayerState();
    }
}
