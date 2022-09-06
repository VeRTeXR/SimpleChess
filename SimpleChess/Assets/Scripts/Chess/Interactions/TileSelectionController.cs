using System;
using Chess.Pieces;
using echo17.Signaler.Core;
using Lean.Pool;
using TMPro;
using UI;
using UI.Summary;
using UnityEngine;
using Utilities;

namespace Chess.Interactions
{
    public class TileSelectionController: MonoBehaviour, ISubscriber
    {
        [SerializeField] private GameObject tileHighlightPrefab;

        private GameObject _tileHighlightInstance;
        private Camera _mainCamera;
        private MoveActionController _moveActionController;
        private bool _isActive;

        private void Awake()
        {
            Signaler.Instance.Subscribe<InitializeGameSession>(this, OnGameSessionInitialized);
            Signaler.Instance.Subscribe<RestartSession>(this, OnRestartSession);
            Signaler.Instance.Subscribe<GameOver>(this, OnGameOver);
        }

        private bool OnRestartSession(RestartSession signal)
        {
            _isActive = true;
            return true;
        }

        private bool OnGameSessionInitialized(InitializeGameSession signal)
        {
            _isActive = true;
            return true;
        }

        private bool OnGameOver(GameOver signal)
        {
            _isActive = false;
            return true;
        }


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
            if (!_isActive) return;
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
                    if (selectedPiece == null) return;
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

     }
}