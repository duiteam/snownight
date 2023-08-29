using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;

public class PuzzlePieceController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public PuzzleController puzzleController;
    public PuzzlePiece puzzlePiece;
    public CanvasGroup glowCanvasGroup;
    
    private Vector2 m_OriginalPosition;
    private RectTransform m_RectTransform;
    private CanvasGroup m_CanvasGroup; // To control visibility and raycast target during drag
    private bool m_IsDragging;
    

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
        
        m_IsDragging = true;
        puzzleController.activeDragCount++;
        m_OriginalPosition = m_RectTransform.anchoredPosition;
        m_CanvasGroup.blocksRaycasts = false;
        
        transform.SetAsLastSibling();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        glowCanvasGroup.DOFade(0.7f, 0.15f);
    }

    // During drag
    public void OnDrag(PointerEventData eventData)
    {
        if (!enabled)
            return;
        
        // Convert the mouse's global position to a position relative to the PuzzleController's RectTransform
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(puzzleController.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            m_RectTransform.anchoredPosition = localPoint;
        }
    }

    // When ending the drag
    // When ending the drag
    public void OnEndDrag(PointerEventData eventData)
    {
        m_IsDragging = false;
        puzzleController.activeDragCount--;
        m_CanvasGroup.blocksRaycasts = true;

        // Check for objects underneath using UI system
        var hitObjects = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hitObjects);

        PuzzlePieceController targetPieceController = null;

        // Loop through hit objects to find the first PuzzlePieceController that's not this one
        foreach (var result in hitObjects)
        {
            if (result.gameObject != gameObject && result.gameObject.GetComponent<PuzzlePieceController>())
            {
                targetPieceController = result.gameObject.GetComponent<PuzzlePieceController>();
                break;
            }
        }

        if (targetPieceController != null)
        {
            // Swap positions with tween
            m_RectTransform.DOAnchorPos(targetPieceController.m_RectTransform.anchoredPosition, 0.3f);
            targetPieceController.m_RectTransform.DOAnchorPos(m_OriginalPosition, 0.3f);
            
            // swap puzzlePiece.currentPosition with targetPieceController.puzzlePiece.currentPosition
            (puzzlePiece.currentPosition, targetPieceController.puzzlePiece.currentPosition) =
                (targetPieceController.puzzlePiece.currentPosition, puzzlePiece.currentPosition);

            targetPieceController.transform.SetAsLastSibling();
            transform.SetAsLastSibling();
        }
        else
        {
            // If released outside the puzzle area, tween the piece back to its original position
            m_RectTransform.DOAnchorPos(m_OriginalPosition, 0.3f);
        }
        
        // Check if the puzzle is solved
        puzzleController.TriggerIsSolvedDetection();
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
        if (!m_IsDragging)
        {
            glowCanvasGroup.DOFade(0, 0.15f);
        }
    }

}
