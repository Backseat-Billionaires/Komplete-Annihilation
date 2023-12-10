using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField]
    private GameObject selectedIndicator; 

    private bool isSelected = false; 

    private void Start()
    {
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(false);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(selected);
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }
    
}