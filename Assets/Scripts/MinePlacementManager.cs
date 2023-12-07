using UnityEngine;
using Mirror;

public class MinePlacementManager : NetworkBehaviour
{
    public GameObject minePrefab; // Assign in Inspector

    // This method attempts to place a mine on a selected metal deposit.
    [Server]
    public void TryPlaceMine(GameObject selectedMetalDeposit)
    {
        if (selectedMetalDeposit == null)
        {
            Debug.LogError("Selected metal deposit is null.");
            return;
        }

        var resourceManagement = GetComponent<ResourceManagement>();
        if (resourceManagement == null)
        {
            Debug.LogError("ResourceManagement component not found.");
            return;
        }

        // Check if enough resources are available to place a mine
        if (!resourceManagement.CanAffordMineCost())
        {
            Debug.Log("Not enough resources to place a mine.");
            return;
        }

        var metalDeposit = selectedMetalDeposit.GetComponent<MetalDeposit>();
        if (metalDeposit == null || metalDeposit.HasMine)
        {
            Debug.Log("Invalid metal deposit or a mine is already placed here.");
            return;
        }

        // Spend resources for placing the mine
        resourceManagement.SpendResourcesForMine();

        // Instantiate and place the mine
        var mineObject = Instantiate(minePrefab, metalDeposit.transform.position, Quaternion.identity);
        NetworkServer.Spawn(mineObject, connectionToClient);

        var mine = mineObject.GetComponent<Mine>();
        if (mine != null)
        {
            mine.Initialize(this.gameObject); // Initialize the mine with this player as the owner
        }
        else
        {
            Debug.LogError("Mine component not found on minePrefab");
        }

        metalDeposit.HasMine = true;
    }
}