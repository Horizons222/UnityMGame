using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float verticalMovementFactor = 0.5f; // Este factor reduce la velocidad de movimiento hacia la cámara y alejándose de ella.
    public Animator animator; // Añade tu Animator aquí desde el Inspector

    // Light to follow the player.
    public Light playerLight;
    private Vector3 lightOffset = new Vector3(0, 10, 0); // Adjust this as needed.

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;

    [SerializeField]
    private GameObject spherePrefab;  //Asigna el prefab de tu esfera en el Inspector de Unity.
    public Button spawnButton;  // Añade esta línea


    [Header("UI Setup")]
    public Canvas playerUI;  // Aquí añade tu Canvas desde el Inspector

    [Header("Visual Setup")]
    private MeshRenderer[] playerMeshRenderers; // Array para almacenar todos los MeshRenderers del modelo del jugador


    [Header("MovmentSetup")]
    [SerializeField] private FixedJoystick joystick;
    public Vector2 movementJoystick;
    [HideInInspector]
    public bool canMove = true;
    public GameObject meshObject; // Crea una variable para el objeto hijo "Mesh"

    void Start()
    {
        
        
        characterController = GetComponent<CharacterController>();
        /*if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(SpawnSphere);  // Registra la función SpawnSphere al evento onClick del botón
        }
        else
        {
            Debug.LogError("No se asignó un botón");
        }*/
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        playerUI.gameObject.SetActive(false); // Desactiva el Canvas en el servidor
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
            //joystick.gameObject.SetActive(false);
            playerUI.gameObject.SetActive(false); // Desactiva el Canvas para los clientes que no sean el propietario
            meshObject.SetActive(false); // Desactiva el objeto hijo "Mesh" si el cliente no es el propietario
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = true;
            //joystick.gameObject.SetActive(true);
            playerUI.gameObject.SetActive(true); // Activa el Canvas para los clientes que sean el propietario
            meshObject.SetActive(false); // Desactiva el objeto hijo "Mesh" si el cliente no es el propietario

        }
    }


    public void SpawnSphere()
    {
        if (spherePrefab != null)
        {
            Instantiate(spherePrefab, transform.position + transform.forward, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No se asignó un prefab de esfera");
        }
    }

    void Update()
    {
        if (base.IsOwner)
        {
            //float movementDirectionX = moveDirection.x;
            //float movementDirectionZ = moveDirection.z;

            // Check for screen touches.
            //if (Input.touchCount > 0)
            //{
            //    Touch touch = Input.GetTouch(0);
            //    // If touch is on left half of screen, move left. If on right half, move right.
            //    if (touch.position.x < Screen.width / 2)
            //    {
            //        movementDirectionX = -1.0f;
            //    }
            //    else if (touch.position.x > Screen.width / 2)
            //    {
            //        movementDirectionX = 1.0f;
            //    }

            //    // If touch is on upper half of screen, move forward. If on lower half, move backwards.
            //    if (touch.position.y > Screen.height / 2)
            //    {
            //        movementDirectionZ = verticalMovementFactor;
            //    }
            //    else if (touch.position.y < Screen.height / 2)
            //    {
            //        movementDirectionZ = -verticalMovementFactor;
            //    }

            //    // Cambia la animación a correr
            //    RPCAniamtion("Speed", 1.0f);
            //  //  animator.SetFloat("Speed", 1.0f);
            //}
            //else
            //{
            //    // Cambia la animación a quieto
            //    //  animator.SetFloat("Speed", 0.0f);
            //    RPCAniamtion("Speed", 0.0f);
            //}

            moveDirection = new Vector3(joystick.Horizontal * walkingSpeed * Time.deltaTime, moveDirection.y, joystick.Vertical * walkingSpeed * Time.deltaTime);

            if (joystick.Horizontal != 0 || joystick.Vertical != 0)
            {
                Debug.Log("joystick reconocido");
                RPCAniamtion("Speed", 1.0f);

                // Only rotate based on horizontal movement, not total velocity including falling.
                Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
                if (horizontalVelocity.magnitude > 0) // Prevents errors when velocity is zero.
                {
                    transform.rotation = Quaternion.LookRotation(horizontalVelocity);
                    Debug.Log("joystick reconocido 2");

                }
            }
            else
            {
                RPCAniamtion("Speed", 0.0f);
                Debug.Log("joystick parado");
            }
            movementJoystick.x = joystick.Horizontal;
            movementJoystick.y = joystick.Vertical;
            // Move the controller
            characterController.Move(moveDirection);

            // Move the light with the player.
            if (playerLight != null)
            {
                playerLight.transform.position = transform.position + lightOffset;
            }


        }
    }
    [ServerRpc]
    private void RPCAniamtion(string parm, float val)
    {
        animator.SetFloat(parm, val);
    }
}





