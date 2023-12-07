using UnityEngine;
using Mirror;

public class MetalDeposit : NetworkBehaviour, IGameSelectable
{
    [SerializeField]
    private GameObject selectedIndicator;
    private bool isSelected = false;

    [SyncVar]
    public bool HasMine = false;

    private void Start()
    {
        if (selectedIndicator != null)
            selectedIndicator.SetActive(false);
    }

    public void Select()
    {
        isSelected = true;
        if (selectedIndicator != null)
            selectedIndicator.SetActive(true);
    }

    public void Deselect()
    {
        isSelected = false;
        if (selectedIndicator != null)
            selectedIndicator.SetActive(false);
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    [Server]
    public void PlaceMine(GameObject minePrefab, GameObject owner)
    {
        if (!HasMine)
        {
            GameObject newMineObject = Instantiate(minePrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(newMineObject);
            Mine newMine = newMineObject.GetComponent<Mine>();

            if (newMine != null)
            {
                newMine.Initialize(owner);
                HasMine = true;
            }
            else
            {
                Debug.LogError("Mine component not found on minePrefab");
            }
        }
    }
}