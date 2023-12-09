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
    private float nextFireTime = 0f;

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
        weaponAmmo.Add("Sniper", sniper.MaxAmmo); 

        weapons.Add("LaserGun", laserGun);
        weaponLevels.Add("LaserGun", 0); // Not available initially
        weaponAmmo.Add("LaserGun", laserGun.MaxAmmo);
    }

    [Server]
    public void FireWeapon(string weaponType)
    {
        if (!CanFire(weaponType)) return;
        
        if (UseAmmo(weaponType))
        {
        }
    }

    private bool CanFire(string weaponType)
    {
        if (!HasAmmo(weaponType) || Time.time < nextFireTime) return false;

        nextFireTime = Time.time + GetFireRate(weaponType);
        return true;
    }

    private float GetFireRate(string weaponType)
    {
        if (!weapons.ContainsKey(weaponType)) return 0f;

        Weapon weapon = weapons[weaponType];
        int level = weaponLevels.ContainsKey(weaponType) ? weaponLevels[weaponType] : 1;
        (_, _, float fireRate) = weapon.GetPropertiesAtLevel(level);
        return 1f / fireRate;
    }

    [Server]
    public bool UpgradeWeapon(string weaponType)
    {
        if (!weaponLevels.ContainsKey(weaponType) || !weapons.ContainsKey(weaponType))
        {
            Debug.LogError("Invalid weapon type");
            return false;
        }

        if (weaponLevels[weaponType] < 3) 
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

        if (ammoCanBuy > 0)
        {
            playerInventory.UseResources(ammoCanBuy * ammoCost);
            weaponAmmo[weaponType] += ammoCanBuy;
        }
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
    
    public bool HasAmmo(string weaponType)
    {
        if (weaponAmmo.TryGetValue(weaponType, out int ammoCount))
        {
            return ammoCount > 0;
        }

        return false;
    }


    public Weapon GetCurrentWeapon() => GetHighestLevelWeapon();
}
