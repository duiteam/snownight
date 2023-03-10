using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerSharedState))]
public class PlayerMovementBehavior : MonoBehaviour
{
    public float jumpForce = 2.5f;

    public PlayerSharedState playerSharedState;

    public ProjectileBehavior projectilePrefab;

    [FormerlySerializedAs("spittedSnowBehaviorPrefab")]
    public SpittedSnowBehavior spittedSnowPrefab;

    public Transform launchOffset;

    public PlayerOrientation orientation = PlayerOrientation.Right;

    private readonly Vector3 m_CamOffset = new Vector3(+5.0f, +4.5f, -10);

    // snow inventory
    private bool m_IsJumping;

    private Camera m_Cam;
    private BoxCollider2D m_Collider;
    private Rigidbody2D m_Rigidbody2D;
    private SpriteRenderer m_SpriteRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<BoxCollider2D>();
        // get the camera
        m_Cam = Camera.main;
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        // horizontal and vertical input
        var h = Input.GetAxis("Horizontal");

        // move the player horizontally
        var velocityScale = playerSharedState.snowInventory.ToVelocityScale();
        var xVelocity = h * velocityScale.Walk;
        transform.Translate(new Vector3(h * velocityScale.Walk, 0, 0) * (Time.deltaTime * 5));

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
        var lerpToX = Mathf.Lerp(m_Cam.transform.position.x, transform.position.x + m_CamOffset.x, lerpT);
        var lerpToY = Mathf.Lerp(m_Cam.transform.position.y, transform.position.y + m_CamOffset.y, lerpT * 10);
        var lerpToZ = Mathf.Lerp(m_Cam.transform.position.z, m_CamOffset.z, lerpT);
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
        if (Input.GetButtonDown("Fire1"))
        {
            var projectile = Instantiate(projectilePrefab, objectLaunchPos, transform.rotation);
            projectile.Orientation = orientation;
        }

        // use snow
        var inputIsUsingSnow = Input.GetKeyDown(KeyCode.Q);
        if (inputIsUsingSnow && playerSharedState.snowInventory >= PlayerSnowInventory.None)
        {
            playerSharedState.snowInventory--;

            UpdatePlayerState();
            
            var spittedSnow = Instantiate(spittedSnowPrefab, objectLaunchPos, transform.rotation);
            spittedSnow.Orientation = orientation;
        }

        // pick up snow
        var inputIsPickingUpSnow = Input.GetKeyDown(KeyCode.E);
        if (inputIsPickingUpSnow && 
            // collided with a SpittedSnow
            // detect it using the m_Collider
            m_Collider.IsTouchingLayers(LayerMask.GetMask("SpittedSnow"))
            && playerSharedState.snowInventory < PlayerSnowInventory.Four)
        {
            // pick up snow
            playerSharedState.snowInventory++;

            UpdatePlayerState();
        }

    }

    // on collision with the ground
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Env::Ground")) m_IsJumping = false;
    }

    // the player should change player states according to the snow inventory value
    private void UpdatePlayerState()
    {
        // change sprite
        var sprite = playerSharedState.snowInventory.ToSprite();
        m_SpriteRenderer.sprite = sprite;
    }
}    