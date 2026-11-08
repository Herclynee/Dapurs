using UnityEngine;

public class WorldInstaller : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;   // BARU

    public void Install(IMoveInput moveInput, IActionInput actionInput)  // BERUBAH
    {
        player.Construct(moveInput);
        interactor.Construct(actionInput);                   // BARU
    }
}