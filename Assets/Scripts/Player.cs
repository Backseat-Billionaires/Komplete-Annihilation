using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public GameObject CommanderGameObject;

    public SyncList<GameObject> UnitGameObjects = new SyncList<GameObject>();

    public Unit Commander { get; private set; }
    public List<Unit> UnitList { get; private set; } = new List<Unit>();

    [Header("References")]
    public GameObject commanderPrefab;

    [Server]
    public void Initialize(Unit commander)
    {
        if (commander == null)
        {
            throw new ArgumentNullException(nameof(commander), "Commander unit cannot be null");
        }

        Commander = commander;
        AddUnit(commander);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        var spawnPosition = GetStartPosition();
        var commanderInstance = Instantiate(commanderPrefab, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(commanderInstance, connectionToClient);

        CommanderGameObject = commanderInstance;

        AddUnit(commanderInstance.GetComponent<Unit>());
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (CommanderGameObject != null)
        {
            Commander = CommanderGameObject.GetComponent<Unit>();
            AddUnit(Commander);
        }

        foreach (var unitGameObject in UnitGameObjects)
        {
            AddUnit(unitGameObject.GetComponent<Unit>());
        }
    }

    [Server]
    private Vector3 GetStartPosition()
    {
        var map = FindObjectOfType<Map>();
        if (map != null)
        {
            return map.GetUniqueRandomSpawnPoint(); // This will return a unique random spawn point
        }
        return Vector3.zero; // Fallback if no map or spawn points are found
    }



    [Server]
    public void AddUnit(Unit unit)
    {
        if (unit == null)
        {
            Debug.LogError("Attempted to add a null unit to the player");
            return;
        }

        // Removed the line: unit.player = this;
        UnitList.Add(unit);
        UnitGameObjects.Add(unit.gameObject);
    }


    [Command]
    public void CmdSendCommandToSelectedUnits(Command command)
    {
        foreach (Unit unit in UnitList)
        {
            if (unit.IsSelected())
            {
                unit.ExecuteCommand(command);
            }
        }
    }

    public void SendCommandToSelectedUnits(Command command)
    {
        if (hasAuthority)
        {
            CmdSendCommandToSelectedUnits(command);
        }
    }
}
