using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite sp1, sp2;
    public Button button;
    
    private Collider2D m_Collider;
    private SpriteRenderer m_SpriteRenderer;
    private Vector3 m_InitialPosition;

    private void Start()
    {
        button.requirement = false;
        m_Collider = GetComponent<Collider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_InitialPosition = transform.position;
    }
    
    // Update is called once per frame
    private void FixedUpdate()
    {
        m_Collider.enabled = !button.requirement; 
        m_SpriteRenderer.sprite = button.requirement ? sp1 : sp2;
        transform.position = button.requirement ? (m_InitialPosition + Vector3.right * 1.25f) : m_InitialPosition;
    }
}
