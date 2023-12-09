using UnityEngine;
using Mirror;
using System;
using System.Collections;

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

    public event Action<GameObject> OnDeath;

    [SyncVar]
    private int score;

    private GameManager gameManager;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        currentLives = maxRespawns;
        deathCount = 0;
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

            if (IsPlayer())
            {
                HandlePlayerDeath(attacker);
            }
            else
            {
                RpcHandleNonPlayerDeath();
            }
        }
    }

    private bool IsPlayer()
    {
        return GetComponent<CharacterController>() != null;
    }

    [Server]
    private void HandlePlayerDeath(GameObject attacker)
    {
        currentLives--;
        if (currentLives <= 0)
        {
            gameManager.RecordPlayerDeath(connectionToClient); // Inform GameManager of player elimination
        }

        if (attacker != null)
        {
            var attackerHealth = attacker.GetComponent<Health>();
            if (attackerHealth != null)
            {
                attackerHealth.IncrementScore();
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
