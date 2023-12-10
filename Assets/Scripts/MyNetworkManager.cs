using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Vector3 spawnPoint = GetSpawnPoint();

        // Instantiate the player object
        GameObject playerObj = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        

        // Add the player object to the connection
        NetworkServer.AddPlayerForConnection(conn, playerObj);


    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.HandlePlayerDisconnect(conn.identity.gameObject);
            }
        }
        
   

        base.OnServerDisconnect(conn);
    }
    
    private Vector3 GetSpawnPoint()
    {

        return new Vector3(0, 0, 0); // Placeholder
    }
}