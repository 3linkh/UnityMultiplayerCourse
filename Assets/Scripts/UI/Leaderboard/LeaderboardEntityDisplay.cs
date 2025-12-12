using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField]
    TMP_Text displayText;

    [SerializeField]
    Color myColor;
    private FixedString32Bytes playerName;

    public ulong ClientID { get; private set; }
    public int Coins { get; private set; }

    public void Initialise(ulong clientID, FixedString32Bytes playerName, int coins)
    {
        ClientID = clientID;
        this.playerName = playerName;

        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            displayText.color = myColor;
        }

        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }

    public void UpdateText()
    {
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {playerName} ({Coins})";
    }
}
