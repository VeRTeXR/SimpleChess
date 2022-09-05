using Lean.Pool;
using TMPro;
using UnityEngine;
using Utilities;

namespace Chess.Interactions
{
    public class TileSelectionController: MonoBehaviour
    {
        [SerializeField] private GameObject tileHighlightPrefab;

        private GameObject _tileHighlightInstance;
        private Camera _mainCamera;
        private MoveActionController _moveActionController;

        private void Start ()
        {
            _mainCamera = Camera.main;
            _moveActionController = GetComponent<MoveActionController>();
            
            var gridPoint = Geometry.GridPoint(0, 0);
            var point = Geometry.PointFromGrid(gridPoint);
            _tileHighlightInstance = LeanPool.Spawn(tileHighlightPrefab, point, Quaternion.identity, gameObject.transform);
            _tileHighlightInstance.SetActive(false);
        }

        private void Update ()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var point = hit.point;
                var gridPoint = Geometry.GridFromPoint(point);

                _tileHighlightInstance.SetActive(true);
                _tileHighlightInstance.transform.position = Geometry.PointFromGrid(gridPoint);
                if (Input.GetMouseButtonDown(0))
                {
                    var selectedPiece = GameManager.Instance.GetPieceAtGrid(gridPoint);
                    if (GameManager.Instance.DoesPieceBelongToCurrentPlayer(selectedPiece))
                    {
                        GameManager.Instance.SelectPiece(selectedPiece);
                        ExitState(selectedPiece);
                    }
                }
            }
            else
            {
                _tileHighlightInstance.SetActive(false);
            }
        }

        public void EnterState()
        {
            enabled = true;
        }

        private void ExitState(GameObject movingPiece)
        {
            enabled = false;
            _tileHighlightInstance.SetActive(false);
            _moveActionController.EnterState(movingPiece);
        }

        public void GameOver()
        {
            enabled = false;
        }
    }
}