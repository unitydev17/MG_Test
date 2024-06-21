using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CargoMover
{
    public static class LevelGenerator
    {
        public static int GenerateLevel()
        {
            var value = (int) (1 + Random.value * 254);

            Debug.Log(ToBinary(value));
            return value;
        }

        public static string ToBinary(int value)
        {
            return Convert.ToString(value, 2).PadLeft(9, '0');
        }
    }
}