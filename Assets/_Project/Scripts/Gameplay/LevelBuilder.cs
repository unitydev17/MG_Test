using UnityEngine;

namespace CargoMover
{
    public class LevelBuilder
    {
        private readonly CargoFactory _cargoFactory;

        public LevelBuilder(CargoFactory cargoFactory)
        {
            _cargoFactory = cargoFactory;
        }

        public void BuildLevel(int value, PlayArea exampleArea, Transform storePoint)
        {
            var counter = BuildExampleArea(value, exampleArea);
            PlaceFreeCargo(storePoint, counter);
        }

        private int BuildExampleArea(int value, PlayArea exampleArea)
        {
            var places = LevelGenerator.ToBinary(value);

            var counter = 0;
            for (var i = 0; i < places.Length; i++)
            {
                if (places[i] == '0') continue;

                var position = exampleArea.GetPositionById(i);
                var cargo = _cargoFactory.Create(position);
                cargo.restrictToMove.Value = true;
                counter++;
            }

            return counter;
        }

        private void PlaceFreeCargo(Transform storePoint, int counter)
        {
            var half = counter / 2;
            for (; counter > 0; counter--)
            {
                var cargo = _cargoFactory.Create();
                cargo.transform.position = storePoint.position + Vector3.right * ((counter - half - 1) * Constants.CargoSpaceRadius);
                cargo.transform.rotation = Quaternion.Euler(0, Random.value * 90, 0);
            }
        }
    }
}