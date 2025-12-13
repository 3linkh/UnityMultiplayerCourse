using Microsoft.Unity.VisualStudio.Editor;
using Unity.Netcode;
using UnityEngine;

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
}
