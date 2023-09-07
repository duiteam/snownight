using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueBehavior : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;

    public bool disableSpeedUp;

    [TextArea(5, 12)] public string[] sentences;

    private readonly List<List<DialogueChunk>> m_AllDialogueChunks = new List<List<DialogueChunk>>();

    // canvas group
    private CanvasGroup m_CanvasGroup;

    // current active adornment view gameobject (if any)
    private GameObject m_CurrentAdornmentViewGameObject;
    private float m_CurrentDelay = 0.05f;

    private Coroutine m_OnHoldCoroutine;

    private void Awake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();

        foreach (var sentence in sentences)
        {
            var dialogueChunks = DialogueParser.Parse(sentence);
            m_AllDialogueChunks.Add(dialogueChunks);
        }

        Debug.Log(m_AllDialogueChunks);

        NextSentence();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) NextSentence();
    }

    public void NextSentence()
    {
        if (m_OnHoldCoroutine != null)
        {
            if (disableSpeedUp) return;
            m_CurrentDelay = 0.002f;
            return;
        }

        if (m_AllDialogueChunks.Count > 0)
        {
            var currentChunks = m_AllDialogueChunks[0];
            m_OnHoldCoroutine = StartCoroutine(RenderDialogueChunks(currentChunks));
            m_AllDialogueChunks.RemoveAt(0);
        }
        else
        {
            m_OnHoldCoroutine = null;
            m_CurrentDelay = 0.05f;
            NextScene();
        }
    }

    private void NextScene()
    {
        CustomSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator RenderDialogueChunks(List<DialogueChunk> dialogueChunks)
    {
        yield return FadeOutText();
        foreach (var dialogueChunk in dialogueChunks)
            switch (dialogueChunk)
            {
                case DialogueChunkText dialogueChunkText:
                    yield return RenderDialogueChunkText(dialogueChunkText);
                    break;
                case DialogueChunkAdornmentPuzzle dialogueChunkAdornmentPuzzle:
                    yield return RenderDialogueChunkAdornmentPuzzle(dialogueChunkAdornmentPuzzle);
                    break;
                case DialogueChunkAdornmentSprite dialogueChunkAdornmentSprite:
                    yield return RenderDialogueChunkAdornmentSprite(dialogueChunkAdornmentSprite);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dialogueChunk));
            }

        m_CurrentDelay = 0.05f;
        m_OnHoldCoroutine = null;

        var lastChunkIsText = dialogueChunks[dialogueChunks.Count - 1] is DialogueChunkText;
        if (!lastChunkIsText) NextSentence();
    }

    private IEnumerator RenderDialogueChunkText(DialogueChunkText dialogueChunkText)
    {
        // enumerate through the dialogueChunkText.EnumerateTextSegments()
        foreach (var dialogueChunkTextSegment in dialogueChunkText.EnumerateTextSegments())
        {
            // set the text
            textDisplay.text = dialogueChunkTextSegment;

            // wait for a delay
            yield return new WaitForSeconds(m_CurrentDelay * (dialogueChunkTextSegment.EndsWith("\n") ? 13 : 1));
        }
    }

    private IEnumerator RenderDialogueChunkAdornmentPuzzle(DialogueChunkAdornmentPuzzle dialogueChunkAdornmentPuzzle)
    {
        yield return DestroyAdornmentViewIfExists();

        var puzzleName = dialogueChunkAdornmentPuzzle.GetPuzzleName();
        // load the prefab
        var prefab = Resources.Load<GameObject>("Puzzle/Puzzle");
        var gameObject = Instantiate(prefab, transform.parent);
        var controller = gameObject.GetComponent<PuzzleController>();
        controller.puzzleConfigName = puzzleName;
        var canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.DOFade(1.0f, 0.8f);

        m_CurrentAdornmentViewGameObject = gameObject;

        // controller.OnPuzzleSolved
        var isSolved = false;
        controller.OnPuzzleSolved += () => isSolved = true;
        yield return new WaitUntil(() => isSolved);
    }

    private IEnumerator RenderDialogueChunkAdornmentSprite(DialogueChunkAdornmentSprite dialogueChunkAdornmentSprite)
    {
        DestroyAdornmentViewIfExists();

        var sprite = dialogueChunkAdornmentSprite.GetSprite();

        // load the prefab
        var prefab = Resources.Load<GameObject>("AdornmentSprite");

        var gameObject = Instantiate(prefab, transform.parent);
        var image = gameObject.GetComponent<Image>();
        image.sprite = sprite;
        var canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.DOFade(1.0f, 0.8f);

        m_CurrentAdornmentViewGameObject = gameObject;

        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator DestroyAdornmentViewIfExists()
    {
        if (m_CurrentAdornmentViewGameObject == null) yield break;

        var canvasGroupTweener = m_CurrentAdornmentViewGameObject
            .GetComponent<CanvasGroup>()
            .DOFade(0, 0.4f);
        // wait until the fade out is complete, then instantly set the text to empty
        yield return canvasGroupTweener.WaitForCompletion();
        Destroy(m_CurrentAdornmentViewGameObject);
        m_CurrentAdornmentViewGameObject = null;
    }

    private IEnumerator FadeOutText()
    {
        var canvasGroupTweener = m_CanvasGroup.DOFade(0, 0.2f);
        // wait until the fade out is complete, then instantly set the text to empty
        yield return canvasGroupTweener.WaitForCompletion();
        textDisplay.text = "";

        m_CanvasGroup.alpha = 1.0f;
    }
}