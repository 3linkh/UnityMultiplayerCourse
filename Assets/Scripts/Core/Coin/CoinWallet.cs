using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Coin>(out Coin coin))
        {
            return;
        }

        int coinValue = coin.Collect();

        if (!IsServer) return;

        totalCoins.Value += coinValue;
    }

    public void SpendCoins(int amount)
    {
        totalCoins.Value -= amount;
    }
}
