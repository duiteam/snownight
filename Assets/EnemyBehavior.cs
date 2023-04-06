using UnityEngine;

public enum EnemyDirection
{
    Left,
    Right
}

public class EnemyBehavior : MonoBehaviour
{
    public bool shouldMove = true;
    public float moveSpeed = 1f;
    public EnemyDirection enemyDirection = EnemyDirection.Left;
    public float knockbackForce = 8f;
    
    public GameObject leftBoundary;
    public GameObject rightBoundary;
    
    private SpriteRenderer m_SpriteRenderer;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_SpriteRenderer.flipX = enemyDirection == EnemyDirection.Left;

        if (!shouldMove) return;

        enemyDirection = enemyDirection switch
        {
            EnemyDirection.Left when transform.position.x <= leftBoundary.transform.position.x => EnemyDirection.Right,
            EnemyDirection.Right when transform.position.x >= rightBoundary.transform.position.x => EnemyDirection.Left,
            _ => enemyDirection
        };

        if (enemyDirection == EnemyDirection.Left)
        {
            transform.Translate(Vector3.left * (moveSpeed * Time.deltaTime));
        }
        else
        {
            transform.Translate(Vector3.right * (moveSpeed * Time.deltaTime));
        }
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
