using Unity.Netcode;

namespace CargoMover
{
    public class Cargo : NetworkBehaviour
    {
        public bool restrictToMove;
        public Placeholder placeholder;
    }
}