using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR;

public class CointSpawner : NetworkBehaviour
{
    [SerializeField] RespawningCoin coinPrefab;

    [SerializeField] private int maxCoins = 50;

    [SerializeField] private int coinValue = 10;

    [SerializeField] private Vector2 xspawnRange;
    [SerializeField] private Vector2 yspawnRange;

    [SerializeField] private LayerMask coinSpawnLayerMask;
    
    private Collider2D[] coinBuffer = new Collider2D[1];
    float coinSpawnRadius;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        coinSpawnRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }
    private void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
        coinInstance.SetCoinValue(coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;

        while (true)
        {
            x = Random.Range(xspawnRange.x, xspawnRange.y);
            y = Random.Range(yspawnRange.x, yspawnRange.y);

            Vector2 spawnPoint = new Vector2(x, y);
            Collider2D hitCollider = Physics2D.OverlapCircle(spawnPoint, coinSpawnRadius, coinSpawnLayerMask);

            if (hitCollider == null)
            {
                return spawnPoint;
            }
        }
    }

}
