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

    // Returns the properties of the weapon based on the current level
    public (int Damage, float Range, float FireRate) GetPropertiesAtLevel(int level)
    {
        int damage = BaseDamage + (level - 1) * 2; // Increment damage by 2 per level
        float range = BaseRange + (level - 1) * 1.5f; // Increment range per level
        float fireRate = BaseFireRate + (level - 1) * 0.2f; // Increment fire rate per level

        return (damage, range, fireRate);
    }

    // Method to get the ammo cost for the weapon
    public int GetAmmoCost()
    {
        return AmmoCostPerRound;
    }
}