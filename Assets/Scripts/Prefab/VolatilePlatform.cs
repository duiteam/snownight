using UnityEngine;

public class VolatilePlatform : MonoBehaviour
{
    public Color myColor;
    public float rFloat;
    public float gFloat;
    public float bFloat;
    public float aFloat;
    public float Speed = 20f;
    public GameObject[] waypoints;

    public Renderer myRenderer;
    private readonly float FallDelay = 0.5f;

    private void Start()
    {
        aFloat = 1;
        gFloat = 0.9f;
        rFloat = 0;

        myRenderer = gameObject.GetComponent<Renderer>();
    }

    private void Update()
    {
        myColor = new Color(rFloat, gFloat, bFloat, aFloat);
        myRenderer.material.color = myColor;
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.tag == "Player")
        {
            Debug.Log("sb");
            InvokeRepeating("ColorChange", 0, 0.01f);
        }
    }

    private void ColorChange()
    {
        //Debug.Log("sb");
        if (rFloat < 0.9f)
            //Debug.Log("r up");
            rFloat += 0.001f * Time.time;
        if (gFloat > 0.1f)
            //Debug.Log("g down");
            gFloat -= 0.001f * Time.time;
        if (rFloat >= 0.9f) Invoke("Fall", FallDelay);
    }

    private void Fall()
    {
        var MovingSpeed = Speed * Time.deltaTime;
        transform.position =
            Vector3.MoveTowards(waypoints[0].transform.position, waypoints[1].transform.position, MovingSpeed);


        if (transform.position == waypoints[1].transform.position)
        {
            Destroy(gameObject);
            Debug.Log("sb");
        }
    }
}