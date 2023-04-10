using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BackgroundSynchronizedDialogueBehavior : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    
    public bool disableSpeedUp;
    
    [TextArea(5, 12)]
    [Tooltip("The text segments to be displayed. Each segment will be displayed in a separate screen.\n" +
             "You may specify the background to be displayed by adding a line with the following format:\n" +
             "[UseBackground:<background resource path>]\n" +
             "The background name should be the same as the name of the background image file.")]
    public string[] segments;

    private Coroutine m_AnimCoroutine;
    private float m_CurrentDelay = 0.05f;
    
    public Image background;
    public CanvasGroup backgroundCanvasGroup;
    
    // initial color
    private Color m_InitialColor;

    private string[] m_Segments;
    
    private void Awake()
    {
        m_InitialColor = textDisplay.color;
        // copy m_Segments from segments
        Array.Resize(ref m_Segments, segments.Length);
        Array.Copy(segments, m_Segments, segments.Length);
        
        NextSegment();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextSegment();
        }
    }

    public void NextSegment()
    {
        if (m_AnimCoroutine != null)
        {
            if (disableSpeedUp)
            {
                return;
            }
            m_CurrentDelay = 0.002f;
            return;
        }
        
        if (m_Segments.Length > 0)
        {
            var text = m_Segments[0];
            m_AnimCoroutine = StartCoroutine(ChangeTextWithAnimation(text));
            Array.Copy(m_Segments, 1, m_Segments, 0, m_Segments.Length - 1);
            Array.Resize(ref m_Segments, m_Segments.Length - 1);
        }
        else
        {
            m_AnimCoroutine = null;
            m_CurrentDelay = 0.05f;
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
        
        // get the first line
        var lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        // check if the first line is a background resource path
        if (lines.Length > 0 && lines[0].StartsWith("[UseBackground:"))
        {
            var backgroundPath = lines[0].Substring("[UseBackground:".Length);
            backgroundPath = backgroundPath.Substring(0, backgroundPath.Length - 1);
            var background = Resources.Load<Sprite>(backgroundPath);
            if (background != null)
            {
                this.background.sprite = background;
                this.backgroundCanvasGroup.alpha = 1.0f;
            }
            else
            {
                Debug.LogWarning("Background image not found: " + backgroundPath);
            }
            // remove the first line
            Array.Copy(lines, 1, lines, 0, lines.Length - 1);
            Array.Resize(ref lines, lines.Length - 1);
        }
        
        // join the remaining lines
        text = string.Join("\n", lines);
        
        var current = "";
        textDisplay.text = transformSentence(current);
        foreach (var c in text)
        {
            current += c;
            textDisplay.text = transformSentence(current);
            if (c == '\n')
            {
                yield return new WaitForSeconds(m_CurrentDelay * 15);
            }
            else
            {
                yield return new WaitForSeconds(m_CurrentDelay);
            }
        }
        
        m_AnimCoroutine = null;
        m_CurrentDelay = 0.05f;
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
