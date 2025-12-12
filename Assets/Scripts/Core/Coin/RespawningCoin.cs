using System;
using UnityEngine;

public class RespawningCoin : Coin
{

    public event Action<RespawningCoin> OnCollected;

    private Vector3 prevPosition;

    private void Update()
    {
        if (prevPosition != transform.position)
        {
            prevPosition = transform.position;
            Show(true);
        }
        
        prevPosition = transform.position;
    }
    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if (alreadyCollected) return 0;

        alreadyCollected = true;
        OnCollected?.Invoke(this);
        return coinValue;
        
    }

    internal void Reset()
    {
        alreadyCollected = false;
    }
}
