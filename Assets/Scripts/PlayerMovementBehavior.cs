using UnityEngine;

[RequireComponent(typeof(PlayerSharedState))]

public class PlayerMovementBehavior : MonoBehaviour
{
    public float jumpForce = 2.5f;
    
    public PlayerSharedState playerSharedState;
    
    // snow inventory
    private bool isJumping;
    
    private Camera m_Cam;
    private CapsuleCollider2D m_BoxCollider;
    private Rigidbody2D m_Rigidbody2D;
    
    private Vector3 camOffset = new Vector3(+5.0f, +1.5f, -10);

    // Start is called before the first frame update
    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_BoxCollider = GetComponent<CapsuleCollider2D>();
        // get the camera
        m_Cam = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        // horizontal and vertical input
        var h = Input.GetAxis("Horizontal");
        
        // move the player horizontally
        transform.Translate(new Vector3(h, 0, 0) * (Time.deltaTime * 5));
        
        // lerp move camera
        // cam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        var lerpToPos = new Vector3(transform.position.x + camOffset.x, transform.position.y + camOffset.y, camOffset.z);
        m_Cam.transform.position = Vector3.Lerp(m_Cam.transform.position, lerpToPos, 0.01f);
        
        // jump
        var inputIsJumping = Input.GetKeyDown(KeyCode.Space);
        if (inputIsJumping && !isJumping)
        {
            isJumping = true;
            m_Rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
        
        // use snow
        var inputIsUsingSnow = Input.GetKeyDown(KeyCode.Q);
        if (inputIsUsingSnow && playerSharedState.snowInventory >= PlayerSnowInventory.None)
        {
            // use snow
            playerSharedState.snowInventory--;
            
            // scale the player
            ScalePlayer();
        }
        
        var inputIsPickingUpSnow = Input.GetKeyDown(KeyCode.E);
        if (inputIsPickingUpSnow && playerSharedState.snowInventory < PlayerSnowInventory.Four)
        {
            // pick up snow
            playerSharedState.snowInventory++;
            
            // scale the player
            ScalePlayer();
        }
    }
    
    // on collision with the ground
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Env::Ground"))
        {
            isJumping = false;
        }
    }
    
    // the player should scale to different sizes according to the snow inventory value
    private void ScalePlayer()
    {
        // get the scale from the snow inventory
        var scale = playerSharedState.snowInventory.ToScale();
        
        // scale the player
        transform.localScale = scale;
        
        // to make the player's collision box match its mesh size,
        // we need to scale the box collider as well
        m_BoxCollider.size = scale;
    }
}
