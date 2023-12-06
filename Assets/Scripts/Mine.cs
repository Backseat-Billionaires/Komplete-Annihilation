using UnityEngine;

public class Mine : MonoBehaviour, IGameSelectable
{
    public int metalPerSecond = 1; // The amount of metal generated per second
    public int maxHealth = 100; // Maximum health of the mine
    public Player owner; // The player who owns the mine

    private ResourceManagement resourceManagement; // Reference to the ResourceManagement script
    private int currentHealth;
    private bool isSelected = false;
    
    void Start()
    {
        resourceManagement = owner.GetComponent<ResourceManagement>();
        if (resourceManagement == null)
        {
            Debug.LogError("ResourceManagement component not found in the owner player.");
            return;
        }

        currentHealth = maxHealth; // Initialize the mine's health
        InvokeRepeating(nameof(GenerateResource), 1f, 1f); // Start generating resources every second
    }

    void GenerateResource()
    {
        resourceManagement.AddMetal(metalPerSecond);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            DestroyMine();
        }
    }

    private void DestroyMine()
    {
        // Optional: Add logic for when the mine is destroyed, like playing an animation or effect
        Destroy(gameObject); // Remove the mine from the game
    }

    // IGameSelectable implementation
    public void Select()
    {
        isSelected = true;
        // Optional: Add visual indication of selection, like highlighting
    }

    public void Deselect()
    {
        isSelected = false;
        // Optional: Remove visual indication of selection
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}
