using UnityEngine;
using Mirror;
using System.Collections;

public class Mine : NetworkBehaviour
{
    [SerializeField]
    private GameObject mineVisualPrefab; // Prefab for the visual representation

    [SyncVar]
    private GameObject owner;

    private const int cost = 100; // Cost to place the mine
    private const float resourceGenerationRate = 1.0f; // 1 metal resource per second

    private Health healthComponent;
    private GameObject visualIndicator; // For selection indication
    
    private Selectable selectableComponent; // Reference to the Selectable component
    public static int MaxActiveMinesPerPlayer = 8; // Maximum number of active mines a player can have

    public override void OnStartServer()
    {
        healthComponent = GetComponent<Health>();
        if (healthComponent == null)
        {
            Debug.LogError("Health component not found on mine");
        }
        else
        {
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

    public void Select()
    {
        if (selectableComponent != null)
        {
            selectableComponent.SetSelected(true); // Select using the Selectable component
        }
    }


    public void Deselect()
    {
        if (selectableComponent != null)
        {
            selectableComponent.SetSelected(false); // Deselect using the Selectable component
        }
    }

    // Additional methods or properties can be added here
}
