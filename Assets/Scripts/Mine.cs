using UnityEngine;
using Mirror;

public class Mine : NetworkBehaviour, IGameSelectable
{
    public int metalPerSecond = 1;
    public int maxHealth = 100;
    private int currentHealth;

    [SyncVar]
    private GameObject ownerGameObject;

    private ResourceManagement resourceManagement;
    private bool isSelected = false;

    [SerializeField]
    private GameObject selectedIndicator;

    void Start()
    {
        currentHealth = maxHealth;

        if (ownerGameObject != null)
        {
            resourceManagement = ownerGameObject.GetComponent<ResourceManagement>();
        }

        if (resourceManagement == null)
        {
            Debug.LogError("ResourceManagement component not found in the owner player.");
            return;
        }

        InvokeRepeating(nameof(GenerateResource), 1f, 1f);

        if (selectedIndicator != null)
            selectedIndicator.SetActive(false);
    }

    void GenerateResource()
    {
        if (isServer && resourceManagement != null)
        {
            resourceManagement.AddMetal(metalPerSecond);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && isServer)
        {
            DestroyMine();
        }
    }

    [Server]
    private void DestroyMine()
    {
        NetworkServer.Destroy(gameObject);
    }

    public void Initialize(GameObject ownerGameObject)
    {
        this.ownerGameObject = ownerGameObject;
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
}