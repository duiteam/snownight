using UnityEngine;

public class BatBehavior : MonoBehaviour
{
    public bool shouldMove = true;
    public float moveSpeed = 1f;
    public float sineAmplitude = 1.0f;
    public float sineFrequency = 1.0f;
    
    public EnemyDirection enemyDirection = EnemyDirection.Left;
    public float knockbackForce = 8f;
    
    public GameObject waypoint1;
    public GameObject waypoint2;

    private SpriteRenderer m_SpriteRenderer;

    private float initialY;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        initialY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        m_SpriteRenderer.flipX = enemyDirection == EnemyDirection.Right;

        if (!shouldMove) return;

        enemyDirection = enemyDirection switch
        {
            EnemyDirection.Left when transform.position.x <= waypoint1.transform.position.x => EnemyDirection.Right,
            EnemyDirection.Right when transform.position.x >= waypoint2.transform.position.x => EnemyDirection.Left,
            _ => enemyDirection
        };

        MoveEnemy();
    }

    void MoveEnemy()
    {
        // Move left or right
        if (enemyDirection == EnemyDirection.Left)
        {
            transform.Translate(Vector3.left * (moveSpeed * Time.deltaTime));
        }
        else
        {
            transform.Translate(Vector3.right * (moveSpeed * Time.deltaTime));
        }

        // Move up and down in a sine wave pattern
        float newY = initialY + Mathf.Sin(Time.time * sineFrequency) * sineAmplitude;
        transform.position = new Vector2(transform.position.x, newY);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var playerRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
            var playerMovementBehavior = other.gameObject.GetComponent<PlayerMovementBehavior>();
            if (playerRigidbody != null)
            {
                var knockbackDirection = DetermineKnockbackDirection(playerRigidbody.transform.position, transform.position);
                var force = knockbackDirection * knockbackForce;
                playerRigidbody.AddForce(force, ForceMode2D.Impulse);
                playerMovementBehavior?.OpponentDoDamage();
            }
        }

        if (other.gameObject.CompareTag("Projectile"))
        {
            SFXBehavior.Instance.PlaySFX(SFXTracks.BatDead);
            Destroy(gameObject);
        }
    }

    private Vector2 DetermineKnockbackDirection(Vector2 playerPosition, Vector2 enemyPosition)
    {
        bool isPlayerOnLeft = playerPosition.x < enemyPosition.x;
        // -45 or 45 degrees
        return isPlayerOnLeft
            ? new Vector2(-1, 1)
            : new Vector2(1, 1);
    }
}
