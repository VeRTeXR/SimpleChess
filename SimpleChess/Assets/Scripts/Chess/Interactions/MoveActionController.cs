using System.Collections.Generic;
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

        private void Start ()
        {
            enabled = false;
            _tileSelectionController = GetComponent<TileSelectionController>();
            //TODO :: get from pool
            _tileHighlight = Instantiate(tileHighlightPrefab, Geometry.PointFromGrid(new Vector2Int(0, 0)),
                Quaternion.identity, gameObject.transform);
            _tileHighlight.SetActive(false);
        }

        private void Update ()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var point = hit.point;
                var gridPoint = Geometry.GridFromPoint(point);

                _tileHighlight.SetActive(true);
                _tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);
                if (!Input.GetMouseButtonDown(0)) 
                    return;
                if (!_moveLocations.Contains(gridPoint))
                    return;

                if (GameManager.Instance.GetPieceAtGrid(gridPoint) == null)
                    GameManager.Instance.Move(_movingPiece, gridPoint);
                else
                {
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

        private void CancelMove()
        {
            enabled = false;

            foreach (var highlight in _locationHighlights)
            {
                //TODO:: return to pool 
                Destroy(highlight);
            }

            GameManager.Instance.DeselectPiece(_movingPiece);
            _tileSelectionController.EnterState();
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
                //TODO :: Update to pool object
                if (GameManager.Instance.GetPieceAtGrid(loc))
                    highlight = Instantiate(attackLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity,
                        gameObject.transform);
                else
                    highlight = Instantiate(moveLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity,
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
            foreach (var highlight in _locationHighlights)
            { 
                //TODO :: return object to pool 
                Destroy(highlight);
            }
        }

        public void GameOver()
        {
            enabled = false;
        }
    }
}
