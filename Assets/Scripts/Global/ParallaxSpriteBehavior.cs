using UnityEngine;

public class ParallaxSpriteBehavior : MonoBehaviour
{
    [Tooltip("The factor by which the sprite will be parallax-ed. 0.5 means it will move half as fast as the camera.")]
    public float parallaxFactor = 0.5f;
    
    [Tooltip("The parallax axis-es. If you want to parallax only in the X axis, uncheck the Y axis.")]
    public bool parallaxX = true;
    public bool parallaxY = true;
    
    private Vector3 m_InitialPosition;
    
    private Camera m_MainCamera;
    
    private Vector3 m_InitialCameraPosition;
    
    void Awake()
    {
        m_InitialPosition = transform.position;
        
        m_MainCamera = Camera.main;
        
        if (m_MainCamera == null)
        {
            Debug.LogError("No main camera found!");
        }
        else
        {
            m_InitialCameraPosition = m_MainCamera.transform.position;
        }
    }
    
    private void LateUpdate()
    {
        if (m_MainCamera == null)
        {
            return;
        }
        
        var cameraPosition = m_MainCamera.transform.position;
        var cameraDelta = cameraPosition - m_InitialCameraPosition;
        
        var parallaxXDelta = parallaxX ? cameraDelta.x * parallaxFactor : 0;
        var parallaxYDelta = parallaxY ? cameraDelta.y * parallaxFactor : 0;
        
        transform.position = m_InitialPosition + new Vector3(parallaxXDelta, parallaxYDelta, 0);
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
}
