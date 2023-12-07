using UnityEngine;

public class MetalDeposit : MonoBehaviour, IGameSelectable
{
    [SerializeField]
    private GameObject selectedIndicator; // Assign in Inspector
    private bool isSelected = false;
    public bool HasMine { get; private set; } = false;

    private GameController gameController;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        if (selectedIndicator != null)
            selectedIndicator.SetActive(false); // Ensure the indicator is not visible initially
    }

    public void Select()
    {
        isSelected = true;
        gameController.AddToSelectedObjects(this);
        if (selectedIndicator != null)
            selectedIndicator.SetActive(true); // Show selection indicator
    }

    public void Deselect()
    {
        isSelected = false;
        gameController.RemoveFromSelectedObjects(this);
        if (selectedIndicator != null)
            selectedIndicator.SetActive(false); // Hide selection indicator
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void PlaceMine(GameObject minePrefab, Player owner)
    {
        if (!HasMine)
        {
            GameObject newMineObject = Instantiate(minePrefab, transform.position, Quaternion.identity);
            Mine newMine = newMineObject.GetComponent<Mine>();

            if (newMine != null)
            {
                newMine.Initialize(owner, this);
                HasMine = true;
            }
            else
            {
                Debug.LogError("Mine component not found on minePrefab");
            }
        }
    }
}
