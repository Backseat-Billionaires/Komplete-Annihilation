using UnityEngine;

public class ResourceManagement : MonoBehaviour
{
    public int Metal { get; private set; }
    public int Energy { get; private set; }

    private const int MineCost = 10;

    void Start()
    {
        Metal = 50;
        Energy = 100;
    }

    public void AddMetal(int amount)
    {
        Metal += amount;
    }

    public void AddEnergy(int amount)
    {
        Energy += amount;
    }

    public bool UseMetal(int amount)
    {
        if (Metal >= amount)
        {
            Metal -= amount;
            return true;
        }
        return false;
    }

    public bool UseEnergy(int amount)
    {
        if (Energy >= amount)
        {
            Energy -= amount;
            return true;
        }
        return false;
    }

    public bool CanAffordMineCost()
    {
        return Metal >= MineCost;
    }

    public void SpendResourcesForMine()
    {
        if (CanAffordMineCost())
        {
            UseMetal(MineCost);
        }
    }
}
