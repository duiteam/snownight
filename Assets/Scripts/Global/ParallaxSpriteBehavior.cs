using UnityEngine;

public class ParallaxSpriteBehavior : MonoBehaviour
{
    [Tooltip("The factor by which the sprite will be parallax-ed. 0.5 means it will move half as fast as the camera.")]
    public float parallaxFactor = 0.5f;
    
    [Tooltip("The parallax axises. If you want to parallax only in the X axis, uncheck the Y axis.")]
    public bool parallaxX = true;
    public bool parallaxY = true;
    
    
    private Vector3 initialPosition;
    
    private Camera mainCamera;
    
    private void LateUpdate()
    {
        if (mainCamera == null)
        {
            return;
        }
        
        var cameraPosition = mainCamera.transform.position;
        var cameraDelta = cameraPosition - initialPosition;
        
        var parallaxXDelta = parallaxX ? cameraDelta.x * parallaxFactor : 0;
        var parallaxYDelta = parallaxY ? cameraDelta.y * parallaxFactor : 0;
        
        transform.position = initialPosition + new Vector3(parallaxXDelta, parallaxYDelta, 0);
    }
    
    void Awake()
    {
        initialPosition = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
}
