using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private float movementSpeed = 5f;
    [SerializeField]
    private LayerMask interactableLayer;
    [SerializeField]
    private LayerMask enemyLayer; 

    private Camera playerCamera;
    private MetalDeposit selectedMetalDeposit;
    private CharacterController characterController;
    private Health healthComponent;
    private PlayerWeapons playerWeapons;

    void Start()
    {
        if (isLocalPlayer)
        {
            playerCamera = Camera.main;
            characterController = GetComponent<CharacterController>();
            playerWeapons = GetComponent<PlayerWeapons>();

            if (characterController == null)
            {
                Debug.LogError("CharacterController is not attached to the player");
            }
            if (playerWeapons == null)
            {
                Debug.LogError("PlayerWeapons component not found on the player");
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
        if (!isLocalPlayer || healthComponent.GetCurrentHealth() <= 0) return;

        HandleMovement();
        HandleInteractionInput();
        HandleMinePlacementInput();
        HandleMouseClickAttackInput();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical) * (movementSpeed * Time.deltaTime);
        characterController.Move(movement);
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E key for interaction
        {
            TryInteract();
        }
    }

    private void HandleMinePlacementInput()
    {
        if (Input.GetKeyDown(KeyCode.M) && selectedMetalDeposit != null) // M key for mine placement
        {
            CmdTryPlaceMine(selectedMetalDeposit.gameObject);
        }
    }

    public void HandleMouseClickAttackInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, 100f, interactableLayer))
            {
                NetworkIdentity hitIdentity = hit.collider.gameObject.GetComponent<NetworkIdentity>();
                Mine hitMine = hit.collider.gameObject.GetComponent<Mine>();
                
                if (hitMine != null && hitIdentity != null && hitIdentity.connectionToClient != connectionToClient)
                {
                    AttackTarget(hit.collider.gameObject);
                }
                else if (hitIdentity != null && hitIdentity.connectionToClient != connectionToClient)
                {
                    AttackTarget(hit.collider.gameObject);
                }
            }
        }
    }

    private void AttackTarget(GameObject target)
    {
        // Get the currently equipped weapon
        Weapon currentWeapon = playerWeapons.GetCurrentWeapon();

        // Fire the weapon
        playerWeapons.FireWeapon(currentWeapon.weaponName);

        // Attack the target
        CmdAttack(target);
    }




    [Command]
    private void CmdAttack(GameObject target)
    {
        Weapon currentWeapon = playerWeapons.GetCurrentWeapon();
        string weaponName = currentWeapon.weaponName;

        if (playerWeapons.HasAmmo(weaponName))
        {
            CombatManager.Instance.PerformAttack(gameObject, target, currentWeapon);
        }
        else 
        {
            
            playerWeapons.ReloadWeapon(weaponName); 
        }
    }


    private void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f, interactableLayer))
        {
            HandleInteraction(hit.collider.gameObject);
        }
    }

    private void HandleInteraction(GameObject hitObject)
    {
        MetalDeposit metalDeposit = hitObject.GetComponent<MetalDeposit>();
        Mine mine = hitObject.GetComponent<Mine>();

        if (metalDeposit != null && IsWithinInteractRange(hitObject))
        {
            CmdInteractWithMetalDeposit(metalDeposit.gameObject);
            selectedMetalDeposit = metalDeposit;
        }
        else if (mine != null && IsWithinInteractRange(hitObject))
        {
            CmdInteractWithMine(mine.gameObject);
        }
    }

    private bool IsWithinInteractRange(GameObject target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= 5f;
    }

    [Command]
    private void CmdInteractWithMetalDeposit(GameObject obj)
    {
        MetalDeposit metalDeposit = obj.GetComponent<MetalDeposit>();
        if (metalDeposit != null)
        {
            metalDeposit.Select(gameObject); 
        }
    }

    [Command]
    private void CmdInteractWithMine(GameObject obj)
    {
        Mine mine = obj.GetComponent<Mine>();
        if (mine != null)
        {
            mine.Select(); 
        }
    }

    [Command]
    private void CmdTryPlaceMine(GameObject metalDepositObj)
    {
        MetalDeposit metalDeposit = metalDepositObj.GetComponent<MetalDeposit>();
        if (metalDeposit != null && metalDeposit.gameObject == selectedMetalDeposit.gameObject)
        {
            metalDeposit.TryPlaceMine(gameObject); 
        }
    }




    
}
