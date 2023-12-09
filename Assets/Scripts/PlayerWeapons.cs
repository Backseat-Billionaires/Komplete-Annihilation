using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerWeapons : NetworkBehaviour
{
    public Weapon pistol;
    public Weapon sniper;
    public Weapon laserGun;

    private Dictionary<string, Weapon> weapons = new Dictionary<string, Weapon>();
    private Dictionary<string, int> weaponLevels = new Dictionary<string, int>();
    private Dictionary<string, int> weaponAmmo = new Dictionary<string, int>();
    
    private PlayerInventory playerInventory;

    public override void OnStartServer()
    {
        InitializeWeapons();
        playerInventory = GetComponent<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory component not found on the player.");
        }
    }

    private void InitializeWeapons()
    {
        weapons.Add("Pistol", pistol);
        weaponLevels.Add("Pistol", 1); // Level 1 by default
        weaponAmmo.Add("Pistol", pistol.MaxAmmo); // Full ammo initially

        weapons.Add("Sniper", sniper);
        weaponLevels.Add("Sniper", 0); // Not available initially
        weaponAmmo.Add("Sniper", 0); // No ammo initially

        weapons.Add("LaserGun", laserGun);
        weaponLevels.Add("LaserGun", 0); // Not available initially
        weaponAmmo.Add("LaserGun", 0); // No ammo initially
    }

    [Server]
    public bool UpgradeWeapon(string weaponType, int cost)
    {
        if (!weaponLevels.ContainsKey(weaponType) || !weapons.ContainsKey(weaponType))
        {
            Debug.LogError("Invalid weapon type");
            return false;
        }

        if (weaponLevels[weaponType] < 3) // Assuming max level is 3
        {
            weaponLevels[weaponType]++;
            RpcUpdateWeaponOnClients(weaponType, weaponLevels[weaponType]);
            return true;
        }
        return false;
    }
    
    [Server]
    public void ReloadWeapon(string weaponType)
    {
        if (!weaponAmmo.ContainsKey(weaponType) || !weapons.ContainsKey(weaponType) || playerInventory == null)
        {
            Debug.LogError("Invalid weapon type for reloading or PlayerInventory not found");
            return;
        }

        Weapon weapon = weapons[weaponType];
        int ammoCost = weapon.GetAmmoCost();
        int ammoToFull = weapon.MaxAmmo - weaponAmmo[weaponType];
        
        int playerMetal = playerInventory.GetResourceCount();
        int ammoCanBuy = Mathf.Min(playerMetal / ammoCost, ammoToFull);
        playerInventory.UseResources(ammoCanBuy * ammoCost);
        weaponAmmo[weaponType] += ammoCanBuy;
    }
    
    [Server]
    public bool UseAmmo(string weaponType)
    {
        if (weaponAmmo.ContainsKey(weaponType) && weaponAmmo[weaponType] > 0)
        {
            weaponAmmo[weaponType]--;
            return true;
        }
        return false;
    }


    [ClientRpc]
    private void RpcUpdateWeaponOnClients(string weaponType, int newLevel)
    {
        if (weaponLevels.ContainsKey(weaponType))
        {
            weaponLevels[weaponType] = newLevel;
            // Update UI or other elements to reflect the new weapon level
        }
    }

    public Weapon GetHighestLevelWeapon()
    {
        string highestLevelWeaponType = null;
        int highestLevel = -1;

        foreach (var weapon in weaponLevels)
        {
            if (weapon.Value > highestLevel)
            {
                highestLevelWeaponType = weapon.Key;
                highestLevel = weapon.Value;
            }
        }

        if (highestLevelWeaponType == null)
        {
            Debug.LogError("No weapons available");
            return null;
        }

        return weapons[highestLevelWeaponType];
    }

    
    public (string WeaponName, int Level) GetHighestLevelWeaponInfo()
    {
        string highestLevelWeaponType = null;
        int highestLevel = -1;

        foreach (var weapon in weaponLevels)
        {
            if (weapon.Value > highestLevel)
            {
                highestLevelWeaponType = weapon.Key;
                highestLevel = weapon.Value;
            }
        }

        if (highestLevelWeaponType == null)
        {
            Debug.LogError("No weapons available");
            return (null, -1);
        }

        return (highestLevelWeaponType, highestLevel);
    }

    public Weapon GetCurrentWeapon() => GetHighestLevelWeapon();


    // Additional methods for weapon usage, cooldowns, etc., can be added here
}
