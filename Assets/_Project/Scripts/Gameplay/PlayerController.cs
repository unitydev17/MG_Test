using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace CargoMover
{
    public class PlayerController : NetworkBehaviour
    {
        public static event Func<Vector3, GameObject, bool> CanMove;
        public static event Func<GameObject, Cargo> FindNearestCargo;
        public static event Func<Transform, Placeholder> FindTouchedPlaceholder;

        [SerializeField] private Transform _touchPoint;
        [SerializeField] private float _speed;

        private Joystick _joystick;
        private Transform _tr;
        private bool _borrowed;
        private Cargo _cargo;
        private ulong _cargoId;
        private Vector2 _prevJoystick;
        private Placeholder _prevPlaceholder;

        public override void OnNetworkSpawn()
        {
            Initialize();
        }

        private void Initialize()
        {
            _joystick = FindObjectOfType<Joystick>();
            _tr = transform;
            _tr.SetLocalY(1);
        }

        private void Update()
        {
            if (!IsOwner) return;

            Move();
            CheckPlaceholder();
            CheckBorrow();
        }


        private void CheckPlaceholder()
        {
            if (!_borrowed) return;
            var placeholder = FindTouchedPlaceholder?.Invoke(_touchPoint);

            if (placeholder != null)
            {
                if (placeholder == _prevPlaceholder) return;
                if (_prevPlaceholder != null) _prevPlaceholder.Reset();
                placeholder.Mark();
                _prevPlaceholder = placeholder;
            }
            else
            {
                if (_prevPlaceholder != null) _prevPlaceholder.Reset();
                _prevPlaceholder = null;
            }
        }


        private void Move()
        {
            if (Vector2.Distance(_prevJoystick, _joystick.Direction) < 0.1f) return;

            var dt = Time.deltaTime;
            var position = _tr.position;
            var nextPos = position + Direction * (_speed * dt);


            if (CanMove?.Invoke(nextPos, gameObject) == false)
            {
                nextPos = position + HorizontalDirection * (_speed * dt);
                if (CanMove?.Invoke(nextPos, gameObject) == false)
                {
                    nextPos = position + VerticalDirection * (_speed * dt);
                    if (CanMove?.Invoke(nextPos, gameObject) == false)
                    {
                        nextPos = position;
                    }
                }
            }

            transform.position = nextPos;
            transform.rotation = Quaternion.LookRotation(Direction);
        }

        private Vector3 Direction => Vector3.forward * _joystick.Vertical + Vector3.right * _joystick.Horizontal;
        private Vector3 VerticalDirection => Vector3.forward * _joystick.Vertical;
        private Vector3 HorizontalDirection => Vector3.right * _joystick.Horizontal;

        private void CheckBorrow()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;

            if (!IsOwner) return;

            if (_borrowed)
            {
                TryPut();
                return;
            }

            TryBorrow();
        }

        private void TryBorrow()
        {
            var cargo = FindNearestCargo?.Invoke(gameObject);
            if (cargo == null) return;
            if (cargo.placeholder != null)
            {
                cargo.placeholder.restricted = false;
                cargo.placeholder = null;
            }

            _borrowed = true;
            // _cargoId = cargo.networkId;
            _cargo = cargo;

            ParentCargoToPlayerServerRpc(_cargo.NetworkObjectId);
        }

        [ServerRpc]
        private void ParentCargoToPlayerServerRpc(ulong cargoId)
        {
            var cargoes = FindObjectsOfType<Cargo>();
            var cargo = cargoes.FirstOrDefault(c => c.NetworkObjectId == cargoId);
            if (cargo == null) return;

            cargo.NetworkObject.TrySetParent(gameObject);
            cargo.transform.SetLocalPositionAndRotation(Vector3.up * 1.5f, Quaternion.identity);
        }

        private void TryPut()
        {
            _borrowed = false;

            var canUsePlaceholder = _prevPlaceholder != null && !_prevPlaceholder.restricted;
            if (canUsePlaceholder)
            {
                _prevPlaceholder.restricted = true;
                _cargo.placeholder = _prevPlaceholder;
            }

            var position = canUsePlaceholder ? _prevPlaceholder.transform.position : _touchPoint.position;
            position.y = 1;

            UnparentCargoServerRpc(_cargo.NetworkObjectId, position);
        }

        [ServerRpc]
        private void UnparentCargoServerRpc(ulong cargoId, Vector3 position)
        {
            var cargoes = FindObjectsOfType<Cargo>();
            var cargo = cargoes.FirstOrDefault(no => no.NetworkObjectId == cargoId);
            if (cargo == null) return;

            cargo.NetworkObject.TryRemoveParent();
            cargo.transform.SetPositionAndRotation(position, Quaternion.identity);
        }
    }
}