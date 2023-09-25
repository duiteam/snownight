using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image m_Background;

    private void Awake()
    {
        var name = CustomSceneManager.Instance.GetSavedSceneName();
        Debug.Log("Saved scene name: " + name);
        if (name != null)
        {
            if (name == "End1BackToMenu")
                m_Background.sprite = Resources.Load<Sprite>("SeparateSprite/menubg_be");
            else if (name == "End2BackToMenu") m_Background.sprite = Resources.Load<Sprite>("SeparateSprite/menubg_ge");
        }
    }

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