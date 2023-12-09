using UnityEngine;
using Mirror;


public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Access the GameManager instance directly if using a singleton pattern
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.SpawnPlayer(conn);
        }
        else
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    // Other NetworkManager methods...
}