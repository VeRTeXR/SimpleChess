using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Chess.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        public bool IsPlayerPiece;
        protected PieceType Type;

        protected Vector2Int[] RookDirections = {new Vector2Int(0,1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0)};
        protected Vector2Int[] BishopDirections = {new Vector2Int(1,1), new Vector2Int(1, -1),
            new Vector2Int(-1, -1), new Vector2Int(-1, 1)};

        public abstract List<Vector2Int> MoveLocations(Vector2Int gridPoint);
    }
}
