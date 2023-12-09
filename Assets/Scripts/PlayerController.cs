using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private float movementSpeed = 5f;
    [SerializeField]
    private LayerMask interactableLayer; // Define a LayerMask for interactable objects
    [SerializeField]
    private int attackDamage = 10; // Damage dealt by the player's attack
    [SerializeField]
    private float attackRange = 2f; // Range of the player's attack

    private Camera playerCamera;
    private MetalDeposit selectedMetalDeposit;
    private CharacterController characterController;
    private Health healthComponent;
    private Map map; // Reference to the Map class for spawn points

    void Start()
    {
        if (isLocalPlayer)
        {
            playerCamera = Camera.main;
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                Debug.LogError("CharacterController is not attached to the player");
            }
        }

        healthComponent = GetComponent<Health>();
        if (healthComponent == null)
        {
            Debug.LogError("Health component not found on the player");
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
        HandleInteractionInput();
        HandleMinePlacementInput();
        HandleAttackInput();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float adjustedSpeed = movementSpeed * Time.deltaTime;
        Vector3 movement = new Vector3(horizontal, 0, vertical) * adjustedSpeed;
        characterController.Move(movement);
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Assuming 'E' is the key for interaction
        {
            TryInteract();
        }
    }

    private void HandleMinePlacementInput()
    {
        if (Input.GetKeyDown(KeyCode.M) && selectedMetalDeposit != null) // Assuming 'M' is the key for mine placement
        {
            CmdTryPlaceMine(selectedMetalDeposit.gameObject);
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Assuming 'F' is the key for attack
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, attackRange, interactableLayer))
        {
            Mine mine = hit.collider.GetComponent<Mine>();
            if (mine != null)
            {
                CmdAttackMine(mine.gameObject);
            }
        }
    }

    
    [Command]
    private void CmdAttackMine(GameObject mineObj)
    {
        Mine mine = mineObj.GetComponent<Mine>();
        if (mine != null)
        {
            Health mineHealth = mine.GetComponent<Health>();
            if (mineHealth != null)
            {
                mineHealth.TakeDamage(attackDamage, gameObject); // gameObject is the attacker
            }
        }
    }

    private void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f, interactableLayer))
        {
            MetalDeposit metalDeposit = hit.collider.GetComponent<MetalDeposit>();
            Mine mine = hit.collider.GetComponent<Mine>();

            if (metalDeposit != null && IsWithinInteractRange(metalDeposit.gameObject))
            {
                CmdInteractWithMetalDeposit(metalDeposit.gameObject);
                selectedMetalDeposit = metalDeposit; // Store the selected metal deposit
            }
            else if (mine != null && IsWithinInteractRange(mine.gameObject))
            {
                CmdInteractWithMine(mine.gameObject);
            }
        }
    }

    private bool IsWithinInteractRange(GameObject target)
    {
        float interactRange = 5.0f; // Define the interaction range
        return Vector3.Distance(transform.position, target.transform.position) <= interactRange;
    }

    [Command]
    private void CmdInteractWithMetalDeposit(GameObject obj)
    {
        MetalDeposit metalDeposit = obj.GetComponent<MetalDeposit>();
        if (metalDeposit != null)
        {
            metalDeposit.Select(gameObject); // Passing the player GameObject as the owner
        }
    }

    [Command]
    private void CmdInteractWithMine(GameObject obj)
    {
        Mine mine = obj.GetComponent<Mine>();
        if (mine != null)
        {
            mine.Select(); // Or any other specific interaction logic
        }
    }

    [Command]
    private void CmdTryPlaceMine(GameObject metalDepositObj)
    {
        MetalDeposit metalDeposit = metalDepositObj.GetComponent<MetalDeposit>();
        if (metalDeposit != null && metalDeposit.gameObject == selectedMetalDeposit.gameObject)
        {
            metalDeposit.TryPlaceMine(gameObject); // Attempt to place a mine
        }
    }




    
}
