using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private Camera mainCamera;
    private Selectable currentlySelected;
    public LayerMask selectableLayer; // Layer for selectable objects

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
            {
                Selectable selectableObject = hit.collider.GetComponent<Selectable>();
                if (selectableObject != null)
                {
                    if (currentlySelected != null && currentlySelected != selectableObject)
                    {
                        currentlySelected.SetSelected(false);
                    }

                    currentlySelected = selectableObject;
                    selectableObject.SetSelected(true);
                }
                else if (currentlySelected != null)
                {
                    // Clicked on non-selectable area, deselect current
                    currentlySelected.SetSelected(false);
                    currentlySelected = null;
                }
            }
        }
    }
}