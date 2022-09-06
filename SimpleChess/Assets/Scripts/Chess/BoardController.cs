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
        
        public GameObject AddPiece(GameObject piece, int col, int row)
        {
            var gridPoint = Geometry.GridPoint(col, row);
            var newPiece = LeanPool.Spawn(piece, Geometry.PointFromGrid(gridPoint), Quaternion.identity, gameObject.transform);
            _pieceObjectList.Add(newPiece);
            return newPiece;
        }

        public void RemovePiece(GameObject piece)
        {
            LeanPool.Despawn(piece);
        }

        public void MovePiece(GameObject piece, Vector2Int gridPoint)
        {
            Debug.LogError("Move : "+piece.transform.name +  " : " +gridPoint.x + " , " + gridPoint.y);
            piece.transform.position = Geometry.PointFromGrid(gridPoint);
        }

        public void SelectPiece(GameObject piece)
        {
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
            foreach (var pieceObject in _pieceObjectList) LeanPool.Despawn(pieceObject);
            _pieceObjectList.Clear();
        }
    }
}
