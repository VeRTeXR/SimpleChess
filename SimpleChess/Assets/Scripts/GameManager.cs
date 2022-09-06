using System.Collections.Generic;
using AIPlayer;
using Chess;
using Chess.Interactions;
using Chess.Pieces;
using Data;
using echo17.Signaler.Core;
using Lean.Pool;
using UI;
using UI.GameState;
using UI.Summary;
using UnityEngine;
using UnityEngine.Timeline;

public class GameManager : MonoBehaviour, IBroadcaster, ISubscriber
{
    public static GameManager Instance;

    public BoardController boardController;

    [Header("Pieces References")] 
    [SerializeField] private GameObject whiteKing;
    [SerializeField] private GameObject whiteQueen;
    [SerializeField] private GameObject whiteBishop;
    [SerializeField] private GameObject whiteKnight;
    [SerializeField] private GameObject whiteRook;
    [SerializeField] private GameObject whitePawn;

    [SerializeField] private GameObject blackKing;
    [SerializeField] private GameObject blackQueen;
    [SerializeField] private GameObject blackBishop;
    [SerializeField] private GameObject blackKnight;
    [SerializeField] private GameObject blackRook;
    [SerializeField] private GameObject blackPawn;

    private GameObject[,] _pieces = new GameObject[8, 8];
    private List<GameObject> _movedPawns = new List<GameObject>();

    private Player _white;
    private Player _black;
    public Player CurrentPlayer;
    private Player _otherPlayer;
    private TileSelectionController _tileSelectionController;
    private MoveActionController _moveActionController;
    private List<Piece> _activePieces = new List<Piece>();
    public Player OtherPlayer => _otherPlayer;


    private void Awake()
    {
        if(Instance != null) 
            Destroy(Instance);
        Instance = this;

        Signaler.Instance.Subscribe<InitializeGameSession>(this, OnInitializeGameSession);
        Signaler.Instance.Subscribe<FinishEnemyTurn>(this, OnEnemyTurnFinished);
        Signaler.Instance.Subscribe<RestartSession>(this, OnRestartGameSession);

    }

    private bool OnRestartGameSession(RestartSession signal)
    {
        _pieces = new GameObject[8, 8];
        _movedPawns.Clear();
        _activePieces.Clear();
        boardController.Clear();
        if (_tileSelectionController == null)
            _tileSelectionController = boardController.GetComponent<TileSelectionController>();
        if (_moveActionController == null)
            _moveActionController = boardController.GetComponent<MoveActionController>();

        _white = new Player("white", true);
        _black = new Player("black", false);
        CurrentPlayer = _white;
        _otherPlayer = _black;

        InitializeGame();

        Signaler.Instance.Broadcast(this, new StartPlayerTurn());

        return true;
    }

    private bool OnInitializeGameSession(InitializeGameSession signal)
    {
        _pieces = new GameObject[8, 8];
        _movedPawns.Clear();
        _activePieces.Clear();
        boardController.Clear();
        if (_tileSelectionController == null)
            _tileSelectionController = boardController.GetComponent<TileSelectionController>();
        if (_moveActionController == null)
            _moveActionController = boardController.GetComponent<MoveActionController>();

        _white = new Player("white", true);
        _black = new Player("black", false);
        CurrentPlayer = _white;
        _otherPlayer = _black;

        InitializeGame();

        Signaler.Instance.Broadcast(this, new StartPlayerTurn());

        return true;
        
    }

    private bool OnEnemyTurnFinished(FinishEnemyTurn signal)
    {
        return true;
    }


    private void InitializeGame()
    {
        InitializePlayerPieces();
        InitializeEnemyPieces();
    }

    private void InitializeEnemyPieces()
    {
        AddPiece(blackRook, _black, 0, 7);
        AddPiece(blackKnight, _black, 1, 7);
        AddPiece(blackBishop, _black, 2, 7);
        AddPiece(blackQueen, _black, 3, 7);
        AddPiece(blackKing, _black, 4, 7);
        AddPiece(blackBishop, _black, 5, 7);
        AddPiece(blackKnight, _black, 6, 7);
        AddPiece(blackRook, _black, 7, 7);

        for (var i = 0; i < 8; i++) AddPiece(blackPawn, _black, i, 6);
    }

    private void InitializePlayerPieces()
    {
        AddPiece(whiteRook, _white, 0, 0);
        AddPiece(whiteKnight, _white, 1, 0);
        AddPiece(whiteBishop, _white, 2, 0);
        AddPiece(whiteQueen, _white, 3, 0);
        AddPiece(whiteKing, _white, 4, 0);
        AddPiece(whiteBishop, _white, 5, 0);
        AddPiece(whiteKnight, _white, 6, 0);
        AddPiece(whiteRook, _white, 7, 0);

        for (var i = 0; i < 8; i++) AddPiece(whitePawn, _white, i, 1);
    }

    private void AddPiece(GameObject prefab, Player player, int col, int row)
    {
        var pieceObject = boardController.AddPiece(prefab, col, row);
        player.Pieces.Add(pieceObject);
        _pieces[col, row] = pieceObject;
        _activePieces.Add(pieceObject.GetComponent<Piece>());
    }

    public void SelectPieceAtGrid(Vector2Int gridPoint)
    {
        var selectedPiece = _pieces[gridPoint.x, gridPoint.y];
        if (selectedPiece != null) boardController.SelectPiece(selectedPiece);
    }

    public List<Vector2Int> MovesForPiece(GameObject pieceObject)
    {
        var piece = pieceObject.GetComponent<Piece>();
        var gridPoint = GetPieceCurrentGridCoordinate(pieceObject);
        var locations = piece.MoveLocations(gridPoint);
        locations.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);
        locations.RemoveAll(gp => IsFriendlyPieceAt(gp));

        return locations;
    }



    public void Move(GameObject piece, Vector2Int gridPoint)
    {
        var pieceComponent = piece.GetComponent<Piece>();
        if (pieceComponent is Pawn && !HasPawnMoved(piece)) 
            _movedPawns.Add(piece);

        var startGridPoint = GetPieceCurrentGridCoordinate(piece);
        _pieces[startGridPoint.x, startGridPoint.y] = null;
        _pieces[gridPoint.x, gridPoint.y] = piece;
        boardController.MovePiece(piece, gridPoint);        
    }

    public void AIMove(Piece piece, Vector2Int targetGridPoint)
    {
        if (piece is Pawn && !HasPawnMoved(piece.gameObject)) 
            _movedPawns.Add(piece.gameObject);

        var startGridPoint = GetPieceCurrentGridCoordinate(piece.gameObject);
        if (startGridPoint.x < 0 && startGridPoint.y < 0)
            return;
        _pieces[startGridPoint.x, startGridPoint.y] = null;
        
        _pieces[Mathf.Clamp(targetGridPoint.x, 0, 7), Mathf.Clamp(targetGridPoint.y, 0, 7)] = piece.gameObject;
        boardController.MovePiece(piece.gameObject, targetGridPoint);    

    }

    public void PawnMoved(GameObject pawn)
    {
        _movedPawns.Add(pawn);
    }

    public bool HasPawnMoved(GameObject pawn)
    {
        return _movedPawns.Contains(pawn);
    }
    
    public void SelectPiece(GameObject piece)
    {
        boardController.SelectPiece(piece);
    }

    public void DeselectPiece(GameObject piece)
    {
        boardController.DeselectPiece(piece);
    }

    public bool DoesPieceBelongToCurrentPlayer(GameObject pieceObject)
    {
        var pieceComponent = pieceObject.GetComponent<Piece>();
        return CurrentPlayer.Pieces.Contains(pieceObject) && pieceComponent.IsPlayerPiece;
    }

    public GameObject GetPieceAtGrid(Vector2Int gridPoint)
    {
        if (gridPoint.x > 7 || gridPoint.y > 7 || gridPoint.x < 0 || gridPoint.y < 0)
        {
            return null;
        }
        return _pieces[gridPoint.x, gridPoint.y];
    }

    private Vector2Int GetPieceCurrentGridCoordinate(GameObject piece)
    {
        for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
                if (_pieces[i, j] == piece)
                    return new Vector2Int(i, j);

        return new Vector2Int(-1, -1);
    }

    private bool IsFriendlyPieceAt(Vector2Int gridPoint)
    {
        var piece = GetPieceAtGrid(gridPoint);

        if (piece == null)
            return false;

        if (_otherPlayer.Pieces.Contains(piece))
            return false;

        return true;
    }

    public void NextPlayer()
    {
        var tempPlayer = CurrentPlayer;
        CurrentPlayer = _otherPlayer;
        _otherPlayer = tempPlayer;

        if (CurrentPlayer.Name == "black")
            Signaler.Instance.Broadcast(this, new StartEnemyTurn());
        else
            Signaler.Instance.Broadcast(this, new StartPlayerTurn());
    }

    public void CapturePieceAt(Vector2Int gridPoint)
    {
        var capturedPieceObject = GetPieceAtGrid(gridPoint);
        var capturedPiece = capturedPieceObject.GetComponent<Piece>();
        if (capturedPiece is King) 
            Signaler.Instance.Broadcast(this, new GameOver {WonPlayerData = CurrentPlayer});

        _activePieces.Remove(capturedPiece);
        
        CurrentPlayer.CapturedPieces.Add(capturedPieceObject);
        _pieces[gridPoint.x, gridPoint.y] = null;
        LeanPool.Links[capturedPieceObject].Despawn(capturedPieceObject);
    }

    public List<Piece> GetActiveChessPiece()
    {
        return _activePieces;
    }

    public GameObject GetCurrentSelectedPiece()
    {
        return boardController.GetSelectedPiece();
    }
}