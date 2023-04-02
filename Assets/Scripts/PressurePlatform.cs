using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PressurePlatform : MonoBehaviour
{
    public float downSpeed = 1f;
    public float upSpeed = 1f;
    public float maxDownDistance = 1f;
    public bool isPlayerFollowingPlate;
    public float reboundDelay = 1f;

    private float initY;
    
    private SpriteRenderer m_SpriteRenderer;
    
    private PlayerMovementBehavior m_PlayerMovementBehavior;
    
    private Sprite m_InitialSprite;

    void Start()
    {
        initY = transform.position.y;
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_InitialSprite = m_SpriteRenderer.sprite;
    }

    void Update()
    {
        m_SpriteRenderer.sprite = m_PlayerMovementBehavior != null
            ? m_PlayerMovementBehavior.snowInventory.ToPlatformSprite()
            : m_InitialSprite;

        SetPlayerFollowingPlate(m_PlayerMovementBehavior != null && m_PlayerMovementBehavior.snowInventory >= PlayerSnowInventory.Three);

        if (isPlayerFollowingPlate)
        {
            float distance = Mathf.Clamp(initY - transform.position.y, 0, maxDownDistance);

            float speed = downSpeed * (1 - distance / maxDownDistance);

            transform.Translate(Vector3.down * (speed * Time.deltaTime));
        }
        else
        {
            float speed = upSpeed;

            transform.Translate(Vector3.up * (speed * Time.deltaTime));

            if (transform.position.y >= initY)
            {
                transform.position = new Vector3(transform.position.x, initY, transform.position.z);
            }
        }
    }
    
    private void SetPlayerFollowingPlate(bool value)
    {
        if (value != isPlayerFollowingPlate && m_PlayerMovementBehavior != null)
        {
            switch (value)
            {
                case true:
                    m_PlayerMovementBehavior.transform.SetParent(transform);
                    break;
                case false:
                    m_PlayerMovementBehavior.transform.SetParent(null);
                    break;
            }
        }
        isPlayerFollowingPlate = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        m_PlayerMovementBehavior = collision.gameObject.GetComponent<PlayerMovementBehavior>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        StartCoroutine(ReboundAfterDelay());
    }
    
    private IEnumerator ReboundAfterDelay()
    {
        yield return new WaitForSeconds(reboundDelay);
        isPlayerFollowingPlate = false;
        
        m_PlayerMovementBehavior.transform.SetParent(null);
        m_PlayerMovementBehavior = null;
    }
}
