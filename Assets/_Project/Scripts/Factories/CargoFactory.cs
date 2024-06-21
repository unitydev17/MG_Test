using UnityEngine;

namespace CargoMover
{
    public class CargoFactory
    {
        private readonly Cargo _prefab;

        public CargoFactory(Cargo prefab)
        {
            _prefab = prefab;
        }

        public Cargo Create(Vector3 position)
        {
            var cargo = Create();
            cargo.transform.position = position;
            return cargo;
        }

        public Cargo Create()
        {
            var go = Object.Instantiate(_prefab);
            return go.GetComponent<Cargo>();
        }
    }
}