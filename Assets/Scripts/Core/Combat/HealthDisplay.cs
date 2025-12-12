using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Health health;
    [SerializeField] Image healthBarImage;
    
    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;
        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, health.CurrentHealth.Value);
    }
    
    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    void HandleHealthChanged(int oldHealth, int newHealth)
    {
        float healthNormalized = (float)newHealth / health.maxHealth;
        healthBarImage.fillAmount = (float)newHealth / health.maxHealth;
    }

}
