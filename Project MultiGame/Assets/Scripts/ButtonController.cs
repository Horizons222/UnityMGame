using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public PlayerController playerController;  // Asegúrate de establecer este campo en el Inspector de Unity

    void Start()
    {
        // Configura el botón para que llame a la función SpawnSphere cuando se pulse.
        GetComponent<Button>().onClick.AddListener(playerController.SpawnSphere);
    }
}
