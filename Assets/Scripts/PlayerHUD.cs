using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health Bar")]
    public HealthBar healthBar;

    [Header("Resource Indicators")]
    public TextMeshProUGUI metalCountText;
    public TextMeshProUGUI mineCountText;
    public TextMeshProUGUI bulletCountText;
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI currentLives;
    public TextMeshProUGUI deathCount;
    public TextMeshProUGUI score;

    private PlayerInventory playerInventory;
    private PlayerController playerController;
    private PlayerWeapons weapons;
    private Health healthComponent;

    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerController = GetComponent<PlayerController>();
        weapons = GetComponent<PlayerWeapons>();
        healthComponent = GetComponent<Health>();

        if (playerInventory == null || playerController == null || weapons == null || healthComponent == null)
        {
            Debug.LogError("Essential components (PlayerInventory, PlayerController, PlayerWeapons, Health) not found on the player object");
            return;
        }
        
        healthBar.SetMaxHealth(healthComponent.GetMaxHealth());
        UpdateHUD(); 
    }

    void Update()
    {
        if (CheckForUpdates())
        {
            UpdateHUD();
        }
    }

    private bool CheckForUpdates()
    {
        return healthComponent.GetCurrentHealth() != healthBar.GetCurrentHealth() ||
               playerInventory.GetResourceCount().ToString() != metalCountText.text ||
               playerInventory.GetActiveMines().ToString() != mineCountText.text ||
               playerInventory.GetBulletCount().ToString() != bulletCountText.text ||
               healthComponent.GetCurrentLives().ToString() != currentLives.text ||
               healthComponent.GetDeathCount().ToString() != deathCount.text ||
               healthComponent.GetScore().ToString() != score.text ||
               weapons.GetHighestLevelWeaponInfo().Item1 != weaponText.text;
    }


    private void UpdateHUD()
    {
        healthBar.SetHealth(healthComponent.GetCurrentHealth());
        metalCountText.text = $"Metal: {playerInventory.GetResourceCount()}";
        mineCountText.text = $"Mines: {playerInventory.GetActiveMines()} / {PlayerInventory.MaxActiveMinesPerPlayer}";
        bulletCountText.text = $"Bullets: {playerInventory.GetBulletCount()}";

        var weaponInfo = weapons.GetHighestLevelWeaponInfo();
        weaponText.text = $"Weapon: {weaponInfo.Item1}, Level: {weaponInfo.Item2}";

        currentLives.text = $"Lives: {healthComponent.GetCurrentLives()}";
        deathCount.text = $"Death Count: {healthComponent.GetDeathCount()}/{healthComponent.GetMaxRespawns()}";
        score.text = $"Score: {healthComponent.GetScore()}";
    }
}
