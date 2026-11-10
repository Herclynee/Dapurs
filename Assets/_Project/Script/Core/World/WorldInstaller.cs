using UnityEngine;

public class WorldInstaller : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    
    // 1. TAMBAHKAN BARIS INI (Variabel untuk interactor)
    [SerializeField] private PlayerInteractor interactor; 

    // 2. UBAH METHOD INSTALL INI (Menerima 2 parameter)
    public void Install(IMoveInput moveInput, IActionInput actionInput)
    {
        player.Construct(moveInput);
        
        // 3. TAMBAHKAN BARIS INI (Menyuntikkan input ke interactor)
        interactor.Construct(actionInput); 
    }
}