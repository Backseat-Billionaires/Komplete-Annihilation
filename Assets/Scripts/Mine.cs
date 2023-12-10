using UnityEngine;
using Mirror;
using System.Collections;

public class Mine : NetworkBehaviour
{
    [SerializeField]
    private GameObject mineVisualPrefab; 

    [SyncVar]
    private GameObject owner;
    
    private const float resourceGenerationRate = 1.0f; 

    private Health healthComponent;
    private GameObject visualIndicator; 
    
    private Selectable selectableComponent; 
    
    private bool isSelected;
    
    void OnMouseDown() 
    {
        
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
                    ownerInventory.AddResources(1); 
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

        visualIndicator = visualInstance; 
    }

    private void HandleMineDestruction(GameObject attacker)
    {
        
        if (owner != null)
        {
            PlayerInventory ownerInventory = owner.GetComponent<PlayerInventory>();
            if (ownerInventory != null)
            {
                ownerInventory.DecrementActiveMines();
            }
        }

      
        NetworkServer.Destroy(gameObject);
    }

    public void Select()
    {
        if (selectableComponent != null)
        {
            selectableComponent.SetSelected(true); 
            visualIndicator.SetActive(true);
        }
    }

    public void Deselect()
    {
        if (selectableComponent != null)
        {
            selectableComponent.SetSelected(false); 
            visualIndicator.SetActive(false);
        }
    }
    
    [Command]
    private void CmdToggleSelection() 
    {
        
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
