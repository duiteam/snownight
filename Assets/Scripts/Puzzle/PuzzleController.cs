using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePiece
{
    public PuzzlePiece(Sprite sprite, Vector2Int expectedPosition)
    {
        this.sprite = sprite;
        this.expectedPosition = expectedPosition;
        currentPosition = new Vector2Int(expectedPosition.x, expectedPosition.y);
    }

    public Sprite sprite { get; }
    public Vector2Int expectedPosition { get; }
    public Vector2Int currentPosition { get; set; }
}

public class Puzzle
{
    public Puzzle(string spritePath, Vector2 size)
    {
        this.size = size;

        // load all sprites under the spritePath, and sort them by name (alphabetically)
        var sprites = Resources.LoadAll<Sprite>($"Sprites/PuzzlePieces/{spritePath}");
        Array.Sort(sprites, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));

        // create a list of PuzzlePieces
        pieces = new List<PuzzlePiece>();
        for (var i = 0; i < sprites.Length; i++)
            pieces.Add(new PuzzlePiece(sprites[i], new Vector2Int(i % (int)size.x, i / (int)size.x)));

        ShuffleCurrentPositions();
    }

    // Size of the puzzle in pieces
    public Vector2 size { get; }

    // The PuzzlePieces
    public List<PuzzlePiece> pieces { get; }

    // GetPosition is a function that returns the position of a puzzle piece
    // given a bounding box and a "position", which is a Vector2Int that represents
    // the position of the puzzle piece in the puzzle.
    // Note that the pivot is in the center of the puzzle piece
    public Vector2 GetPosition(Rect boundingBox, Vector2Int position)
    {
        var unitSize = new Vector2(boundingBox.width / size.x, boundingBox.height / size.y);
        return new Vector2(
            boundingBox.x + unitSize.x * (position.x + 0.5f),
            boundingBox.y + unitSize.y * (size.y - position.y - 0.5f)
        );
    }

    private void ShuffleCurrentPositions()
    {
        var shuffledPieces =
            new List<Vector2Int>(
                pieces.Select(piece => new Vector2Int(piece.currentPosition.x, piece.currentPosition.y)));
        shuffledPieces.Shuffle();

        for (var i = 0; i < pieces.Count; i++) pieces[i].currentPosition = shuffledPieces[i];
    }

    public bool IsSolved()
    {
        return pieces.All(piece => piece.currentPosition == piece.expectedPosition);
    }
}

public class Puzzles
{
    public static readonly Puzzles instance = new Puzzles();

    private readonly Dictionary<string, Puzzle> puzzles = new Dictionary<string, Puzzle>
    {
        { "Test", new Puzzle("TestPuzzle", new Vector2(3, 3)) }
    };

    public Puzzle Get(string name)
    {
        return puzzles[name];
    }
}

public class PuzzleController : MonoBehaviour
{
    // an OnPuzzleSolved trigger that will be called when the puzzle is solved
    public delegate void PuzzleSolvedDelegate();

    public string puzzleConfigName;

    private CanvasGroup m_GlowLargeCanvasGroup;

    private RectTransform m_RectTransform;

    public Puzzle puzzle { get; private set; }

    public int activeDragCount { get; set; }

    // Start is called before the first frame update
    private void Start()
    {
        OnPuzzleSolved += () => Debug.Log("Puzzle solved!");
        puzzle = Puzzles.instance.Get(puzzleConfigName);
        m_RectTransform = GetComponent<RectTransform>();
        m_GlowLargeCanvasGroup = transform.Find("GlowLarge").gameObject.GetComponent<CanvasGroup>();

        SummonChildPuzzlePieces();
    }

    public event PuzzleSolvedDelegate OnPuzzleSolved;

    private void SummonChildPuzzlePieces()
    {
        var puzzlePiecePrefab = Resources.Load<GameObject>("Puzzle/PuzzlePiece");
        var unitSize = new Vector2(m_RectTransform.rect.width / puzzle.size.x,
            m_RectTransform.rect.height / puzzle.size.y);

        foreach (var puzzlePiece in puzzle.pieces)
        {
            var puzzlePieceGameObject = Instantiate(puzzlePiecePrefab, transform);
            puzzlePieceGameObject.GetComponent<Image>().sprite = puzzlePiece.sprite;
            puzzlePieceGameObject.name = $"PuzzlePiece ({puzzlePiece.sprite.name})";
            puzzlePieceGameObject.transform.SetParent(transform);
            puzzlePieceGameObject.transform.localScale = Vector3.one;

            var position = puzzle.GetPosition(m_RectTransform.rect, puzzlePiece.currentPosition);
            puzzlePieceGameObject.transform.localPosition = position;

            var puzzleRectTransform = puzzlePieceGameObject.GetComponent<RectTransform>();
            puzzleRectTransform.sizeDelta = unitSize;

            var controller = puzzlePieceGameObject.GetComponent<PuzzlePieceController>();
            controller.puzzleController = this;
            controller.puzzlePiece = puzzlePiece;
        }
    }

    private void StartGlowLarge()
    {
        m_GlowLargeCanvasGroup.DOFade(1, 1f);
    }

    public void TriggerIsSolvedDetection()
    {
        if (puzzle.IsSolved())
        {
            OnPuzzleSolved?.Invoke();
            Invoke(nameof(StartGlowLarge), 0.5f);

            // list all puzzle piece game objects under this puzzle controller
            var puzzlePieceGameObjects = (from Transform child in transform
                select child.GetComponent<PuzzlePieceController>()
                into puzzlePieceController
                where puzzlePieceController != null
                select puzzlePieceController).ToList();

            // disable all puzzle piece controllers
            foreach (var puzzlePieceController in puzzlePieceGameObjects) puzzlePieceController.enabled = false;
        }
        else
        {
            Debug.Log("Puzzle not solved yet");
        }
    }
}