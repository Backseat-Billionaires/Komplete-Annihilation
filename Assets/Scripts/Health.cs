using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    [SyncVar]
    private int currentHealth;
    [SerializeField]
    private float respawnDelay = 5f;
    [SerializeField]
    private int maxRespawns = 15;
    [SyncVar]
    private int currentLives;
    [SyncVar]
    private int deathCount;
    [SyncVar]
    private int score;
    [SyncVar]
    private int killCount;

    private GameManager gameManager;

    public event Action<GameObject> OnDeath;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        currentLives = maxRespawns;
        deathCount = 0;
        killCount = 0;
        gameManager = FindObjectOfType<GameManager>();
    }

    [Server]
    public void TakeDamage(int amount, GameObject attacker)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDeath?.Invoke(attacker);
            deathCount++;

            if (IsPlayerCharacter())
            {
                HandlePlayerDeath(attacker);
            }
            else
            {
                RpcHandleNonPlayerDeath();
            }
        }
    }

    private bool IsPlayerCharacter()
    {
        return GetComponent<PlayerController>() != null;
    }

    [Server]
    private void HandlePlayerDeath(GameObject attacker)
    {
        currentLives--;
        if (currentLives <= 0)
        {
            gameManager.RecordPlayerDeath(connectionToClient);
        }

        if (attacker != null)
        {
            var attackerHealth = attacker.GetComponent<Health>();
            if (attackerHealth != null)
            {
                attackerHealth.IncrementScore();
                attackerHealth.IncrementKillCount();
            }
        }

        if (currentLives > 0)
        {
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            RpcEliminatePlayer();
        }
    }

    [Server]
    private void IncrementScore()
    {
        score += 1000;
    }

    [Server]
    private void IncrementKillCount()
    {
        killCount++;
        CheckForWeaponUpgrade();
    }

    [Server]
    private void CheckForWeaponUpgrade()
    {
        const int KillsForUpgrade = 3;
        if (killCount >= KillsForUpgrade)
        {
            var playerWeapons = GetComponent<PlayerWeapons>();
            if (playerWeapons != null)
            {
                playerWeapons.UpgradeWeapon(playerWeapons.GetCurrentWeapon().weaponName);
                killCount = 0;
            }
        }
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(respawnDelay);
        RpcRespawnAtPoint(gameManager.GetRespawnPoint());
    }

    [ClientRpc]
    private void RpcRespawnAtPoint(Vector3 spawnPoint)
    {
        if (isLocalPlayer)
        {
            transform.position = spawnPoint;
            GetComponent<CharacterController>().enabled = true;
        }
    }

    [ClientRpc]
    private void RpcEliminatePlayer()
    {
        if (isLocalPlayer)
        {
            Debug.Log("You have been eliminated!");
            GetComponent<CharacterController>().enabled = false;
        }
    }

    [ClientRpc]
    private void RpcHandleNonPlayerDeath()
    {
        if (!isLocalPlayer)
        {
            Destroy(gameObject);
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetCurrentLives() => currentLives;
    public int GetDeathCount() => deathCount;
    public int GetScore() => score;
    public int GetMaxHealth() => maxHealth;
    public int GetMaxRespawns() => maxRespawns;
}
