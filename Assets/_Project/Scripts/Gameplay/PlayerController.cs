using UnityEngine;
using VContainer;

namespace CargoMover
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _touchPoint;
        [SerializeField] private float _speed;

        private Joystick _joystick;
        private Transform _tr;
        private bool _borrowed;
        private Cargo _cargo;
        private Vector2 _prevJoystick;
        private PhysicCaster _physicCaster;
        private Placeholder _prevPlaceholder;


        [Inject]
        public void Construct(PhysicCaster physicCaster)
        {
            _physicCaster = physicCaster;
        }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _joystick = FindObjectOfType<Joystick>();
            _tr = transform;
            // _tr.SetLocalY(1);
        }

        private void Update()
        {
            Move();
            CheckPlaceholder();
            CheckBorrow();
        }


        private void CheckPlaceholder()
        {
            if (!_borrowed) return;
            
            var found = _physicCaster.FindTouchedPlaceholder(_touchPoint, out var placeholder);
            if (found)
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

            if (!_physicCaster.CanMove(nextPos, gameObject))
            {
                nextPos = position + HorizontalDirection * (_speed * dt);
                if (!_physicCaster.CanMove(nextPos, gameObject))
                {
                    nextPos = position + VerticalDirection * (_speed * dt);
                    if (!_physicCaster.CanMove(nextPos, gameObject))
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

            if (_borrowed)
            {
                Put();
                return;
            }

            Borrow();
        }

        private void Put()
        {
            _borrowed = false;
            _cargo.transform.parent = null;

            var canUsePlaceholder = _prevPlaceholder != null && !_prevPlaceholder.restricted;
            if (canUsePlaceholder)
            {
                _prevPlaceholder.restricted = true;
                _cargo.placeholder = _prevPlaceholder;
            }

            var position = canUsePlaceholder ? _prevPlaceholder.transform.position : _touchPoint.position;
            position.y = 1;

            _cargo.transform.SetPositionAndRotation(position, Quaternion.identity);
        }

        private void Borrow()
        {
            var cargo = _physicCaster.FindNearestCargo(gameObject);
            if (cargo == null) return;

            if (cargo.placeholder != null)
            {
                cargo.placeholder.restricted = false;
                cargo.placeholder = null;
            }

            _borrowed = true;
            _cargo = cargo;

            cargo.transform.parent = gameObject.transform;
            cargo.transform.SetLocalPositionAndRotation(Vector3.up * 1.5f, Quaternion.identity);
        }
    }
}