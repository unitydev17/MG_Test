using UnityEngine;

namespace CargoMover
{
    public class Constants
    {
        public const string Cargo_Layer = "Cargo";
        public const string Player_Layer = "Player";
        public const string Placeholder_Layer = "Placeholder";

        public const float TouchRadius = 0.25f;
        public const float SearchRadius = 1f;
        public const float CargoRadius = 0.5f;
        public const float CargoSpaceRadius = 1.5f;

        public static readonly Vector3 CargoHeight = Vector3.up * 1.5f;
    }
}