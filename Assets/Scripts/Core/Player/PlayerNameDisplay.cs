using System;
using Unity.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{

    [SerializeField] private TankPlayer player;

    [SerializeField] TMPro.TMP_Text playerNameText;
    void Start()
    {
        HandlePlayerNameChanged(string.Empty, player.playerName.Value);
        
        player.playerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        playerNameText.text = newName.ToString();
    }

    void OnDestroy()
    {
        player.playerName.OnValueChanged -= HandlePlayerNameChanged;
    }
}
