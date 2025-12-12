using System;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] CoinWallet coinWallet;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] GameObject serverProjectilePrefab;
    [SerializeField] GameObject clientProjectilePrefab;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;


    [Header("Settings")]
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField] float muzzleFlashDuration = 1f;
    [SerializeField] private int costToFire;

    bool shouldFire;
    private float timer;
    private float muzzleFlashTimer;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.AttackEvent += HandlePrimaryFire;
        
    }
    
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.AttackEvent -= HandlePrimaryFire;

    }
    
    void Update()
    {
        if(muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if(muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) return;

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }   

        if (!shouldFire)
        {
            return;
        }

        if (timer > 0f)
        {
            return;
        }

        if(coinWallet.totalCoins.Value < costToFire)
        {
            return;
        }
        

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        timer = 1 / fireRate;
    }

    


    void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;

    }

    [ServerRpc]
    void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (coinWallet.totalCoins.Value < costToFire)
        {
            return;
        }
        coinWallet.SpendCoins(costToFire);
        GameObject projectileInstance = Instantiate(
            serverProjectilePrefab,
            spawnPos,
            Quaternion.identity);

        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }
    
        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }


    [ClientRpc]
    void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner) return;
        SpawnDummyProjectile(spawnPos, direction);
    }
    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(
            clientProjectilePrefab,
            spawnPos,
            Quaternion.identity);

        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

    }

    
    
    
}
