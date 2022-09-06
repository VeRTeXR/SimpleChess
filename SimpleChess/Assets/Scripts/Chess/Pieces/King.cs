using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Chess.Pieces
{
    public class King : Piece
    {
        private void Start()
        {
            Type = PieceType.King;
        }
        
        public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
        {   
            var locations = new List<Vector2Int>();
            var directions = new List<Vector2Int>(BishopDirections);
            directions.AddRange(RookDirections);

            foreach (var dir in directions)
            {
                var nextGridPoint = new Vector2Int(gridPoint.x + dir.x, gridPoint.y + dir.y);
                locations.Add(nextGridPoint);
            }

            return locations;
        }
    }
}
