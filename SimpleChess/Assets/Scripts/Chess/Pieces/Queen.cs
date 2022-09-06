using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Chess.Pieces
{
    public class Queen : Piece
    {
        private void Start()
        {
            Type = PieceType.Queen;
        }

        public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
        {
            var locations = new List<Vector2Int>();
            var directions = new List<Vector2Int>(BishopDirections);
            directions.AddRange(RookDirections);

            foreach (var dir in directions)
            {
                for (var i = 1; i < 8; i++)
                {
                    var nextGridPoint = new Vector2Int(gridPoint.x + i * dir.x, gridPoint.y + i * dir.y);
                    locations.Add(nextGridPoint);
                    if (GameManager.Instance.GetPieceAtGrid(nextGridPoint))
                        break;
                }
            }

            return locations;
        }
    }
}
