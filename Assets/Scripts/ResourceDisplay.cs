using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    public ResourceManagement resourceManagement;
    public TextMeshProUGUI metalText;
    public TextMeshProUGUI energyText;

    void Update()
    {
        if (resourceManagement != null)
        {
            metalText.text = "Metal: " + resourceManagement.Metal;
            energyText.text = "Energy: " + resourceManagement.Energy;
        }
    }
}
