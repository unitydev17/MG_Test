using UnityEngine;

namespace CargoMover
{
    public static class Utils
    {
        public static void SetLocalY(this Transform tr, float posY)
        {
            var pos = tr.localPosition;
            pos.y = posY;
            tr.localPosition = pos;
        }

        public static void Clean(this Collider[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = null;
            }
        }
    }
}