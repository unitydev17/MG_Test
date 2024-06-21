using System.Linq;
using UnityEngine;

namespace CargoMover
{
    public class PlayArea : MonoBehaviour
    {
        private Placeholder[] _placeholders;

        private void Awake()
        {
            _placeholders = transform.GetComponentsInChildren<Placeholder>();
        }


        public Vector3 GetPositionById(int id)
        {
            var placeholder = _placeholders.First(p => p.id == id);
            var pos = placeholder.transform.position;
            pos.y = 1;
            return pos;
        }
    }
}