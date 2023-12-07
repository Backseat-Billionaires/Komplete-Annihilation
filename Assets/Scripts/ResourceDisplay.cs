using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    public TextMeshProUGUI metalText;
    public TextMeshProUGUI energyText;
    private ResourceManagement resourceManagement;

    void Update()
    {
        // Find the active player's ResourceManagement component
        if (resourceManagement == null)
        {
            var activePlayer = FindObjectOfType<Player>(); // Find the active player
            if (activePlayer != null)
            {
                resourceManagement = activePlayer.GetComponent<ResourceManagement>();
            }
        }

        // Update the HUD display
        if (resourceManagement != null)
        {
            metalText.text = "Metal: " + resourceManagement.Metal;
            energyText.text = "Energy: " + resourceManagement.Energy;
        }
    }
}
