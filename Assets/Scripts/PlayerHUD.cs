using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health Bar")]
    public HealthBar healthBar;

    [Header("Resource Indicators")]
    public Text metalCountText;
    public Text mineCountText;
    public Text bulletCountText;
    public Text weaponText;
    public Text currentLives;
    public Text deathCount;
    public Text score;

    private PlayerInventory playerInventory;
    private PlayerController playerController;
    private PlayerWeapons weapons;
    private Health healthComponent;

    // Variables for tracking changes
    private int lastHealth;
    private int lastMetalCount;
    private int lastMineCount;
    private int lastBulletCount;
    private string lastWeaponInfo;
    private int lastCurrentLives;
    private int lastDeathCount;
    private int lastScore;

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
        UpdateHUD(); // Initial update
    }

    void Update()
    {
        if (healthComponent.GetCurrentHealth() != lastHealth ||
            playerInventory.GetResourceCount() != lastMetalCount ||
            playerInventory.GetActiveMines() != lastMineCount ||
            playerInventory.GetBulletCount() != lastBulletCount ||
            healthComponent.GetCurrentLives() != lastCurrentLives ||
            healthComponent.GetDeathCount() != lastDeathCount ||
            healthComponent.GetScore() != lastScore ||
            weapons.GetHighestLevelWeaponInfo().Item1 != lastWeaponInfo)
        {
            UpdateHUD();
        }
    }

    private void UpdateHUD() 
    {
        (string weaponName, int weaponLevel) = weapons.GetHighestLevelWeaponInfo();
        lastWeaponInfo = $"{weaponName}, Level: {weaponLevel}";

        lastHealth = healthComponent.GetCurrentHealth();
        healthBar.SetHealth(lastHealth);

        lastMetalCount = playerInventory.GetResourceCount();
        metalCountText.text = $"Metal: {lastMetalCount}";

        lastMineCount = playerInventory.GetActiveMines();
        mineCountText.text = $"Mines: {lastMineCount} / {PlayerInventory.MaxActiveMinesPerPlayer}";

        lastBulletCount = playerInventory.GetBulletCount();
        bulletCountText.text = $"Bullets: {lastBulletCount}";

        lastCurrentLives = healthComponent.GetCurrentLives();
        currentLives.text = $"Lives: {lastCurrentLives}";

        lastDeathCount = healthComponent.GetDeathCount();
        deathCount.text = $"Death Counter: {lastDeathCount} / {healthComponent.GetMaxRespawns()}";

        lastScore = healthComponent.GetScore();
        score.text = $"Score: {lastScore}";
    }

    // Additional methods for HUD updates can be added here
}
