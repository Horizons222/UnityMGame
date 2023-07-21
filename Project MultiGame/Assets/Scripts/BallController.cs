using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform myPlayer;
    [SerializeField] private float xDestroyVal;

    private Vector3 forwardDirection;
    public Transform MyPlayer { get { return myPlayer; }  set { myPlayer = value; } }
    private void OnEnable()
    {
        Debug.Log(gameObject.activeInHierarchy);
    }
    private void Start()
    {
        Debug.Log(gameObject.activeInHierarchy);
        forwardDirection = myPlayer.transform.forward;
    }
    private void Update()
    {
        
        if (myPlayer)
        {
            rb.velocity = forwardDirection * speed;
        }
        if(transform.position.x >= xDestroyVal)
        {
            if (IsServer)
            {
                Despawn();
            }
        }
      
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        Despawn();

    }
}
