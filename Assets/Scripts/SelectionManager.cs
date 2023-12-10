using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private Camera playerCamera;
    private Selectable currentlySelected;
    public LayerMask selectableLayer; // Layer for selectable objects

    public void SetCamera(Camera camera)
    {
        playerCamera = camera;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && playerCamera != null) // Left mouse button
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
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
                    currentlySelected.SetSelected(false);
                    currentlySelected = null;
                }
            }
        }
    }
}