using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void RestartLevel()
    {
        CustomSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
