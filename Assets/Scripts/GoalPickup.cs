using UnityEngine;

public class GoalPickup : MonoBehaviour
{
    public GameObject completeLevelUI;
    public void CompleteLevel()
    {
        completeLevelUI.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Level Won!");
    }
    
    void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CompleteLevel();
        }
    }
}
