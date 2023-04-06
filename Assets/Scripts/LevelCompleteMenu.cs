using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteMenu : MonoBehaviour
{
    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        CustomSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}