using Pathfinding;
using System.Data;
using UnityEngine;

[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(AIPath))]
public class Unit : MonoBehaviour
{
    // Editor fields

    public bool canMove;
    public bool canStop;

    // Internal data fields
    public Player player;

    [SerializeField]
    private GameObject selectedSquare;
    private AIDestinationSetter destinationSetter;
    private AIPath ai;

    private bool selected;

    public bool IsSelected => selected;

    void Start()
    {
        selected = false;
        destinationSetter = GetComponent<AIDestinationSetter>();
        ai = GetComponent<AIPath>();
    }

    public void Select(bool multi = false)
    {
        if (!multi)
            foreach (var unit in player.UnitList)
                if (unit.selected)
                    unit.Deselect();

        SelectInternal(true);
    }

    public void Deselect()
    {
        SelectInternal(false);
    }

    private void SelectInternal(bool setIsSelected)
    {
        selected = setIsSelected;
        selectedSquare.SetActive(setIsSelected);
    }

    public void Move(Vector3 destination)
    {
        ai.destination = destination;
    }

    public void Stop()
    {
        ai.SetPath(null);
        ai.destination = Vector3.positiveInfinity;
        destinationSetter.target = null;
    }

    public void ExecuteCommand(Command command)
    {
        switch (command.Type)
        {
            case CommandType.Move:
                Move(command.Position);
                break;

            case CommandType.Stop:
                Stop();
                break;

            default:
                break;
        }
    }
}