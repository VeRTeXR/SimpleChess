using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Pieces;
using echo17.Signaler.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AIPlayer
{
    public class AIController : MonoBehaviour, ISubscriber, IBroadcaster
    {
        private List<Piece> _enemyActivePiece = new List<Piece>();
        private List<Vector2Int> _movableCoordinateList = new List<Vector2Int>();
        private Piece _selectedPiece;
        [SerializeField] private float minDelay = 0.5f;
        [SerializeField] private float maxDelay = 1.5f;
        private bool _isGameOver;

        private void Awake()
        {
            Signaler.Instance.Subscribe<GameOver>(this, OnGameOver);
            Signaler.Instance.Subscribe<StartEnemyTurn>(this, OnEnemyTurnStarted);
        }

        private bool OnGameOver(GameOver signal)
        {
            _isGameOver = true;
            return true;
        }

        public void Initialize()
        {
            _isGameOver = false;
        }
        
        private bool OnEnemyTurnStarted(StartEnemyTurn signal)
        {
            if (_isGameOver) return false;
            var allActivePieces = GameManager.Instance.GetActiveChessPiece();
            _enemyActivePiece = allActivePieces.Where(piece => !piece.IsPlayerPiece).ToList();
            
            //TODO:: Use minimax and alpha beta pruning for decision making 
            RandomlySelectPiece();
            GameManager.Instance.SelectPiece(_selectedPiece.gameObject);
            _movableCoordinateList = GameManager.Instance.MovesForPiece(_selectedPiece.gameObject);
            StartAiPieceSelectionSequence(_selectedPiece);
            
            
            return true;
        }

        private void RandomlySelectPiece()
        {
            var randRng = Random.Range(0, _enemyActivePiece.Count);
            _selectedPiece = _enemyActivePiece[randRng];
        }

        private void StartAiPieceSelectionSequence(Piece piece)
        {
            var seq = LeanTween.sequence();
            var enemyActionDelay = Random.Range(minDelay, maxDelay);
            seq.append(enemyActionDelay);
            seq.append(
                () =>
                {
                    if (_movableCoordinateList.Count > 0)
                    {
                        var moveRng = Random.Range(0, _movableCoordinateList.Count);
                        if (GameManager.Instance.GetPieceAtGrid(_movableCoordinateList[moveRng]) == null)
                        {
                            GameManager.Instance.AIMove(piece, _movableCoordinateList[moveRng]);
                        }
                        else
                        {
                            GameManager.Instance.CapturePieceAt(_movableCoordinateList[moveRng]);
                            GameManager.Instance.AIMove(piece, _movableCoordinateList[moveRng]);
                        }
                    }
                    else
                    {
                        ExitEnemyTurn();
                        return;
                    }

                    ExitEnemyTurn();
                }
            );
        }

        private void ExitEnemyTurn()
        {
            GameManager.Instance.DeselectPiece(_selectedPiece.gameObject);
            _movableCoordinateList.Clear();
            _selectedPiece = null;
            GameManager.Instance.NextPlayer();
            Signaler.Instance.Broadcast(this, new FinishEnemyTurn());
        }
    }
}
