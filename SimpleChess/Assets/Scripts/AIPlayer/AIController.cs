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

        private void Awake()
        {
            Signaler.Instance.Subscribe<StartEnemyTurn>(this, OnEnemyTurnStarted);
        }

        private bool OnEnemyTurnStarted(StartEnemyTurn signal)
        {
            var allActivePieces = GameManager.Instance.GetActiveChessPiece();
            _enemyActivePiece = allActivePieces.Where(piece => !piece.IsPlayerPiece).ToList();
            
            //TODO:: Use minimax and alphabeta pruning for decision making 
            RandomlySelectPiece();
            GameManager.Instance.SelectPiece(_selectedPiece.gameObject);
            _movableCoordinateList = GameManager.Instance.MovesForPiece(_selectedPiece.gameObject);
            AnimateMovingSelectedPiece(_selectedPiece);
            return true;
        }

        private void RandomlySelectPiece()
        {
            var randRng = Random.Range(0, _enemyActivePiece.Count);
            _selectedPiece = _enemyActivePiece[randRng];
        }

        private void AnimateMovingSelectedPiece(Piece piece)
        {
            if (_movableCoordinateList.Count > 0)
            {
                var moveRng = Random.Range(0, _movableCoordinateList.Count);
                if (GameManager.Instance.GetPieceAtGrid(_movableCoordinateList[moveRng]) == null)
                {
                    GameManager.Instance.AIMove(piece, _movableCoordinateList[moveRng]);
                    GameManager.Instance.DeselectPiece(_selectedPiece.gameObject);
                }
                else
                {
                    GameManager.Instance.CapturePieceAt(_movableCoordinateList[moveRng]);
                    GameManager.Instance.AIMove(piece, _movableCoordinateList[moveRng]);
                    GameManager.Instance.DeselectPiece(_selectedPiece.gameObject);
                }
            }
            _movableCoordinateList.Clear();
            _selectedPiece = null;
            GameManager.Instance.NextPlayer();
            // Signaler.Instance.Broadcast(this, new FinishEnemyTurn());
        }
    }
}
