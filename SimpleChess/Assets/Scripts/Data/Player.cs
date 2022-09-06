using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class Player
    {
        public List<GameObject> Pieces;
        public List<GameObject> CapturedPieces;

        public string Name;
        public int ForwardZValue;

        public Player(string name, bool forwardZValue)
        {
            Name = name;
            Pieces = new List<GameObject>();
            CapturedPieces = new List<GameObject>();

            if (forwardZValue)
                ForwardZValue = 1;
            else
                ForwardZValue = -1;
        }
    }
}
