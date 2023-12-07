using Pathfinding;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(AIPath))]
public class Unit : NetworkBehaviour, IGameSelectable
{
    // Editor fields
    public bool canMove;
    public bool canStop;
    public bool canAttack;
    public bool isCombatUnit;
    public bool isConstructorUnit;
    public bool facesRightByDefault = true;

    [SerializeField]
    private GameObject selectedSquare;
    private AIDestinationSetter destinationSetter;
    private AIPath ai;
    private Vector2 lastPosition;
    private float flipMultiplier;
    private bool selected;

    void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        ai = GetComponent<AIPath>();
        lastPosition = transform.position;
        flipMultiplier = facesRightByDefault ? 1f : -1f;
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        if (currentPosition.x != lastPosition.x)
        {
            transform.localScale = new Vector3(Mathf.Sign(currentPosition.x - lastPosition.x) * flipMultiplier, 1, 1);
        }
        lastPosition = currentPosition;
    }

    // IGameSelectable interface implementation
    public void Select()
    {
        selected = true;
        selectedSquare.SetActive(true);
        FindObjectOfType<UnitSelectionManager>().SelectObject(gameObject);
    }

    public void Deselect()
    {
        selected = false;
        selectedSquare.SetActive(false);
        FindObjectOfType<UnitSelectionManager>().DeselectObject(gameObject);
    }

    public bool IsSelected()
    {
        return selected;
    }

    [Command]
    public void CmdMove(Vector3 destination)
    {
        if (canMove)
        {
            RpcMove(destination);
        }
    }

    [Command]
    public void CmdStop()
    {
        if (canStop)
        {
            RpcStop();
        }
    }

    [Command]
    public void CmdAttack(GameObject target)
    {
        if (canAttack && target != null)
        {
            // Implement attack logic here, e.g., set target, start attack coroutine
        }
    }

    [ClientRpc]
    void RpcMove(Vector3 destination)
    {
        ai.destination = destination;
    }

    [ClientRpc]
    void RpcStop()
    {
        ai.SetPath(null);
        ai.destination = Vector3.positiveInfinity;
        destinationSetter.target = null;
    }

    public void ExecuteCommand(Command command)
    {
        if (!isServer) return;

        switch (command.Type)
        {
            case CommandType.Move:
                CmdMove(command.Position);
                break;
            case CommandType.Stop:
                CmdStop();
                break;
            case CommandType.Attack:
                if (command.Target != null)
                {
                    CmdAttack(command.Target);
                }
                break;
            // Implement additional command types as needed
        }
    }

    // Methods for K-Bot functionality (placeholder implementations)
    public void ConstructBuilding(GameObject buildingPrefab, Vector3 position)
    {
        if (isConstructorUnit)
        {
            // Construction logic for K-Bots
        }
    }

    public void HealUnit(GameObject target)
    {
        if (isConstructorUnit)
        {
            // Healing logic for K-Bots
        }
    }
}
