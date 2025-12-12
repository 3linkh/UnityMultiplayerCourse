using Unity.Netcode;
using UnityEngine;

public class Movement : NetworkBehaviour
{

    [Header("References")]

    [SerializeField] InputReader inputReader;

    [SerializeField] Transform bodyTransform;
    [SerializeField] Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] float movementSpeed = 4f;
    [SerializeField] float turningRate = 30f;

    private Vector2 previousMovementInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent += HandleMove;
    }
    
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
    }

    void Update()
    {
        if (!IsOwner) return;

        float zRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0, 0, zRotation);


    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        rb.linearVelocity = (Vector2)bodyTransform.up * previousMovementInput.y * movementSpeed;
    }

    void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
        
    }
}
