using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Chess.Pieces
{
    public class Knight : Piece
    {
        private void Start()
        {
            Type = PieceType.Knight;
        }
 
        public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
        {
            var locations = new List<Vector2Int>
            {
                new Vector2Int(gridPoint.x - 1, gridPoint.y + 2),
                new Vector2Int(gridPoint.x + 1, gridPoint.y + 2),
                new Vector2Int(gridPoint.x + 2, gridPoint.y + 1),
                new Vector2Int(gridPoint.x - 2, gridPoint.y + 1),
                new Vector2Int(gridPoint.x + 2, gridPoint.y - 1),
                new Vector2Int(gridPoint.x - 2, gridPoint.y - 1),
                new Vector2Int(gridPoint.x + 1, gridPoint.y - 2),
                new Vector2Int(gridPoint.x - 1, gridPoint.y - 2)
            };

            return locations;
        }
    }
}
