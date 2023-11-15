using System;
using System.Collections.Generic;

public class Player
{
    public Player(Unit commander)
    {
        if (commander == null) throw new ArgumentNullException(nameof(commander));
        AddUnit(commander);
    }

    public List<Unit> UnitList { get; } = new();

    public void AddUnit(Unit unit)
    {
        unit.player = this;
        UnitList.Add(unit);
    }

    public void SendCommandToSelectedUnits(Command command)
    {
        foreach (Unit unit in UnitList)
        {
            if (unit.IsSelected)
                unit.ExecuteCommand(command);
        }
    }
}
