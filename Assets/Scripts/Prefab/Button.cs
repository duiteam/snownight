using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public bool requirement;

    void Start()
    {
        requirement = false;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // when player or snowball collide with button, requirement = true
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Collidable::CollectableSnow")
        {
            Debug.Log("Push");
            requirement = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        // when player or snowball exit button, requirement = false
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Collidable::CollectableSnow")
        {
            Debug.Log("Exit");
            requirement = false;
        }
    }

}
