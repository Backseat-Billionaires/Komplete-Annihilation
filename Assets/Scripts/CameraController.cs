using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float edgePanMultiplier = 1f;
    public float zoomSpeed = 30f;
    public float minZoom = 15f;
    public Vector2 panLimitMin = new(0f, 0f);
    public Vector2 panLimitMax = new(192f, -192f);
    public Player activePlayer;

    private Camera mainCamera;
    private Vector3 lastMousePosition;
    private float maxZoom;
    private Vector2 boundaryMin;
    private Vector2 boundaryMax;

    void Start()
    {
        mainCamera = Camera.main;

        // Calculate boundaries
        boundaryMin = new Vector2(Mathf.Min(panLimitMin.x, panLimitMax.x), Mathf.Min(panLimitMin.y, panLimitMax.y));
        boundaryMax = new Vector2(Mathf.Max(panLimitMin.x, panLimitMax.x), Mathf.Max(panLimitMin.y, panLimitMax.y));

        // Calculate max zoom
        var maxZoomX = Mathf.Abs(boundaryMax.x - boundaryMin.x) / 4;
        var maxZoomY = Mathf.Abs(boundaryMax.y - boundaryMin.y) / 4;
        maxZoom = Mathf.Min(maxZoomX, maxZoomY);
    }

    void Update()
    {
        // Handle camera zooming
        HandleZoom();

        // Handle camera panning
        HandlePan();

        HandleClick();

        // Update the last mouse position
        lastMousePosition = Input.mousePosition;
    }

    void HandlePan()
    {
        Vector3 pos = transform.position;

        // Check if the middle mouse button is held down
        if (Input.GetMouseButton(2))
        {
            // Calculate the displacement in world space
            Vector3 offset = mainCamera.ScreenToWorldPoint(lastMousePosition) - mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Apply the displacement to the camera position
            pos += offset;

            // Clamp the position
            pos.x = Mathf.Clamp(pos.x, panLimitMin.x + mainCamera.orthographicSize * mainCamera.aspect, panLimitMax.x - mainCamera.orthographicSize * mainCamera.aspect);
            pos.y = Mathf.Clamp(pos.y, panLimitMax.y + mainCamera.orthographicSize, panLimitMin.y - mainCamera.orthographicSize);

            transform.position = pos;
        }
        else
        {
            var panAmount = Camera.main.orthographicSize * edgePanMultiplier * Time.deltaTime;

            // Move the camera if the mouse is at the edge of the screen
            //if (Input.mousePosition.y >= Screen.height - 1)
            //    pos.y += panAmount;
            //if (Input.mousePosition.y <= 1)
            //    pos.y -= panAmount;
            //if (Input.mousePosition.x >= Screen.width - 1)
            //    pos.x += panAmount;
            //if (Input.mousePosition.x <= 1)
            //    pos.x -= panAmount;

            // Clamp the position
            pos.x = Mathf.Clamp(pos.x, panLimitMin.x + mainCamera.orthographicSize * mainCamera.aspect, panLimitMax.x - mainCamera.orthographicSize * mainCamera.aspect);
            pos.y = Mathf.Clamp(pos.y, panLimitMax.y + mainCamera.orthographicSize, panLimitMin.y - mainCamera.orthographicSize);

            transform.position = pos;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newSize = mainCamera.orthographicSize - scroll * zoomSpeed;

        // Clamp the zoom level
        newSize = Mathf.Clamp(newSize, minZoom, maxZoom);

        // Get the world position under the mouse cursor
        Vector3 mousePosWorldBeforeZoom = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 clampedBefore = scroll < 0 ? new Vector3(Mathf.Clamp(mousePosWorldBeforeZoom.x, boundaryMin.x, boundaryMax.x), Mathf.Clamp(mousePosWorldBeforeZoom.y, boundaryMin.y, boundaryMax.y), mousePosWorldBeforeZoom.z) : mousePosWorldBeforeZoom;

        // Set the new zoom level
        mainCamera.orthographicSize = newSize;

        // Get the world position under the mouse cursor after zooming
        Vector3 mousePosWorldAfterZoom = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 clampedAfter = scroll < 0 ? new Vector3(Mathf.Clamp(mousePosWorldAfterZoom.x, boundaryMin.x, boundaryMax.x), Mathf.Clamp(mousePosWorldAfterZoom.y, boundaryMin.y, boundaryMax.y), mousePosWorldAfterZoom.z) : mousePosWorldAfterZoom;

        // Adjust the camera position to keep the world position under the mouse cursor unchanged
        transform.position += clampedBefore - clampedAfter;

        // Clamp the position after zooming
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, panLimitMin.x + mainCamera.orthographicSize * mainCamera.aspect, panLimitMax.x - mainCamera.orthographicSize * mainCamera.aspect);
        pos.y = Mathf.Clamp(pos.y, panLimitMax.y + mainCamera.orthographicSize, panLimitMin.y - mainCamera.orthographicSize);
        transform.position = pos;
    }

    void HandleClick()
    {
        // Left click
        if (Input.GetMouseButtonUp(0))
        {
            var obj = GetObjectAtCursor();

            // Select unit(s)
            if (obj != null && obj.GetComponent<Unit>() is Unit u)
                u.Select(Input.GetKey(KeyCode.LeftShift));

            // Else tell selected units to move to cursor
            else
            {
                var command = new Command(CommandType.Move, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                activePlayer.SendCommandToSelectedUnits(command);
            }
        }

        // Right click
        else if (Input.GetMouseButtonUp(1))
        {
            // Deselect all units
            foreach (var unit in activePlayer.UnitList)
                if (unit.IsSelected)
                    unit.Deselect();
        }
    }

    public static GameObject GetObjectAtCursor()
    {
        int layerObject = 8;
        Vector2 ray = new(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D hit = Physics2D.Raycast(ray, ray, layerObject);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        return null;
    }
}
