using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuUI;
    public void PauseMenuAwake()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseMenuClose()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
