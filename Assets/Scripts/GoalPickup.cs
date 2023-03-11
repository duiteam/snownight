using UnityEngine;

public class GoalPickup : MonoBehaviour
{
    public GameObject completeLevelUI;
    public void CompleteLevel()
    {
        completeLevelUI.SetActive(true);
        Debug.Log("Level Won!");
    }
    void OnTriggerEnter2D ()
    {
        CompleteLevel();
    }
}
