using UnityEngine;

namespace CargoMover
{
    public class Placeholder : MonoBehaviour
    {
        [SerializeField] private Color _baseColor;
        [SerializeField] private Color _markedColor;

        private Renderer _renderer;
        

        public int id;
        public bool restricted;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log(other.gameObject);
        }

        private void OnCollisionExit(Collision other)
        {
            Debug.Log(other.gameObject);
        }

        public void Mark()
        {
            _renderer.material.color = _markedColor;
        }

        public void Reset()
        {
            _renderer.material.color = _baseColor;
        }
    }
}