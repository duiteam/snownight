using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public PlayerOrientation Orientation;

    public float Speed = 1.0f;

    private Rigidbody2D m_Rigidbody2D;
    private SpriteRenderer m_SpriteRenderer;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        var factor = Orientation == PlayerOrientation.Left ? -1 : 1;
        var force = new Vector2(factor * Speed, 0f);
        m_Rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        
        if (Orientation == PlayerOrientation.Left)
        {
            m_SpriteRenderer.flipX = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}