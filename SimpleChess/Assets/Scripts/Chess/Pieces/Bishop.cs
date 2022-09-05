using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Chess.Pieces
{
    public class Bishop : Piece
    {
        private void Start()
        {
            Type = PieceType.Bishop;
        }

        public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
        {
            var locations = new List<Vector2Int>();

            foreach (var dir in BishopDirections)
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
