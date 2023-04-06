using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    public static CustomSceneManager Instance { get; private set; }
    
    // blockSceneNames is a list of scene names that should not be saved
    public string[] blockSceneNames;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        SaveScene(sceneName);
    }
    
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        SaveScene(SceneManager.GetSceneByBuildIndex(sceneIndex).name);
    }

    private static void SaveScene(string sceneName)
    {
        if (Instance.blockSceneNames.Contains(sceneName)) return;
        PlayerPrefs.SetString("SavedScene", sceneName);
        PlayerPrefs.Save();
    }

    public string GetSavedSceneName()
    {
        if (PlayerPrefs.HasKey("SavedScene"))
        {
            return PlayerPrefs.GetString("SavedScene");
        }
        else
        {
            Debug.LogWarning("No saved scene found!");
            return null;
        }
    }
}
