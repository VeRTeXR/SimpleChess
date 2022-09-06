using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using Utilities;

namespace Chess
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material selectedMaterial;

        private List<GameObject> _pieceObjectList = new List<GameObject>();
        private GameObject _currentSelectedPiece;

        public GameObject AddPiece(GameObject piece, int col, int row)
        {
            var gridPoint = Geometry.GridPoint(col, row);
            var newPiece = LeanPool.Spawn(piece, Geometry.PointFromGrid(gridPoint), Quaternion.identity, gameObject.transform);
            _pieceObjectList.Add(newPiece);
            return newPiece;
        }

     
        public void MovePiece(GameObject piece, Vector2Int gridPoint)
        {
            LeanTween.move(piece, Geometry.PointFromGrid(gridPoint), 0.2f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(
                () =>
                {
                    piece.transform.position = Geometry.PointFromGrid(gridPoint);
                });           
        }

        public void SelectPiece(GameObject piece)
        {
            _currentSelectedPiece = piece;
            var renderers = piece.GetComponentInChildren<MeshRenderer>();
            renderers.material = selectedMaterial;
        }

        public void DeselectPiece(GameObject piece)
        {
            var renderers = piece.GetComponentInChildren<MeshRenderer>();
            renderers.material = defaultMaterial;
        }

        public void Clear()
        {
            foreach (var pieceObject in _pieceObjectList) RemovePiece(pieceObject);
            _pieceObjectList.Clear();
        }

        private void RemovePiece(GameObject piece)
        {
            LeanPool.Links[piece].Despawn(piece);
        }

        public GameObject GetSelectedPiece()
        {
            return _currentSelectedPiece;
        }
    }
}
