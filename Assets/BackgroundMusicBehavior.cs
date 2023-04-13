using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicBehavior : MonoBehaviour
{
    public AudioSource bgmAudioSource;
    public AudioSource ambientAudioSource;
    
    public static BackgroundMusicBehavior Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // subscribe to SceneManager.sceneLoaded
        // when SceneManager.sceneLoaded is called, call the OnSceneLoaded method
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        
        OnActiveSceneChanged(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        // unsubscribe from SceneManager.sceneLoaded
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
    {
        ChangeBGMMusic(GetBGMMusicNameForSceneIndex(nextScene.buildIndex));
        ChangeAmbientMusic(GetAmbientMusicNameForSceneIndex(nextScene.name));
    }
    
    private static string GetBGMMusicNameForSceneIndex(int sceneIndex)
    {
        if (sceneIndex == 0 && CustomSceneManager.GetSavedSceneIndex() >= 13)
        {
            return "1"; // if already saw the ending, play the ending music
        }
        return sceneIndex switch
        {
            _ when sceneIndex <= 11 => "0",
            _ when sceneIndex == 12 => null,
            _ => "1",
        };
    }
    
    private static string GetAmbientMusicNameForSceneIndex(string sceneName)
    {
        return sceneName switch
        {
            // match sceneName to regex Level\d-\d
            _ when Regex.IsMatch(sceneName, @"Level\d-\d") => "wind",
            _ => null,
        };
    }
    
    private void ChangeBGMMusic(string musicName)
    {
        if (bgmAudioSource.clip && bgmAudioSource.clip.name == musicName)
        {
            return;
        }
        
        if (bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }

        var resources = Resources.Load<AudioClip>($"Audio/BGM/{musicName}");
        bgmAudioSource.clip = resources;
        bgmAudioSource.Play();
    }
    
    private void ChangeAmbientMusic(string musicName)
    {
        if (ambientAudioSource.clip && ambientAudioSource.clip.name == musicName)
        {
            return;
        }
        
        if (ambientAudioSource.isPlaying)
        {
            ambientAudioSource.Stop();
        }

        var resources = Resources.Load<AudioClip>($"Audio/Ambient/{musicName}");
        ambientAudioSource.clip = resources;
        ambientAudioSource.Play();
    }
    
    
}
