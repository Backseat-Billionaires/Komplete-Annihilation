using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private Camera mainCamera;
    private Selectable currentlySelected;

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

            if (Physics.Raycast(ray, out hit))
            {
                Selectable selectableObject = hit.collider.GetComponent<Selectable>();
                if (selectableObject != null)
                {
                    if (currentlySelected != null)
                    {
                        currentlySelected.SetSelected(false);
                    }

                    currentlySelected = selectableObject;
                    selectableObject.SetSelected(true);
                }
            }
        }
    }
}