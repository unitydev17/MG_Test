using Unity.Netcode;

namespace CargoMover
{
    public class Cargo : NetworkBehaviour
    {
        public NetworkVariable<bool> restrictToMove = new NetworkVariable<bool>();
        public Placeholder placeholder;
    }
}