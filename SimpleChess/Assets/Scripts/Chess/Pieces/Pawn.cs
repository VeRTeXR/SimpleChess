using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        private void Start()
        {
            Type = PieceType.Pawn;
        }


        public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
        {
            var locations = new List<Vector2Int>();

            var forwardDirection = GameManager.Instance.CurrentPlayer.ForwardZValue;
            var forwardOne = new Vector2Int(gridPoint.x, gridPoint.y + forwardDirection);
            if (GameManager.Instance.GetPieceAtGrid(forwardOne) == false) locations.Add(forwardOne);

            var forwardTwo = new Vector2Int(gridPoint.x, gridPoint.y + 2 * forwardDirection);
            if (GameManager.Instance.HasPawnMoved(gameObject) == false &&
                GameManager.Instance.GetPieceAtGrid(forwardTwo) == false)
                locations.Add(forwardTwo);

            var forwardRight = new Vector2Int(gridPoint.x + 1, gridPoint.y + forwardDirection);
            if (GameManager.Instance.GetPieceAtGrid(forwardRight)) locations.Add(forwardRight);

            var forwardLeft = new Vector2Int(gridPoint.x - 1, gridPoint.y + forwardDirection);
            if (GameManager.Instance.GetPieceAtGrid(forwardLeft)) locations.Add(forwardLeft);

            return locations;

        }
    }
}
