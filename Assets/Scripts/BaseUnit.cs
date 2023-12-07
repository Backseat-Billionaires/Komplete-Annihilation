// using Pathfinding;
// using UnityEngine;
// using Mirror;
//
// public abstract class BaseUnit : NetworkBehaviour, IGameSelectable
// {
//     public bool canMove;
//     public bool canStop;
//
//     [SerializeField]
//     private GameObject selectedSquare;
//     protected AIDestinationSetter destinationSetter;
//     protected AIPath ai;
//     protected bool selected;
//
//     protected virtual void Start()
//     {
//         destinationSetter = GetComponent<AIDestinationSetter>();
//         ai = GetComponent<AIPath>();
//     }
//
//     public void Select()
//     {
//         selected = true;
//         selectedSquare.SetActive(true);
//         FindObjectOfType<UnitSelectionManager>().SelectObject(gameObject);
//     }
//
//     public void Deselect()
//     {
//         selected = false;
//         selectedSquare.SetActive(false);
//         FindObjectOfType<UnitSelectionManager>().DeselectObject(gameObject);
//     }
//
//     public bool IsSelected()
//     {
//         return selected;
//     }
//
//     public abstract void ExecuteCommand(Command command);
// }