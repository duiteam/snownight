using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueBehavior : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    
    [TextArea(3, 10)]
    public string[] sentences;
    
    private string currentSentence;
    
    private Coroutine m_AnimCoroutine;
    private float currentDelay = 0.05f;
    
    // initial color
    private Color m_InitialColor;
    
    private void Awake()
    {
        m_InitialColor = textDisplay.color;
    }

    private void Start()
    {
        NextSentence();
    }

    public void NextSentence()
    {
        if (m_AnimCoroutine != null)
        {
            currentDelay = 0.002f;
            return;
        }
        
        if (sentences.Length > 0)
        {
            var text = sentences[0];
            currentSentence = text;
            m_AnimCoroutine = StartCoroutine(ChangeTextWithAnimation(text));
            Array.Copy(sentences, 1, sentences, 0, sentences.Length - 1);
            Array.Resize(ref sentences, sentences.Length - 1);
        }
        else
        {
            NextScene();
        }
    }
    
    private void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private static string transformSentence(string sentence)
    {
        return "<style=\"Dialogue\">" + sentence + "</style>";
    }

    private IEnumerator ChangeTextWithAnimation(string text)
    {
        yield return FadeOutText();
        
        var current = "";
        textDisplay.text = transformSentence(current);
        foreach (var c in text)
        {
            current += c;
            textDisplay.text = transformSentence(current);
            yield return new WaitForSeconds(currentDelay);
        }
        
        m_AnimCoroutine = null;
        currentDelay = 0.05f;
    }

    private IEnumerator FadeOutText()
    {
        var alpha = 1.0f;
        while (alpha > 0.0f)
        {
            alpha -= 0.05f;
            textDisplay.color = new Color(m_InitialColor.r, m_InitialColor.g, m_InitialColor.b, alpha);
            yield return new WaitForSeconds(0.02f);
        }
        textDisplay.text = "";
        textDisplay.color = new Color(m_InitialColor.r, m_InitialColor.g, m_InitialColor.b, 1.0f);
    }
}
