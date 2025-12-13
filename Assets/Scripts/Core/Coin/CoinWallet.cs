using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField]
    Health health;

    [SerializeField]
    BountyCoin coinPrefab;

    [Header("Settings")]
    [SerializeField]
    float coinSpread = 3f;

    [SerializeField]
    float bountyPercentage = 50f;

    [SerializeField]
    int bountyCoinCount = 10;

    [SerializeField]
    private int minBountyCoinValue = 5;

    [SerializeField]
    private LayerMask coinSpawnLayerMask;

    private Collider2D[] coinBuffer = new Collider2D[1];
    float coinSpawnRadius;

    public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        coinSpawnRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        health.OnDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        health.OnDie -= HandleDie;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Coin>(out Coin coin))
        {
            return;
        }

        int coinValue = coin.Collect();

        if (!IsServer)
            return;

        totalCoins.Value += coinValue;
    }

    public void SpendCoins(int amount)
    {
        totalCoins.Value -= amount;
    }

    private void HandleDie(Health health)
    {
        int bountyValue = (int)(totalCoins.Value * (bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyCoinValue < minBountyCoinValue)
        {
            return;
        }

        for (int i = 0; i < bountyCoinCount; i++)
        {
            BountyCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetCoinValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }

    Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * coinSpread;
            Collider2D hitCollider = Physics2D.OverlapCircle(
                spawnPoint,
                coinSpawnRadius,
                coinSpawnLayerMask
            );

            if (hitCollider == null)
            {
                return spawnPoint;
            }
        }
    }
}
