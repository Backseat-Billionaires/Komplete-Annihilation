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
    public TextMeshProUGUI currentLivesText;
    public TextMeshProUGUI deathCountText;
    public TextMeshProUGUI scoreText;

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

    public void SetPlayerComponents(PlayerInventory inventory, PlayerController controller, PlayerWeapons playerWeapons, Health health)
    {
        playerInventory = inventory;
        playerController = controller;
        weapons = playerWeapons;
        healthComponent = health;

        healthBar.SetMaxHealth(health.GetMaxHealth());
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
        bool healthChanged = healthComponent != null && healthComponent.GetCurrentHealth() != lastHealth;
        bool metalCountChanged = playerInventory != null && playerInventory.GetResourceCount() != lastMetalCount;
        bool mineCountChanged = playerInventory != null && playerInventory.GetActiveMines() != lastMineCount;
        bool bulletCountChanged = playerInventory != null && playerInventory.GetBulletCount() != lastBulletCount;
        bool livesChanged = healthComponent != null && healthComponent.GetCurrentLives() != lastCurrentLives;
        bool deathCountChanged = healthComponent != null && healthComponent.GetDeathCount() != lastDeathCount;
        bool scoreChanged = healthComponent != null && healthComponent.GetScore() != lastScore;
        bool weaponChanged = weapons != null && weapons.GetHighestLevelWeaponInfo().Item1 != lastWeaponInfo;

        return healthChanged || metalCountChanged || mineCountChanged || bulletCountChanged || livesChanged || deathCountChanged || scoreChanged || weaponChanged;
    }


    private void UpdateHUD()
    {
        var weaponInfo = weapons.GetHighestLevelWeaponInfo();
        lastWeaponInfo = $"{weaponInfo.Item1}, Level: {weaponInfo.Item2}";

        lastHealth = healthComponent.GetCurrentHealth();
        healthBar.SetHealth(lastHealth);

        lastMetalCount = playerInventory.GetResourceCount();
        metalCountText.text = $"Metal: {lastMetalCount}";

        lastMineCount = playerInventory.GetActiveMines();
        mineCountText.text = $"Mines: {lastMineCount} / {PlayerInventory.MaxActiveMinesPerPlayer}";

        lastBulletCount = playerInventory.GetBulletCount();
        bulletCountText.text = $"Bullets: {lastBulletCount}";

        lastCurrentLives = healthComponent.GetCurrentLives();
        currentLivesText.text = $"Lives: {lastCurrentLives}";

        lastDeathCount = healthComponent.GetDeathCount();
        deathCountText.text = $"Death Count: {lastDeathCount} / {healthComponent.GetMaxRespawns()}";

        lastScore = healthComponent.GetScore();
        scoreText.text = $"Score: {lastScore}";
    }
}
