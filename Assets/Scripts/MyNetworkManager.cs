using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            GameObject playerObject = Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, playerObject);
            gameManager.SetupPlayer(playerObject, conn); // Additional setup for player
        }
        else
        {
            Debug.LogError("GameManager not found in the scene.");
        }
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
}