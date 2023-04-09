using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueBehavior : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    
    [TextArea(5, 12)]
    public string[] sentences;

    private Coroutine m_AnimCoroutine;
    private float currentDelay = 0.05f;
    
    // initial color
    private Color m_InitialColor;

    private string[] m_Sentences;
    
    private void Awake()
    {
        m_InitialColor = textDisplay.color;
        // copy m_Sentences from sentences
        Array.Resize(ref m_Sentences, sentences.Length);
        Array.Copy(sentences, m_Sentences, sentences.Length);
        
        NextSentence();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextSentence();
        }
    }

    public void NextSentence()
    {
        if (m_AnimCoroutine != null)
        {
            currentDelay = 0.002f;
            return;
        }
        
        if (m_Sentences.Length > 0)
        {
            var text = m_Sentences[0];
            m_AnimCoroutine = StartCoroutine(ChangeTextWithAnimation(text));
            Array.Copy(m_Sentences, 1, m_Sentences, 0, m_Sentences.Length - 1);
            Array.Resize(ref m_Sentences, m_Sentences.Length - 1);
        }
        else
        {
            m_AnimCoroutine = null;
            currentDelay = 0.05f;
            NextScene();
        }
    }
    
    private void NextScene()
    {
        CustomSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
