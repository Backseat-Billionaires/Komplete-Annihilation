using UnityEngine;
using Mirror;

public class NetworkGameController : NetworkBehaviour
{
    public static NetworkGameController singleton { get; private set; }
    public Map map;

    private void Awake()
    {
        singleton = this; // Assign the singleton
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        map = FindObjectOfType<Map>(); // Find and assign the Map reference
    }

    [Server]
    public void SetActivePlayer(Player player)
    {
        // Logic for setting the active player
        // This could involve setting a SyncVar, managing a list of players, etc.
    }
}