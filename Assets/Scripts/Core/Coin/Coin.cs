using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    protected int coinValue = 10;
    protected bool alreadyCollected;

    public abstract int Collect();

    public void SetCoinValue(int value)
    {
        coinValue = value;
    }

    protected void Show(bool show)
    {
        spriteRenderer.enabled = show;
    }

}
