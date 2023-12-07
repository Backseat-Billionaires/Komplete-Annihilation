using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    
    public CustomNetworkManager() {}
    // Override for adding a player to the server
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject playerObj = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // Give the player a unique name for easier debugging
        playerObj.name = $"{playerPrefab.name} [connId={conn.connectionId}]";

        // Add the player to the server
        NetworkServer.AddPlayerForConnection(conn, playerObj);
    }

    private Transform GetStartPosition()
    {
        // Implement logic to fetch different spawn positions for each player
        // Placeholder logic: return a random start position
        int startIndex = Random.Range(0, startPositions.Count);
        return startPositions[startIndex];
    }
}