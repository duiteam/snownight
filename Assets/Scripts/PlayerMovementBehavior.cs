using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerMovementBehavior : MonoBehaviour
{
    public float jumpForce = 2.5f;
    public float fallingMultiplier = 2.5f;

    public ProjectileBehavior projectilePrefab;

    [FormerlySerializedAs("spittedSnowBehaviorPrefab")]
    public SpittedSnowBehavior spittedSnowPrefab;

    public Transform launchOffset;
    public GameObject heatIndicator;

    public PlayerOrientation orientation = PlayerOrientation.Right;

    // snow inventory
    public PlayerSnowInventory snowInventory = PlayerSnowInventory.Four;
    
    public Vector3 CamOffset = new Vector3(+5.0f, +4.5f, -10);

    private Camera m_Cam;
    private CapsuleCollider2D m_Collider;

    // snow collision
    private bool m_IsJumping;
    private bool m_IsCollidingSnow;
    private GameObject m_CollidingSnow;
    
    // heat
    private int m_Heat;
    
    // damage cooldown
    [Tooltip("Minimum time in seconds between each damage taken allowed")]
    public float damageCooldown = 0.5f;
    private float m_LastDamageTime;
    
    public int heat
    {
        get => m_Heat;
        set
        {
            m_Heat = value;
            // load sprite from Resources/Sprites/Player/heatwave_{heat}
            var sprite = Resources.Load<Sprite>($"Sprites/Player/heatwave_{m_Heat.ToString()}");
            heatIndicator.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
    
    // cached component members
    private Rigidbody2D m_Rigidbody2D;
    private SpriteRenderer m_SpriteRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<CapsuleCollider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        // get the camera
        m_Cam = Camera.main;
    }
    
    private float gaussian(float x, float mu, float sigma)
    {
        var a = 1 / (sigma * Mathf.Sqrt(2 * Mathf.PI));
        var b = -((x - mu) * (x - mu)) / (2 * sigma * sigma);
        return a * Mathf.Exp(b);
    }

    // Update is called once per frame
    private void Update()
    {
        // horizontal and vertical input
        var h = Input.GetAxis("Horizontal");

        // move the player horizontally
        var velocityScale = snowInventory.ToVelocityScale();
        transform.Translate(new Vector3(h, 0, 0) * (velocityScale.Walk * (Time.deltaTime * 5)));
        // m_Rigidbody2D.velocity = new Vector2(h * velocityScale.Walk * 5, m_Rigidbody2D.velocity.y);
        // m_Rigidbody2D.AddForce(new Vector2(h * velocityScale.Walk * 5, 0));

        // flip the player
        if (h < 0)
        {
            orientation = PlayerOrientation.Left;
            m_SpriteRenderer.flipX = true;
        }
        else if (h > 0)
        {
            orientation = PlayerOrientation.Right;
            m_SpriteRenderer.flipX = false;
        }

        // lerp move camera
        // cam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        // var lerpToPos = new Vector3(transform.position.x + camOffset.x, transform.position.y + camOffset.y, camOffset.z);
        var lerpT = 0.01f * velocityScale.Walk;
        var camTransformPos = m_Cam.transform.position;
        var transformPos = transform.position;
        var lerpToX = Mathf.Lerp(camTransformPos.x, transformPos.x + CamOffset.x, lerpT);
        var lerpToY = Mathf.Lerp(camTransformPos.y, transformPos.y + CamOffset.y, lerpT * 10);
        var lerpToZ = Mathf.Lerp(camTransformPos.z, CamOffset.z, lerpT);
        m_Cam.transform.position = new Vector3(lerpToX, lerpToY, lerpToZ);

        // jump
        var inputIsJumping = Input.GetKeyDown(KeyCode.Space);
        if (inputIsJumping && !m_IsJumping)
        {
            m_IsJumping = true;
            m_Rigidbody2D.AddForce(new Vector2(0, jumpForce * velocityScale.Jump), ForceMode2D.Impulse);
        }
        
        // fall faster
        var fallVelocity = m_Rigidbody2D.velocity.y;
        var invertedGaussianFallMultiplier = fallingMultiplier + (1 - gaussian(fallVelocity, -0.5f, 2));

        // m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, (Physics2D.gravity.y * (invertedGaussianFallMultiplier - 1) * Time.deltaTime));
        m_Rigidbody2D.AddForce(new Vector2(0, Physics2D.gravity.y * (invertedGaussianFallMultiplier - 1) * Time.deltaTime), ForceMode2D.Impulse);

        var objectLaunchPos = launchOffset.position;
        if (orientation == PlayerOrientation.Left) objectLaunchPos.x -= launchOffset.localScale.x;
        else objectLaunchPos.x += launchOffset.localScale.x;

        // shooting projectile
        if (Input.GetKeyDown(KeyCode.LeftShift))
            if (snowInventory.Decrement())
            {
                SFXBehavior.Instance.PlaySFX(SFXTracks.ProjectileImpact);
                var projectile = Instantiate(projectilePrefab, objectLaunchPos, transform.rotation);
                projectile.Orientation = orientation;

                UpdatePlayerState();
            }

        // use snow
        if (Input.GetKeyDown(KeyCode.Q))
            if (snowInventory.Decrement())
            {
                SFXBehavior.Instance.PlaySFX(SFXTracks.SpitSnowball);
                var spittedSnow = Instantiate(spittedSnowPrefab, objectLaunchPos, transform.rotation);
                spittedSnow.Orientation = orientation;

                UpdatePlayerState();
            }

        if (m_IsCollidingSnow && Input.GetKeyDown(KeyCode.E))
        {
            if (snowInventory.Increment())
            {
                UpdatePlayerState();
                
                m_CollidingSnow.GetComponent<SnowdriftBehavior>().CollectSnow();
            }
        }
    }

    // on collision enter
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Env::Ground") && other.contactCount > 0)
        {
            foreach (var contact in other.contacts)
            {
                var normal = contact.normal;
                var topEdge = DetectCollisionTopEdge(normal);
                if (topEdge)
                {
                    m_IsJumping = false;
                }
                Debug.Log("Collided with " + other.gameObject.name + " at " + contact.point + " with normal " + normal);
            }
        }

        // pick up snow
        // if (Input.GetKeyDown(KeyCode.E))
        if (other.gameObject.CompareTag("Collidable::CollectableSnow"))
        {
            // pick up snow
            if (snowInventory.Increment())
            {
                UpdatePlayerState();
                
                // destroy the snow
                Destroy(other.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Snowdrift"))
        {
            m_IsCollidingSnow = true;
            m_CollidingSnow = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Snowdrift"))
        {
            m_IsCollidingSnow = false;
            m_CollidingSnow = null;
        }
    }

    // the player should change player states according to the snow inventory value
    public void UpdatePlayerState()
    {
        // change sprite
        var sprite = snowInventory.ToSprite();
        m_SpriteRenderer.sprite = sprite;

        var modifier = snowInventory.ToColliderModifier();
        m_Collider.size = modifier.Size;
        m_Collider.offset = modifier.Offset;
    }

    public void OpponentDoDamage(bool shouldObeyDamageCooldown = true)
    {
        if (shouldObeyDamageCooldown && Time.time - m_LastDamageTime < damageCooldown) return;
        
        m_LastDamageTime = Time.time;

        if (snowInventory.Decrement())
        {
            UpdatePlayerState();
        }
        else
        {
            // unable to do decrement anymore: user is at minimum snow already. kill the player
            CustomSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    bool DetectCollisionTopEdge(Vector2 normal)
    {
        return normal.y >= 0.1f;
    }
}