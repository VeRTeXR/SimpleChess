using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Chess.Pieces
{
    public class Rook : Piece
    {
        private void Start()
        {
            Type = PieceType.Rook;
        }
        
        
        public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
        {
            var availableLocation = new List<Vector2Int>();

            foreach (var dir in RookDirections)
            {
                for (var i = 1; i < 8; i++)
                {
                    var nextGridPoint = new Vector2Int(gridPoint.x + i * dir.x, gridPoint.y + i * dir.y);
                    availableLocation.Add(nextGridPoint);
                    if (GameManager.Instance.GetPieceAtGrid(nextGridPoint))
                        break;
                }
            }
            return availableLocation;
        }
    }
}
