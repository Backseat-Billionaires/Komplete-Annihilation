using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    private List<IGameSelectable> selectedObjects = new List<IGameSelectable>();

    public void SelectObject(GameObject obj, bool multiSelect = false)
    {
        IGameSelectable selectable = obj.GetComponent<IGameSelectable>();
        if (selectable != null)
        {
            if (!multiSelect)
            {
                DeselectAllExcept(obj); // Deselect all other objects if not multi-selecting
            }

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

    private void DeselectAllExcept(GameObject obj)
    {
        foreach (var selectable in new List<IGameSelectable>(selectedObjects)) // Create a copy of the list to iterate over
        {
            Component selectableComponent = selectable as Component;
            if (selectableComponent != null && selectableComponent.gameObject != obj)
            {
                selectable.Deselect();
            }
        }
        selectedObjects.RemoveAll(s => ((Component)s).gameObject != obj);
    }

    private void EnforceSelectionRules()
    {
        // If a mine is selected, deselect any other selectable objects
        bool mineSelected = selectedObjects.Exists(obj => ((Component)obj).gameObject.GetComponent<Mine>() != null);
        if (mineSelected)
        {
            var othersToDeselect = selectedObjects.FindAll(obj => !(((Component)obj).gameObject.GetComponent<Mine>() != null));
            foreach (var selectable in othersToDeselect)
            {
                selectable.Deselect();
                selectedObjects.Remove(selectable);
            }
        }

        // If a metal deposit is selected, make sure only one can be selected at a time
        var selectedDeposits = selectedObjects.FindAll(obj => ((Component)obj).gameObject.GetComponent<MetalDeposit>() != null);
        if (selectedDeposits.Count > 1)
        {
            for (int i = 0; i < selectedDeposits.Count - 1; i++)
            {
                selectedDeposits[i].Deselect();
                selectedObjects.Remove(selectedDeposits[i]);
            }
        }
    }

    public void DeselectAll()
    {
        foreach (var selectable in selectedObjects)
        {
            selectable.Deselect();
        }
        selectedObjects.Clear();
    }

    public void DeselectObject(GameObject obj)
    {
        IGameSelectable selectable = obj.GetComponent<IGameSelectable>();
        if (selectable != null && selectable.IsSelected())
        {
            selectable.Deselect();
            selectedObjects.Remove(selectable);
        }
    }
}
