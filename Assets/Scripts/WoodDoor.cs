using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodDoor : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Destroy(gameObject);
        }
    }
}
