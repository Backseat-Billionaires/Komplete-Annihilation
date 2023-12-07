using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public properties
    public Unit Commander { get; private set; }
    public List<Unit> UnitList { get; private set; } = new List<Unit>();

    // Initialization method
    public void Initialize(Unit commander)
    {
        if (commander == null)
        {
            throw new ArgumentNullException(nameof(commander), "Commander unit cannot be null");
        }

        Commander = commander;
        AddUnit(commander);
    }

    // Method to add a unit to the player's control
    public void AddUnit(Unit unit)
    {
        if (unit == null)
        {
            Debug.LogError("Attempted to add a null unit to the player");
            return;
        }

        unit.player = this; // Assign this player to the unit
        UnitList.Add(unit);
    }

    // Method to send a command to all selected units
    public void SendCommandToSelectedUnits(Command command)
    {
        foreach (Unit unit in UnitList)
        {
            if (unit.IsSelected())
            {
                unit.ExecuteCommand(command);
            }
        }
    }

}
