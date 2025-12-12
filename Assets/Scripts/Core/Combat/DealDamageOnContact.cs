using UnityEngine;
using Unity.Netcode;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] int damage = 5;

    ulong ownerClientId;
    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody == null) return;

        if(other.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
        {
            if (networkObject.OwnerClientId == ownerClientId) return;
        }

        if (other.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }

}
