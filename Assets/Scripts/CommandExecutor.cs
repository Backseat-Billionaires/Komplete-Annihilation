using UnityEngine;
using Mirror;

public class CommandExecutor : NetworkBehaviour
{
    
    public CommandExecutor() {}
    
    [Server]
    public void ExecuteCommand(Command command, Player player)
    {
        switch (command.Type)
        {
            case CommandType.Move:
                ExecuteMoveCommand(command, player);
                break;
            case CommandType.Stop:
                ExecuteStopCommand(player);
                break;
            case CommandType.Build:
                ExecuteBuildCommand(command, player);
                break;
            case CommandType.Attack:
                ExecuteAttackCommand(command, player);
                break;
            default:
                break;
            // Add more cases as needed for different command types
        }
    }

    private void ExecuteMoveCommand(Command command, Player player)
    {
        foreach (var unit in player.UnitList)
        {
            if (unit.IsSelected())
            {
                unit.CmdMove(command.Position);
            }
        }
    }

    private void ExecuteStopCommand(Player player)
    {
        foreach (var unit in player.UnitList)
        {
            if (unit.IsSelected())
            {
                unit.CmdStop();
            }
        }
    }

    private void ExecuteBuildCommand(Command command, Player player)
    {
        if (command.Type == CommandType.Build && command.BuildingPrefab != null)
        {
            // Check if the player has enough resources to build
            var resourceManagement = player.GetComponent<ResourceManagement>();
            var buildingCost = GetBuildingCost(command.BuildingPrefab); // You need to define this method
            if (resourceManagement != null && resourceManagement.CanAfford(buildingCost.MetalCost, buildingCost.EnergyCost))
            {
                // Spend resources and instantiate the building
                resourceManagement.SpendResources(buildingCost.MetalCost, buildingCost.EnergyCost);
                Instantiate(command.BuildingPrefab, command.Position, Quaternion.identity);
            }
        }
    }

    private void ExecuteAttackCommand(Command command, Player player)
    {
        if (command.Type == CommandType.Attack && command.Target != null)
        {
            foreach (var unit in player.UnitList)
            {
                if (unit.IsSelected())
                {
                    unit.CmdAttack(command.Target); // Implement CmdAttack in the Unit class
                }
            }
        }
    }

    // Implement this method to determine the building cost based on the prefab
    private (int MetalCost, int EnergyCost) GetBuildingCost(GameObject buildingPrefab)
    {
        // Example implementation - adjust based on your game's mechanics
        switch (buildingPrefab.name)
        {
            case "KBotFactory":
                return (50, 30); // Example costs for a K-Bot Factory
            // Add cases for other buildings
            default:
                return (0, 0);
        }
    }

    // Add more methods as needed for command execution logic
}
