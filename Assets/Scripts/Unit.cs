using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static List<Unit> unitList = new List<Unit>();
    public float speed;
    private Vector3 target;
    private bool selected;

    void Start() 
    {
        
        // unitList = new List<Unit>();
        target = transform.position;
        selected = false;
        unitList.Add(this);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && selected)
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
            
        }
        // foreach (Unit unit in unitList)
        // {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        // }

        if (Input.GetMouseButtonDown(1))
        {
            selected = false;
        }
    }

    private void OnMouseDown()
    {
        if(!Input.GetKeyDown(KeyCode.LeftShift))
        {
            foreach (Unit obj in unitList)
            {
                obj.selected = false;
            }
        }

        selected = true;
    }


    
    // public static List<Unit> unitList = new List<Unit>();
    // public float speed;
    // private Vector3 target;
    // private bool selected;

    // void Start() 
    // {
    //     speed = 5f;
    //     // unitList = new List<Unit>();
    //     target = transform.position;
    //     selected = false;
    //     unitList.Add(this);
    // }

    // void Update()
    // {
    //     if(Input.GetMouseButtonDown(0) && selected)
    //     {
    //         target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         target.z = transform.position.z;
            
    //     }
    //     // foreach (Unit unit in unitList)
    //     // {
    //     transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    //     // }

    //     if (Input.GetMouseButtonDown(1))
    //     {
    //         selected = false;
    //     }

    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         selected = true;
    //     }

        
    // }

    // private void OnMouseDown()
    // {
    //     foreach (Unit obj in unitList)
    //     {
    //         if (Input.GetMouseButtonDown(1))
    //         {
    //             obj.selected = false;
    //         }

    //         if (Input.GetMouseButtonDown(0))
    //         {
    //             obj.selected = true;
    //         }
    //     }

    // }



    // private Vector3 startPosition;
    // private List<Unit> selectedUnitList;

    // void Start()
    // {
    //     selectedUnitList = new List<Unit>();
    // }

    // private void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         startPosition = Input.mousePosition;
    //     }

    //     if (Input.GetMouseButtonUp(0))
    //     {
    //         // Debug.Log(Input.mousePosition + " " + startPosition);
    //         Collider2D [] collider2DArray =  Physics2D.OverlapAreaAll(startPosition, Input.mousePosition);
    //         selectedUnitList.Clear();
    //         foreach (Collider2D collider2D in collider2DArray)
    //         {
    //             Unit unit = collider2D.GetComponent<Unit>();
    //             if (unit != null)
    //             {
    //                 selectedUnitList.Add(unit);
    //             }
    //         }

    //         Debug.Log(selectedUnitList.Count);
    //     }
    // }



    // public static List<Unit> unitList = new List<Unit>();
    // public float speed;
    // private Vector3 target;
    // private bool selected;
    // public GameObject unit;
    // // private Rigidbody2D body;
    // // private float xInput, yInput;

    // // private bool isMoving;
    // // public static bool dragselected;
    // // public static bool mouseOverPlayer;
    // // private Vector2 mousePosition;
    // // private float dragOffsetX, dragOffsetY;
    
    // // private TilemapCollider2D solid_objects;

    // void Start() 
    // {
    //     speed = 5f;
    //     unitList.Add(this);
    //     target = transform.position;
    //     selected = false;
    //     // body = GetComponent<Rigidbody2D>();
    //     // solid_objects = GetComponent<TilemapCollider2D>();
    //     // isMoving = false;
    //     // dragselected = false;
    //     // mouseOverPlayer = false;
    // }

    // void Update()
    // {
    //     if(Input.GetMouseButtonDown(0) && selected)
    //     {
    //         target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         target.z = transform.position.z;
            
    //     }
        
    //     transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

   
    //         // xInput = Input.mousePosition.x;
    //         // yInput = Input.mousePosition.y;
    //         // isMoving = xInput != 0 || yInput != 0;

    //         // if (body.IsTouchingLayers(solidObjects) && !IsMoving())
    //         // {
    //         //     Debug.Log("Target is 0");
    //         //     transform.position += Vector3.zero;
    //         // }
    //         // else
    //         // {
    //             // Debug.Log("Move Towards target");
    //         // }

            
    //         // xInput = Input.mousePosition.x;
    //         // yInput = Input.mousePosition.y;

    //         // // xInput = Input.GetAxisRaw("Horizontal");
    //         // // yInput = Input.GetAxisRaw("Vertical");


    //         //     isMoving = xInput != 0 || yInput != 0;
        
    //         // if (isMoving)
    //         // {
    //         //     var moveVector = new Vector3(xInput , yInput, 0);   
        
    //         //     // BAD MOVEMENT - Circumvents RigidBody2D Physics
    //         //     // transform.position += moveVector * speed * Time.deltaTime;
    //         //     //
        
    //         //     // CORRECT MOVEMENT
    //         //     body.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
        
    //         //         // animator.SetFloat("xInput", xInput);
    //         //         // animator.SetFloat("yInput", yInput);
    //         // }
        

    //     // if (body.IsTouchingLayers(solidObjects))
    //     // {
    //     //     speed = 0f;
    //     //     Debug.Log("Touching Object.");
    //     // }


    //     // if (Input.GetMouseButtonDown(1))
    //     // {
    //     //     selected = false;
    //     // }

        

    // }

    // // private bool IsMoving()
    // // {
    // //     var previous = transform.position;
    // //     Thread.Sleep(1000);
    // //     var current = transform.position;

    // //     return  previous != current;
    // // }

    // private void isSelected()
    // {
    //     private Vector3 mouse = Input.mousePosition;
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //     }
    // }

    // // private void OnMouseDown()
    // // {
    // //     selected = true;
    // //     // foreach (Unit obj in unitList)
    // //     // {
    // //     //     if (obj != this)
    // //     //     {
    // //     //         obj.selected = false;
    // //     //     }
    // //     // }

    // //     if (Input.GetMouseButtonDown(1))
    // //     {
    // //         selected = false;
    // //     }

    // //     // if (Input.GetKeyDown("left shift") && Input.GetMouseButtonDown(0))
    // //     // {
    // //     //     foreach (UnitController obj in movableObjects)
    // //     //     {
    // //     //         obj.selected = true;
    // //     //     }
    // //     // }
    // // }
}
