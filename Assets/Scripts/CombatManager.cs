using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PerformAttack(GameObject attacker, GameObject target, Weapon currentWeapon)
    {
        PlayerWeapons playerWeapons = attacker.GetComponent<PlayerWeapons>();
        if (playerWeapons == null)
        {
            Debug.LogError("PlayerWeapons component not found on the attacker");
            return;
        }

        // check if the player has ammo for the current weapon
        if (!playerWeapons.UseAmmo(currentWeapon.weaponName)) 
        {
            Debug.Log("No ammo left to perform an attack.");
            return;
        }

        // perform attack if target is in range and visible
        if (IsTargetInRangeAndVisible(attacker, target, currentWeapon.BaseRange))
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(currentWeapon.BaseDamage, attacker);
            }
        }
    }

    private bool IsTargetInRangeAndVisible(GameObject attacker, GameObject target, float range)
    {
        if (Vector3.Distance(attacker.transform.position, target.transform.position) > range)
        {
            return false;
        }

        RaycastHit hit;
        Vector3 direction = target.transform.position - attacker.transform.position;
        if (Physics.Raycast(attacker.transform.position, direction, out hit, range))
        {
            return hit.transform == target.transform;
        }

        return false;
    }
    
}