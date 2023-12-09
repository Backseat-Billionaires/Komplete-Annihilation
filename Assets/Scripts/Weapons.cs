using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int BaseDamage;
    public float BaseRange;
    public float BaseFireRate;
    public int MaxAmmo;  // Maximum ammo capacity for the weapon
    public int AmmoCostPerRound; // Cost of ammo per round in metal
    
    public (int Damage, float Range, float FireRate) GetPropertiesAtLevel(int level)
    {
        int damage = BaseDamage + (level - 1) * 2; 
        float range = BaseRange + (level - 1) * 1.5f; 
        float fireRate = BaseFireRate + (level - 1) * 0.2f; 

        return (damage, range, fireRate);
    }
    
    public int GetAmmoCost()
    {
        return AmmoCostPerRound;    
    }
}