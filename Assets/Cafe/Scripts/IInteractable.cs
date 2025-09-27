using UnityEngine;

namespace Cafe
{
    public interface IInteractable
    {
        void Interact(PlayerController player);
        string GetInteractionText();
        bool CanInteract(PlayerController player);
    }
}
