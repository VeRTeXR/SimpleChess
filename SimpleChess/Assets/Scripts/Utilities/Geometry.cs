using UnityEngine;

namespace Utilities
{
    public static class Geometry
    {
        public static Vector3 PointFromGrid(Vector2Int gridPoint)
        {
            var x = -3.5f + 1.0f * gridPoint.x;
            var z = -3.5f + 1.0f * gridPoint.y;
            return new Vector3(x, 0, z);
        }

        public static Vector2Int GridPoint(int column, int row)
        {
            return new Vector2Int(column, row);
        }

        public static Vector2Int GridFromPoint(Vector3 point)
        {
            var column = Mathf.FloorToInt(4.0f + point.x);
            var row = Mathf.FloorToInt(4.0f + point.z);
            return new Vector2Int(column, row);
        }
    }
}