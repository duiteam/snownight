using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite sp1, sp2;
    public Button button;
    Collider2D collider;

    void Start()
    {
        button.requirement = false;
        collider = GetComponent<Collider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        // When Button push, door open
        if (button.requirement == true)
        {
            Debug.Log("open");
            collider.enabled = false;
            GetComponent<SpriteRenderer>().sprite = sp1;
        }
        // When Button not been push, door close
        if (button.requirement == false)
        {
            Debug.Log("close");
            collider.enabled = true;
            GetComponent<SpriteRenderer>().sprite = sp2;
        }
    }
}
