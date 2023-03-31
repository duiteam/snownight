using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBehavior : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    
    [TextArea(3, 10)]
    public string[] sentences;
    
    private string currentSentence;
    
    private Coroutine m_AnimCoroutine;
    private float currentDelay = 0.05f;

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
            textDisplay.text = "";
        }
    }

    private static string transformSentence(string sentence)
    {
        return "<style=\"Dialogue\">" + sentence + "</style>";
    }

    private IEnumerator ChangeTextWithAnimation(string text)
    {
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
}
