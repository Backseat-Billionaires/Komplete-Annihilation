
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(AIPath))]
public class Unit : MonoBehaviour
{
    // Editor fields
    public bool canMove;
    public bool canStop;
    public Player player;
    public bool facesRightByDefault = true; // Indicates if the sprite faces right by default

    [SerializeField]
    private GameObject selectedSquare;
    private AIDestinationSetter destinationSetter;
    private AIPath ai;
    private bool selected;
    private CameraController cameraController; // Reference to the CameraController
    private Vector2 lastPosition;
    private float flipMultiplier; // Used to flip the sprite based on the default facing direction

    public bool IsSelected => selected;

    void Start()
    {
        selected = false;
        destinationSetter = GetComponent<AIDestinationSetter>();
        ai = GetComponent<AIPath>();
        cameraController = FindObjectOfType<CameraController>(); // Find the CameraController in the scene
        lastPosition = transform.position;
        flipMultiplier = facesRightByDefault ? 1f : -1f; // Set the flip multiplier based on the default facing direction
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;

        // Check for horizontal movement
        if (currentPosition.x != lastPosition.x) // If the unit has moved horizontally
        {
            // Flip the unit based on the movement direction
            transform.localScale = new Vector3(Mathf.Sign(currentPosition.x - lastPosition.x) * flipMultiplier, 1, 1);
        }

        lastPosition = currentPosition; // Update the last position for the next frame
    }

    public void Select(bool multi = false)
    {
        if (!multi)
        {
            foreach (var unit in player.UnitList)
            {
                if (unit.selected)
                {
                    unit.Deselect();
                }
            }
        }
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
        if (canMove)
        {
            Debug.Log("[Unit] Move command received"); // Debugging log
            ai.destination = destination;
        }
    }

    public void Stop()
    {
        if (canStop)
        {
            Debug.Log("[Unit] Stop command received"); // Debugging log
            ai.SetPath(null);
            ai.destination = Vector3.positiveInfinity;
            destinationSetter.target = null;
        }
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
                // Handle other command types if necessary
                break;
        }
    }
}
