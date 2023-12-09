using UnityEngine;
using Mirror;

public class PlayerInventory : NetworkBehaviour
{
    [SyncVar]
    private int metalResources;
    [SyncVar]
    private int bulletCount;
    [SyncVar]
    private int activeMinesCount; 

    
    private readonly int initialBulletCount = 20;
    private readonly int initialMetalCount = 100;
    public const int MaxActiveMinesPerPlayer = 8;


    private MetalDeposit _metalDeposit; 

    public override void OnStartServer()
    {
        metalResources = initialMetalCount;
        bulletCount = initialBulletCount;
        activeMinesCount = 0; 
    }
    
    public void IncrementActiveMines()
    {
        if (activeMinesCount < MaxActiveMinesPerPlayer)
        {
            activeMinesCount++;
        }
    }
    
    public void DecrementActiveMines()
    {
        if (activeMinesCount > 0)
        {
            activeMinesCount--;
        }
    }

    [Server]
    public void AddResources(int amount)
    {
        metalResources += amount;
        RpcUpdateResourceOnClients(metalResources);
    }

    [Server]
    public void UseResources(int amount)
    {
        if (HasEnoughResources(amount))
        {
            metalResources -= amount;
            RpcUpdateResourceOnClients(metalResources);
        }
    }

    public bool HasEnoughResources(int amount)
    {
        return metalResources >= amount;
    }

    [ClientRpc]
    private void RpcUpdateResourceOnClients(int newResourceAmount)
    {
        metalResources = newResourceAmount;
    }

    public int GetResourceCount()
    {
        return metalResources;
    }

    // [Server]
    // public void AddBullets(int amount)
    // {
    //     bulletCount += amount;
    // }
    //
    // [Server]
    // public void UseBullets(int amount)
    // {
    //     if (bulletCount >= amount)
    //     {
    //         bulletCount -= amount;
    //     }
    // }

    public int GetBulletCount() => bulletCount;
    
    public int GetActiveMines() => activeMinesCount;
    
}