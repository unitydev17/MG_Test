using Unity.Netcode;
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
            var instance = Object.Instantiate(_prefab);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();

            return instance.GetComponent<Cargo>();
        }
    }
}