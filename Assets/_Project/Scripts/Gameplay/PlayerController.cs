using HelloWorld.Utils;
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

        private Vector2 _joystickValue;
        private PhysicCaster _physicCaster;


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
            _tr.SetLocalY(1);
        }

        private void Update()
        {
            Move();
            CheckPlaceholder();
            CheckBorrow();
        }

        private Placeholder _prevPlaceholder;

        private void CheckPlaceholder()
        {
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
            if (Vector2.Distance(_joystickValue, _joystick.Direction) < 0.1f) return;

            var position = _tr.position;
            var direction = Vector3.forward * _joystick.Vertical + Vector3.right * _joystick.Horizontal;
            var nextPos = position + direction * (_speed * Time.deltaTime);

            var canMove = _physicCaster.CanMove(nextPos, gameObject);
            if (!canMove) nextPos = position;

            transform.position = nextPos;
            transform.rotation = Quaternion.LookRotation(direction);
        }

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