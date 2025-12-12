using Unity.Netcode;
using UnityEngine;

public class Aiming : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform turretTransform;

    void LateUpdate()
    {
        if (!IsOwner) return;

        Vector2 lookScreenPosition = inputReader.LookPosition;
        Vector2 lookWorldPosition = Camera.main.ScreenToWorldPoint(lookScreenPosition);

        turretTransform.up = new Vector2
        (
            lookWorldPosition.x - turretTransform.position.x,
            lookWorldPosition.y - turretTransform.position.y
        );
    }


}
