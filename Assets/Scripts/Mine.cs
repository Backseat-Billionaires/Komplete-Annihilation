using UnityEngine;
using Mirror;
using System.Collections;

public class Mine : NetworkBehaviour
{
    [SerializeField]
    private GameObject mineVisualPrefab; // Prefab for the visual representation

    [SyncVar]
    private GameObject owner;
    
    private const float resourceGenerationRate = 1.0f; // 1 metal resource per second

    private Health healthComponent;
    private GameObject visualIndicator; // For selection indication
    
    private Selectable selectableComponent; // Reference to the Selectable component
    
    private bool isSelected;
    
    void OnMouseDown() 
    {
        // Only allow selection interactions for the local player
        if (!isOwned) return;

        CmdToggleSelection();
    }

    public override void OnStartServer()
    {
        healthComponent = GetComponent<Health>();
        if (healthComponent == null)
        {
            Debug.LogError("Health component not found on mine");
        }
        else
        {
            healthComponent.OnDeath += HandleMineDestruction;
            StartCoroutine(GenerateResource());
        }
        
        selectableComponent = GetComponent<Selectable>();
        if (selectableComponent == null)
        {
            Debug.LogError("Selectable component not found on the Mine object");
        }
    }

    private IEnumerator GenerateResource()
    {
        while (healthComponent != null && healthComponent.GetCurrentHealth() > 0)
        {
            yield return new WaitForSeconds(resourceGenerationRate);
            if (owner != null)
            {
                PlayerInventory ownerInventory = owner.GetComponent<PlayerInventory>();
                if (ownerInventory != null)
                {
                    ownerInventory.AddResources(1); // Add 1 metal resource per second
                }
            }
        }
    }

    [Server]
    public void Initialize(GameObject mineOwner, Vector3 placementPosition)
    {
        owner = mineOwner;
        transform.position = placementPosition;

        GameObject visualInstance = Instantiate(mineVisualPrefab, placementPosition, Quaternion.identity);
        NetworkServer.Spawn(visualInstance);

        visualIndicator = visualInstance; // Store reference for selection indication
    }

    private void HandleMineDestruction(GameObject attacker)
    {
        // Notify the owner's inventory to decrement the active mine count
        if (owner != null)
        {
            PlayerInventory ownerInventory = owner.GetComponent<PlayerInventory>();
            if (ownerInventory != null)
            {
                ownerInventory.DecrementActiveMines();
            }
        }

        // Destroy the mine object
        NetworkServer.Destroy(gameObject);
    }

    public void Select()
    {
        if (selectableComponent != null)
        {
            selectableComponent.SetSelected(true); // Select using the Selectable component
            visualIndicator.SetActive(true);
        }
    }

    public void Deselect()
    {
        if (selectableComponent != null)
        {
            selectableComponent.SetSelected(false); // Deselect using the Selectable component
            visualIndicator.SetActive(false);
        }
    }
    
    [Command]
    private void CmdToggleSelection()
    {
        //Toggle selection state on the server
        if (isSelected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }
    
}
