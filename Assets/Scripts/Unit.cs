using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static List<Unit> unitList = new List<Unit>();
    public float speed;

    [SerializeField]
    private GameObject selectedSquare;
    private Vector3? target;
    private bool selected;

    public bool IsSelected => selected;

    void Start()
    {
        selected = false;
        unitList.Add(this);
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.Value, speed * Time.deltaTime);
            if (transform.position == target.Value)
                target = null;
        }
    }

    public void Select(bool multi = false)
    {
        if (!multi)
            foreach (var unit in unitList)
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

    public void Move(Vector2 destination)
    {
        target = destination;
    }

    public void executeCommand(Command command)
    {
        switch (command.CommandType)
        {
            case CommandType.Move:
                Move(command.Position);
                break;
            
            default:
                break;
        }
    }
}