using System;
using UnityEngine;
using VContainer.Unity;

namespace CargoMover
{
    public class PhysicCaster : IInitializable, IDisposable
    {
        private Collider[] _colliders;
        private int _placeholderMask;
        private int _moveMask;
        private int _cargoMask;

        public void Initialize()
        {
            CreateMasks();
            _colliders = new Collider[10];

            PlayerController.CanMove += CanMove;
            PlayerController.FindNearestCargo += FindNearestCargo;
            PlayerController.FindTouchedPlaceholder += FindPlaceholder;
        }

        public void Dispose()
        {
            PlayerController.CanMove -= CanMove;
            PlayerController.FindNearestCargo -= FindNearestCargo;
            PlayerController.FindTouchedPlaceholder -= FindPlaceholder;
        }

        private Placeholder FindPlaceholder(Transform transform)
        {
            FindTouchedPlaceholder(transform, out var placeholder);
            return placeholder;
        }

        private void CreateMasks()
        {
            _placeholderMask = LayerMask.GetMask(Constants.Placeholder_Layer);
            _moveMask = LayerMask.GetMask(Constants.Cargo_Layer, Constants.Player_Layer);
            _cargoMask = LayerMask.GetMask(Constants.Cargo_Layer);
        }

        private bool FindTouchedPlaceholder(Transform touchPoint, out Placeholder placeholder)
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

        private bool CanMove(Vector3 nextPos, GameObject playerObject)
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

        private Cargo FindNearestCargo(GameObject playerObject)
        {
            _colliders.Clean();
            var pos = playerObject.transform.position;

            var size = Physics.OverlapSphereNonAlloc(pos, Constants.SearchRadius, _colliders, _cargoMask);

            if (size == 0) return null;

            foreach (var col in _colliders)
            {
                if (col == null) continue;
                var cargo = col.GetComponent<Cargo>();
                if (cargo.restrictToMove.Value) continue;
                return cargo;
            }

            return null;
        }
    }
}