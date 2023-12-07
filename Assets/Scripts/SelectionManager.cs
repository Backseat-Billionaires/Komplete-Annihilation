using UnityEngine;
using System.Collections.Generic;


public class SelectionManager : MonoBehaviour
{
    private List<IGameSelectable> selectedObjects = new List<IGameSelectable>();

    public void SelectObject(GameObject obj)
    {
        IGameSelectable selectable = obj.GetComponent<IGameSelectable>();
        if (selectable != null)
        {
            if (selectable.IsSelected())
            {
                selectable.Deselect();
                selectedObjects.Remove(selectable);
            }
            else
            {
                selectable.Select();
                selectedObjects.Add(selectable);
            }
        }
        EnforceSelectionRules();
    }

    private void EnforceSelectionRules()
    {
        // Logic to enforce selection rules
    }

    public void DeselectAll()
    {
        foreach (var selectable in selectedObjects)
        {
            selectable.Deselect();
        }
        selectedObjects.Clear();
    }
}