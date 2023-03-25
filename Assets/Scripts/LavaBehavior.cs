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
        yield return new WaitForSeconds(3);
        m_PlayerMovementBehavior.snowInventory.Decrement();
        m_PlayerMovementBehavior.UpdatePlayerState();
    }
}
