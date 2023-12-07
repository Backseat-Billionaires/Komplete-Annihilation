using UnityEngine;

public enum CommandType
{
    Move,
    Build,
    Stop,
    Attack
}

public class Command
{
    // Default constructor for Mirror serialization
    public Command() {}

    public CommandType Type { get; private set; }
    public Vector3 Position { get; set; }
    public GameObject Target { get; set; } // Changed from Unit to GameObject for more flexibility
    public GameObject BuildingPrefab { get; set; } // For build commands

    // Constructor for simple commands
    public Command(CommandType type)
    {
        Type = type;
    }

    // Constructor for commands with a position (e.g., Move, Build)
    public Command(CommandType type, Vector3 position) : this(type)
    {
        Position = position;
    }

    // Constructor for commands with a target (e.g., Attack)
    public Command(CommandType type, GameObject target) : this(type)
    {
        Target = target;
    }

    // Constructor for build commands with a building prefab
    public Command(CommandType type, Vector3 position, GameObject buildingPrefab) : this(type, position)
    {
        BuildingPrefab = buildingPrefab;
    }
}