using UnityEngine;
using Mirror;

public class MetalDeposit : NetworkBehaviour
{
    [SerializeField]
    private GameObject minePrefab;
    [SyncVar]
    private bool hasMine = false;

    private GameObject _mineOwner; 
    private Selectable selectableComponent;

    private const int mineCost = 100; 

    private void Awake()
    {
        selectableComponent = GetComponent<Selectable>();
        if (selectableComponent == null)
        {
            Debug.LogError("Selectable component not found on the MetalDeposit object");
        }
    }

    [Server]
    public void TryPlaceMine(GameObject owner)
    {
        if (!isOwned)
        {
            Debug.LogError("Attempted to place mine without authority.");
            return;
        }

        PlayerInventory ownerInventory = owner.GetComponent<PlayerInventory>();
        if (ownerInventory == null)
        {
            Debug.LogError("PlayerInventory component not found on mine owner");
            return;
        }

        if (ownerInventory.GetActiveMines() >= PlayerInventory.MaxActiveMinesPerPlayer)
        {
            // Provide feedback to the player
            SendFeedbackToPlayer(owner, "Maximum number of active mines reached.");
            return;
        }

        if (!IsOwnerInRange(owner) || !ownerInventory.HasEnoughResources(mineCost))
        {
            // Provide feedback to the player
            SendFeedbackToPlayer(owner, "Not enough resources or not in range to place a mine.");
            return;
        }

        if (!hasMine)
        {
            ownerInventory.UseResources(mineCost); 
            GameObject newMineObject = Instantiate(minePrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(newMineObject, owner);
            Mine newMine = newMineObject.GetComponent<Mine>();

            if (newMine != null)
            {
                newMine.Initialize(owner, transform.position);
                hasMine = true;
                ownerInventory.IncrementActiveMines(); 
            }
            else
            {
                Debug.LogError("Mine component not found on minePrefab");
            }
        }
    }

    private bool IsOwnerInRange(GameObject owner)
    {
        float placementRange = 5.0f; 
        return Vector3.Distance(owner.transform.position, transform.position) <= placementRange;
    }

    private void SendFeedbackToPlayer(GameObject player, string message)
    {

        Debug.Log(message);
    }

    public void Select(GameObject owner)
    {
        if (owner == _mineOwner)
        {
            selectableComponent.SetSelected(true); 
        }
    }

    public void Deselect()
    {
        selectableComponent.SetSelected(false); 
    }
}
