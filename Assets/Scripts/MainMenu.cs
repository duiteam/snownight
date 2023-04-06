using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void GoToScene(string sceneName)
    {
        CustomSceneManager.Instance.LoadScene(sceneName);
    }
    
    public void ContinueGame()
    {
        var savedSceneName = CustomSceneManager.Instance.GetSavedSceneName();
        Debug.Log("Saved scene name: " + savedSceneName);
        if (savedSceneName != null) CustomSceneManager.Instance.LoadScene(savedSceneName);
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application has quit");
    }
}
