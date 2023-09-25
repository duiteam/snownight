using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePieceController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public PuzzleController puzzleController;
    public CanvasGroup glowCanvasGroup;
    private CanvasGroup m_CanvasGroup; // To control visibility and raycast target during drag
    private bool m_IsDragging;

    private Vector2 m_OriginalPosition;
    private RectTransform m_RectTransform;
    public PuzzlePiece puzzlePiece;


    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnDisable()
    {
        glowCanvasGroup.DOFade(0, 0.15f);
    }

    // When starting to drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!enabled)
            return;

        if (m_IsDragging)
            return;

        m_IsDragging = true;
        puzzleController.activeDragCount++;
        m_OriginalPosition = m_RectTransform.anchoredPosition;
        m_CanvasGroup.blocksRaycasts = false;

        transform.SetAsLastSibling();
    }

    // During drag
    public void OnDrag(PointerEventData eventData)
    {
        if (!enabled)
            return;

        if (!m_IsDragging)
            return;

        // Convert the mouse's global position to a position relative to the PuzzleController's RectTransform
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(puzzleController.GetComponent<RectTransform>(),
                eventData.position, eventData.pressEventCamera, out var localPoint))
            m_RectTransform.anchoredPosition = localPoint;
    }

    // When ending the drag
    // When ending the drag
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!enabled)
            return;

        if (!m_IsDragging)
            return;

        puzzleController.activeDragCount--;
        m_CanvasGroup.blocksRaycasts = true;

        // Check for objects underneath using UI system
        var hitObjects = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hitObjects);

        var targetPieceController =
            (from result in hitObjects
                where result.gameObject != gameObject && result.gameObject.GetComponent<PuzzlePieceController>()
                select result.gameObject.GetComponent<PuzzlePieceController>()).FirstOrDefault();

        // Loop through hit objects to find the first PuzzlePieceController that's not this one

        Debug.Log(
            $"targetPieceController: {targetPieceController}; m_IsDragging: {targetPieceController?.m_IsDragging}");
        if (targetPieceController != null && !targetPieceController.m_IsDragging)
        {
            // Swap positions with tween
            m_RectTransform.DOAnchorPos(targetPieceController.m_RectTransform.anchoredPosition, 0.3f);

            targetPieceController.m_IsDragging = true;
            targetPieceController.m_RectTransform.DOAnchorPos(m_OriginalPosition, 0.3f).onComplete +=
                () =>
                {
                    m_IsDragging = false;
                    targetPieceController.m_IsDragging = false;
                };

            // swap puzzlePiece.currentPosition with targetPieceController.puzzlePiece.currentPosition
            (puzzlePiece.currentPosition, targetPieceController.puzzlePiece.currentPosition) =
                (targetPieceController.puzzlePiece.currentPosition, puzzlePiece.currentPosition);

            targetPieceController.transform.SetAsLastSibling();
            transform.SetAsLastSibling();

            glowCanvasGroup.DOFade(0, 0.15f);
            targetPieceController.glowCanvasGroup.DOFade(0, 0.15f);
        }
        else
        {
            // If released outside the puzzle area, tween the piece back to its original position
            m_RectTransform.DOAnchorPos(m_OriginalPosition, 0.3f).onComplete += () => m_IsDragging = false;
        }

        // Check if the puzzle is solved
        puzzleController.TriggerIsSolvedDetection();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        glowCanvasGroup.DOFade(0.7f, 0.15f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!enabled)
            return;
        glowCanvasGroup.DOFade(1, 0.15f);

        if (puzzleController.activeDragCount <= 0)
            transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!m_IsDragging) glowCanvasGroup.DOFade(0, 0.15f);
    }
}