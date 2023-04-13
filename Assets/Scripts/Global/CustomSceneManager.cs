using System.Collections;
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
        StartCoroutine(DoLoadScene(sceneName));
    }
    
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(DoLoadScene(sceneIndex));
    }
    private IEnumerator DoLoadScene(string sceneName)
    {
        yield return FindObjectOfType<LevelLoaderBehavior>().StartFadeOut();
        SceneManager.LoadScene(sceneName);
        SaveScene(SceneManager.GetSceneByName(sceneName));
        yield return FindObjectOfType<LevelLoaderBehavior>().StartFadeIn();
    }
    
    private IEnumerator DoLoadScene(int sceneIndex)
    {
        yield return FindObjectOfType<LevelLoaderBehavior>().StartFadeOut();
        SceneManager.LoadScene(sceneIndex);
        SaveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
        yield return FindObjectOfType<LevelLoaderBehavior>().StartFadeIn();
    }

    private static void SaveScene(Scene scene)
    {
        if (Instance.blockSceneNames.Contains(scene.name)) return;
        PlayerPrefs.SetString("SavedSceneName", scene.name);
        PlayerPrefs.SetInt("SavedSceneIndex", scene.buildIndex);
        PlayerPrefs.Save();
    }

    public static string GetSavedSceneName()
    {
        if (PlayerPrefs.HasKey("SavedSceneName"))
        {
            return PlayerPrefs.GetString("SavedSceneName");
        }

        Debug.LogWarning("No saved scene found!");
        return null;
    }
    
    public static int GetSavedSceneIndex()
    {
        if (PlayerPrefs.HasKey("SavedSceneIndex"))
        {
            return PlayerPrefs.GetInt("SavedSceneIndex");
        }

        Debug.LogWarning("No saved scene found!");
        return -1;
    }
}
