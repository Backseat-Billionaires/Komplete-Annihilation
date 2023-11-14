using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandType{
    Move,
    Build,
    Stop
}

public class Command
{
    public Command(CommandType type){
        Type = type;
    }

public Command(CommandType type, Vector2 position) : this(type){
    Position = position;
}

public Command(CommandType type, Vector2 position, Unit target) : this(type, position){
    Target = target;
}

    public CommandType Type { get; }
    public Vector2 Position { get; set; }
    public Unit Target { get; }
}
