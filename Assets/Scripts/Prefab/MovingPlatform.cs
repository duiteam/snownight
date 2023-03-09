using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject[] waypoints;
    public float Speed = 1f;

    private int PointIndex;

    private void Update()
    {
        if (Vector3.Distance(transform.position, waypoints[PointIndex].transform.position) < 0.1f) PointIndex += 1;
        if (PointIndex >= waypoints.Length) PointIndex = 0;
        var MovingSpeed = Speed * Time.deltaTime;

        transform.position =
            Vector3.MoveTowards(transform.position, waypoints[PointIndex].transform.position, MovingSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) collision.transform.SetParent(transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) collision.transform.SetParent(null);
    }
}