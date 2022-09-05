using UnityEngine;
using Utilities;

namespace Chess
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private  Material defaultMaterial;
        [SerializeField] private Material selectedMaterial;

        
        public GameObject AddPiece(GameObject piece, int col, int row)
        {
            // TODO :: Update to LeanPool function here!
            Vector2Int gridPoint = Geometry.GridPoint(col, row);
            var newPiece = Instantiate(piece, Geometry.PointFromGrid(gridPoint), Quaternion.identity, gameObject.transform);
            return newPiece;
        }

        public void RemovePiece(GameObject piece)
        {
            Destroy(piece);
        }

        public void MovePiece(GameObject piece, Vector2Int gridPoint)
        {
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
    }
}
