using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public GameObject commanderPrefab;
    public GameObject playerPrefab;
    private Map map;

    public override void OnStartServer()
    {
        base.OnStartServer();
        map = FindObjectOfType<Map>(); // Find and assign the Map reference
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn, PlayerSpawnOptions spawnOptions)
    {
        var spawnPoint = map.GetSpawnPoint(spawnOptions.SpawnOrder);
        var playerObject = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(playerObject, conn);
        
        var commander = Instantiate(commanderPrefab, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(commander, conn);

        var player = playerObject.GetComponent<Player>();
        player.Initialize(commander.GetComponent<Unit>());
    }
}