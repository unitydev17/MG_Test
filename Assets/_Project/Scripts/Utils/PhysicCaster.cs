using HelloWorld.Utils;
using UnityEngine;
using VContainer.Unity;

namespace CargoMover
{
    public class PhysicCaster : IInitializable
    {
        private Collider[] _colliders;
        private int _placeholderMask;
        private int _moveMask;
        private int _cargoMask;

        public void Initialize()
        {
            CreateMasks();
            _colliders = new Collider[10];
        }


        private void CreateMasks()
        {
            _placeholderMask = LayerMask.GetMask(Constants.Placeholder_Layer);
            _moveMask = LayerMask.GetMask(Constants.Cargo_Layer, Constants.Player_Layer);
            _cargoMask = LayerMask.GetMask(Constants.Cargo_Layer);
        }

        public bool FindTouchedPlaceholder(Transform touchPoint, out Placeholder placeholder)
        {
            _colliders.Clean();
            var pos = touchPoint.position;

            var size = Physics.OverlapSphereNonAlloc(pos, Constants.TouchRadius, _colliders, _placeholderMask);
            if (size > 0)
            {
                foreach (var col in _colliders)
                {
                    if (col == null) continue;
                    var ph = col.GetComponent<Placeholder>();
                    if (ph.restricted) continue;
                    
                    placeholder = ph;
                    return true;
                }
            }

            placeholder = null;
            return false;
        }

        public bool CanMove(Vector3 nextPos, GameObject playerObject)
        {
            _colliders.Clean();

            var size = Physics.OverlapSphereNonAlloc(nextPos, Constants.CargoRadius, _colliders, _moveMask);
            if (size <= 0) return true;

            for (var i = 0; i < size; i++)
            {
                var col = _colliders[i];
                if (col.gameObject.Equals(playerObject)) continue;
                return false;
            }

            return true;
        }

        public Cargo FindNearestCargo(GameObject playerObject)
        {
            _colliders.Clean();
            var pos = playerObject.transform.position;

            var size = Physics.OverlapSphereNonAlloc(pos, Constants.SearchRadius, _colliders, _cargoMask);

            if (size == 0) return null;

            foreach (var col in _colliders)
            {
                if (col == null) continue;
                var cargo = col.GetComponent<Cargo>();
                if (cargo.restrictToMove) continue;
                return cargo;
            }

            return null;
        }
    }
}