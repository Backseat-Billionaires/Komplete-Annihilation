using UnityEngine;
using Mirror;

public class ResourceManagement : NetworkBehaviour
{
    [SyncVar]
    private int metal;
    [SyncVar]
    private int energy;

    // Constants for different costs
    private const int MineCost = 10;
    // Add other costs as constants or configurable fields

    // Properties for resource values
    public int Metal
    {
        get { return metal; }
        private set { metal = value; }
    }

    public int Energy
    {
        get { return energy; }
        private set { energy = value; }
    }

    void Start()
    {
        if (isServer)
        {
            Metal = 50;
            Energy = 100;
        }
    }

    [Server]
    public void AddMetal(int amount)
    {
        Metal += amount;
    }

    [Server]
    public void AddEnergy(int amount)
    {
        Energy += amount;
    }

    [Server]
    public bool UseMetal(int amount)
    {
        if (Metal >= amount)
        {
            Metal -= amount;
            return true;
        }
        return false;
    }

    [Server]
    public bool UseEnergy(int amount)
    {
        if (Energy >= amount)
        {
            Energy -= amount;
            return true;
        }
        return false;
    }

    // General methods for checking affordability and spending resources
    public bool CanAfford(int metalCost, int energyCost)
    {
        return Metal >= metalCost && Energy >= energyCost;
    }

    [Server]
    public void SpendResources(int metalCost, int energyCost)
    {
        if (CanAfford(metalCost, energyCost))
        {
            UseMetal(metalCost);
            UseEnergy(energyCost);
        }
    }

    // Legacy methods for mine - consider integrating these into the general methods
    public bool CanAffordMineCost()
    {
        return CanAfford(MineCost, 0); // Assuming mine costs only metal
    }

    [Server]
    public void SpendResourcesForMine()
    {
        SpendResources(MineCost, 0); // Spending only metal for mine
    }

    // Add similar methods for other types of buildings and units
}
