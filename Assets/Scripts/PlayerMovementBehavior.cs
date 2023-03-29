using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovementBehavior : MonoBehaviour
{
    public float jumpForce = 2.5f;

    public ProjectileBehavior projectilePrefab;

    [FormerlySerializedAs("spittedSnowBehaviorPrefab")]
    public SpittedSnowBehavior spittedSnowPrefab;

    public Transform launchOffset;
    public GameObject heatIndicator;

    public PlayerOrientation orientation = PlayerOrientation.Right;

    // snow inventory
    public PlayerSnowInventory snowInventory = PlayerSnowInventory.Four;
    
    private readonly Vector3 m_CamOffset = new Vector3(+5.0f, +4.5f, -10);

    private Camera m_Cam;
    private BoxCollider2D m_Collider;

    // snow collision
    private bool m_IsJumping;
    private bool m_IsCollidingSnow;
    private GameObject m_CollidingSnow;
    
    // heat
    private int m_Heat;
    
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
        m_Collider = GetComponent<BoxCollider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        // get the camera
        m_Cam = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        // horizontal and vertical input
        var h = Input.GetAxis("Horizontal");

        // move the player horizontally
        var velocityScale = snowInventory.ToVelocityScale();
        transform.Translate(new Vector3(h, 0, 0) * (velocityScale.Walk * (Time.deltaTime * 5)));

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
        var lerpToX = Mathf.Lerp(camTransformPos.x, transformPos.x + m_CamOffset.x, lerpT);
        var lerpToY = Mathf.Lerp(camTransformPos.y, transformPos.y + m_CamOffset.y, lerpT * 10);
        var lerpToZ = Mathf.Lerp(camTransformPos.z, m_CamOffset.z, lerpT);
        m_Cam.transform.position = new Vector3(lerpToX, lerpToY, lerpToZ);

        // jump
        var inputIsJumping = Input.GetKeyDown(KeyCode.Space);
        if (inputIsJumping && !m_IsJumping)
        {
            m_IsJumping = true;
            m_Rigidbody2D.AddForce(new Vector2(0, jumpForce * velocityScale.Jump), ForceMode2D.Impulse);
        }

        var objectLaunchPos = launchOffset.position;
        if (orientation == PlayerOrientation.Left) objectLaunchPos.x -= launchOffset.localScale.x;
        else objectLaunchPos.x += launchOffset.localScale.x;

        // shooting projectile
        if (Input.GetKeyDown(KeyCode.LeftShift))
            if (snowInventory.Decrement())
            {
                var projectile = Instantiate(projectilePrefab, objectLaunchPos, transform.rotation);
                projectile.Orientation = orientation;

                UpdatePlayerState();
            }

        // use snow
        if (Input.GetKeyDown(KeyCode.Q))
            if (snowInventory.Decrement())
            {
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
        if (other.gameObject.CompareTag("Env::Ground")) m_IsJumping = false;

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
}