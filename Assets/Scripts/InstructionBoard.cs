using UnityEngine;

public class InstructionBoard : MonoBehaviour
{
    public GameObject InstructionUI;
    
    public void UITurnOn()
    {
        InstructionUI.SetActive(true);
        Debug.Log("UI Open");
    }
    public void UITurnOff()
    {
        InstructionUI.SetActive(false);
        Debug.Log("UI Close");
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            UITurnOn();
        }

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            UITurnOff();
        }

    }
}

