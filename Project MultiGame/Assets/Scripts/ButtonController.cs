using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public PlayerController playerController;  // Aseg�rate de establecer este campo en el Inspector de Unity

    void Start()
    {
        // Configura el bot�n para que llame a la funci�n SpawnSphere cuando se pulse.
        GetComponent<Button>().onClick.AddListener(playerController.SpawnSphere);
    }
}
