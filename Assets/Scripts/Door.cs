using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite sp1, sp2;
    public Button button;
    
    private Collider2D m_Collider;
    private SpriteRenderer m_SpriteRenderer;

    private void Start()
    {
        button.requirement = false;
        m_Collider = GetComponent<Collider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        m_Collider.enabled = !button.requirement; 
        m_SpriteRenderer.sprite = button.requirement ? sp1 : sp2;
    }
}
