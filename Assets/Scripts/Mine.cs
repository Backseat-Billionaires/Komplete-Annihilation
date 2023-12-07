using UnityEngine;

public class Mine : MonoBehaviour, IGameSelectable
{
    public int metalPerSecond = 1;
    public int maxHealth = 100;
    private int currentHealth;
    private Player owner;
    private ResourceManagement resourceManagement;
    private bool isSelected = false;

    [SerializeField]
    private GameObject selectedIndicator; // Assign in Inspector

    void Start()
    {
        resourceManagement = owner.GetComponent<ResourceManagement>();
        if (resourceManagement == null)
        {
            Debug.LogError("ResourceManagement component not found in the owner player.");
            return;
        }

        currentHealth = maxHealth;
        InvokeRepeating(nameof(GenerateResource), 1f, 1f);

        if (selectedIndicator != null)
            selectedIndicator.SetActive(false); // Initial state
    }

    void GenerateResource()
    {
        resourceManagement.AddMetal(metalPerSecond);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            DestroyMine();
    }

    private void DestroyMine()
    {
        Destroy(gameObject);
    }

    // Initialize the mine with the owner and metal deposit
    public void Initialize(Player owner, MetalDeposit deposit)
    {
        this.owner = owner;
    }

    // IGameSelectable implementation
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
}
