using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpittedSnowBehavior : MonoBehaviour
{
    public PlayerOrientation Orientation;

    public float Speed = 1.0f;

    [Tooltip("The time in seconds the snowball will live")]
    public float LifetimeSeconds = 4.0f;

    [Tooltip("The time in seconds it takes for the snowball to fade out")]
    public float FadeoutTimeSeconds = 1.0f;

    private float m_InitialScale;

    private Rigidbody2D m_Rigidbody2D;
    private SpriteRenderer m_SpriteRenderer;
    private float m_StartTime;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        var factor = Orientation == PlayerOrientation.Left ? -1 : 1;
        var force = new Vector2(factor * Speed, 0.4f);
        m_Rigidbody2D.AddForce(force, ForceMode2D.Impulse);

        m_StartTime = Time.time;
        m_InitialScale = transform.localScale.x;
        StartCoroutine(DestroyObject());
    }

    private void Update()
    {
        

        // fade out the snowball
        var fadeoutTime = Mathf.Clamp(Time.time - m_StartTime - LifetimeSeconds + FadeoutTimeSeconds, 0,
            FadeoutTimeSeconds);
        var fadeoutPercentage = Mathf.Lerp(1, 0, fadeoutTime / FadeoutTimeSeconds);
        var color = m_SpriteRenderer.color;
        color.a = fadeoutPercentage;
        m_SpriteRenderer.color = color;
        // resize the snowball
        transform.localScale = m_InitialScale * fadeoutPercentage * Vector3.one;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Env::Ground"))
            // stop the snowball from moving
            m_Rigidbody2D.velocity = Vector2.zero;
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(LifetimeSeconds);
        //Wait for LifetimeSeconds
        transform.position = new Vector3(-10000.0f, -10000.0f, 0f);
        Destroy(gameObject, 1.0f);
    }
}