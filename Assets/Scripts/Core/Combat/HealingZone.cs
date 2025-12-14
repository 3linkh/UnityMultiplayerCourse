using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField]
    Image healPowerBar;

    [Header("Settings")]
    [SerializeField]
    int maxHealPower = 30;

    [SerializeField]
    float healCooldown = 60f;

    [SerializeField]
    float healTickRate = 1f;

    [SerializeField]
    private int coinsPerTick = 10;

    [SerializeField]
    int healPerTick = 10;
    private float remainingCooldown;
    private float tickTimer;

    private List<TankPlayer> playersInZone = new List<TankPlayer>();

    private NetworkVariable<int> healPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            healPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, healPower.Value);
        }

        if (IsServer)
        {
            healPower.Value = maxHealPower;
        }
    }

    public override void OnNetworkDespawn() { }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsServer)
            return;
        if (!collider.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            return;
        }

        playersInZone.Remove(player);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (!IsServer)
            return;
        if (!collider.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            return;
        }

        playersInZone.Remove(player);
    }

    void Update()
    {
        if (!IsServer)
            return;
        if (remainingCooldown > 0f)
        {
            remainingCooldown -= Time.deltaTime;
            if (remainingCooldown <= 0f)
            {
                healPower.Value = maxHealPower;
            }
            else
            {
                return;
            }
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= 1 / healTickRate)
        {
            foreach (TankPlayer player in playersInZone)
            {
                if (healPower.Value == 0)
                {
                    break;
                }

                if (player.Health.CurrentHealth.Value == player.Health.maxHealth)
                {
                    continue;
                }

                if (player.wallet.totalCoins.Value < coinsPerTick)
                {
                    continue;
                }

                player.wallet.SpendCoins(coinsPerTick);
                player.Health.RestoreHealth(healPerTick);

                healPower.Value -= 1;

                if (healPower.Value == 0)
                {
                    remainingCooldown = healCooldown;
                }
            }

            tickTimer = tickTimer % (1 / healTickRate);
        }
    }

    private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        healPowerBar.fillAmount = (float)newHealPower / maxHealPower;
    }
}
