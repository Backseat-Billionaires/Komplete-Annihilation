using UnityEngine;

public class MetalDeposit : MonoBehaviour, IGameSelectable
{
    [SerializeField]
    private GameObject selectedIndicator; // Assign a GameObject to visually indicate selection
    public bool IsSelected { get; private set; } = false;
    public bool HasMine { get; private set; } = false;

    private void Start()
    {
        if (selectedIndicator != null)
            selectedIndicator.SetActive(false); // Ensure the indicator is not visible initially
    }

    public void Select()
    {
        IsSelected = true;
        if (selectedIndicator != null)
            selectedIndicator.SetActive(true); // Show selection indicator
    }

    public void Deselect()
    {
        IsSelected = false;
        if (selectedIndicator != null)
            selectedIndicator.SetActive(false); // Hide selection indicator
    }

    public void PlaceMine(Mine minePrefab, Player owner)
    {
        if (!HasMine)
        {
            Mine newMine = Instantiate(minePrefab, transform.position, Quaternion.identity);
            newMine.Initialize(owner, this); // Initialize the mine with the owner and this deposit
            HasMine = true;
        }
    }
}
