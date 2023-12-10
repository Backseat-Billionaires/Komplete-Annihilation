using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinePlacement : NetworkBehaviour
{
    public GameObject minePrefab;

    [Command]
    public void CmdPlaceMine(Vector3 position)
    {
        GameObject mine = Instantiate(minePrefab, position, Quaternion.identity);
        NetworkServer.Spawn(mine);
        
        if (isLocalPlayer)
        {
            mine.layer = LayerMask.NameToLayer("FriendlyMine");
        }
        else
        {
            mine.layer = LayerMask.NameToLayer("EnemyMine");
        }
    }
}
