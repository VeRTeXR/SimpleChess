using System.Collections.Generic;
using Chess.Pieces;
using Lean.Pool;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

namespace Chess.Interactions
{
    public class MoveActionController : MonoBehaviour
    {
        [SerializeField] public GameObject moveLocationPrefab;
        [SerializeField] public GameObject tileHighlightPrefab;
        [SerializeField] public GameObject attackLocationPrefab;

        private GameObject _tileHighlight;
        private GameObject _movingPiece;
        private List<Vector2Int> _moveLocations;
        private List<GameObject> _locationHighlights;
        private TileSelectionController _tileSelectionController;
        private Camera _mainCamera;

        private void Start ()
        {
            _mainCamera = Camera.main;
            enabled = false;
            _tileSelectionController = GetComponent<TileSelectionController>();
            _tileHighlight = LeanPool.Spawn(tileHighlightPrefab, Geometry.PointFromGrid(new Vector2Int(0, 0)),
                Quaternion.identity, gameObject.transform);
            _tileHighlight.SetActive(false);
        }

        private void Update ()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var point = hit.point;
                var gridPoint = Geometry.GridFromPoint(point);

                _tileHighlight.SetActive(true);
                _tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);
                
                if (!Input.GetMouseButtonDown(0)) 
                    return;

                
                var pieceAtGrid = GameManager.Instance.GetPieceAtGrid(gridPoint);
                if (pieceAtGrid == null)
                {
                    if (!_moveLocations.Contains(gridPoint))
                        return;
                    GameManager.Instance.Move(_movingPiece, gridPoint);
                }
                else
                {
                    if (pieceAtGrid == GameManager.Instance.boardController.GetSelectedPiece())
                    {
                        UnselectReturnToSelectPiece();
                        return;
                    }

                    if (pieceAtGrid.GetComponent<Piece>().IsPlayerPiece)
                    {
                        if (UnselectThenSelectValidPiece(pieceAtGrid)) return;
                        return;
                    }
                    if (!_moveLocations.Contains(gridPoint))
                        return;

                    GameManager.Instance.CapturePieceAt(gridPoint);
                    GameManager.Instance.Move(_movingPiece, gridPoint);
                }
                
                ExitState();
            }
            else
            {
                _tileHighlight.SetActive(false);
            }
        }

        private bool UnselectThenSelectValidPiece(GameObject pieceAtGrid)
        {
            CancelMove();
            if (GameManager.Instance.DoesPieceBelongToCurrentPlayer(pieceAtGrid))
            {
                GameManager.Instance.SelectPiece(pieceAtGrid);
                EnterState(pieceAtGrid);

                return true;
            }

            return false;
        }

        private void UnselectReturnToSelectPiece()
        {
            enabled = false;
            CancelMove();
            _tileSelectionController.EnterState();
        }


        private void CancelMove()
        {
            foreach (var highlight in _locationHighlights) LeanPool.Despawn(highlight);
            GameManager.Instance.DeselectPiece(_movingPiece);
            
        }

        public void EnterState(GameObject piece)
        {
            _movingPiece = piece;
            enabled = true;

            _moveLocations = GameManager.Instance.MovesForPiece(_movingPiece);
            _locationHighlights = new List<GameObject>();

            if (_moveLocations.Count == 0) 
                CancelMove();

            foreach (var loc in _moveLocations)
            {
                GameObject highlight;
                if (GameManager.Instance.GetPieceAtGrid(loc))
                    highlight = LeanPool.Spawn(attackLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity,
                        gameObject.transform);
                else
                    highlight = LeanPool.Spawn(moveLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity,
                        gameObject.transform);
                _locationHighlights.Add(highlight);
            }
        }

        private void ExitState()
        {
            enabled = false;
            _tileHighlight.SetActive(false);
            GameManager.Instance.DeselectPiece(_movingPiece);
            _movingPiece = null;
            GameManager.Instance.NextPlayer();
            _tileSelectionController.EnterState();
            
            foreach (var highlight in _locationHighlights) LeanPool.Despawn(highlight);
        }

    }
}
