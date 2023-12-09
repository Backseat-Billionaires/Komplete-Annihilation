using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private Map map;

    public static GameManager Instance { get; private set; }

    private Dictionary<int, int> playerScores = new Dictionary<int, int>(); // Store scores with connectionId as key
    private HashSet<int> eliminatedPlayers = new HashSet<int>(); // Track eliminated players by connectionId

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

    public Vector3 GetRespawnPoint()
    {
        return map.GetUniqueRandomSpawnPoint();
    }

    public void SpawnPlayer(NetworkConnectionToClient conn)
    {
        if (map == null)
        {
            Debug.LogError("Map reference is not set in GameManager");
            return;
        }

        Vector3 spawnPoint = map.GetUniqueRandomSpawnPoint();
        GameObject playerObject = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, playerObject);
    }

    [Server]
    public void RecordPlayerDeath(NetworkConnectionToClient conn)
    {
        int connectionId = conn.connectionId;
        if (!eliminatedPlayers.Contains(connectionId))
        {
            eliminatedPlayers.Add(connectionId);

            Health playerHealth = conn.identity.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerScores[connectionId] = playerHealth.GetScore();
            }

            CheckEndGameConditions();
        }
    }

    [Server]
    private void CheckEndGameConditions()
    {
        if (NetworkServer.connections.Count - eliminatedPlayers.Count <= 1)
        {
            foreach (var conn in NetworkServer.connections)
            {
                if (!eliminatedPlayers.Contains(conn.Key))
                {
                    Health winnerHealth = conn.Value.identity.GetComponent<Health>();
                    if (winnerHealth != null && winnerHealth.GetScore() > 0)
                    {
                        // Winner found - end the game and send scores
                        EndGame();
                        break;
                    }
                }
            }
        }
    }

    [Server]
    private void EndGame()
    {
        foreach (var playerScore in playerScores)
        {
            // Call HighScoreSender to send the high score
            HighScoreSender.Instance.SendHighScore(playerScore.Key.ToString(), playerScore.Value);
        }

        // Optional: Implement logic to reset or conclude the game
        // For example, reset the game state or disconnect all players
    }

    // Add additional methods here as needed
}
