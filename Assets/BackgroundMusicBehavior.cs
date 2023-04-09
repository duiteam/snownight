using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicBehavior : MonoBehaviour
{
    private AudioSource m_AudioSource;
    
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
        
        m_AudioSource = GetComponent<AudioSource>();
        
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
        ChangeMusic(GetMusicNameForSceneIndex(nextScene.buildIndex));
    }
    
    private string GetMusicNameForSceneIndex(int sceneIndex)
    {
        if (sceneIndex == 0 && SceneManager.GetSceneByName(CustomSceneManager.Instance.GetSavedSceneName()).buildIndex >= 13)
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
    
    private void ChangeMusic(string musicName)
    {
        if (m_AudioSource.clip && m_AudioSource.clip.name == musicName)
        {
            return;
        }
        
        if (m_AudioSource.isPlaying)
        {
            m_AudioSource.Stop();
        }

        var resources = Resources.Load<AudioClip>($"Audio/BGM/{musicName}");
        m_AudioSource.clip = resources;
        m_AudioSource.Play();
    }
    
    
}
