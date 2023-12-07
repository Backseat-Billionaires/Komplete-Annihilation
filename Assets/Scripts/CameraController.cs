using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour
{
    public float edgePanMultiplier = 1f;
    public float zoomSpeed = 30f;
    public float minZoom = 15f;
    private float maxZoom;

    public Vector2 panLimitMin = new Vector2(0f, 0f);
    public Vector2 panLimitMax = new Vector2(192f, -192f);
    private Vector3 lastMousePosition;
    private Vector2 boundaryMin;
    private Vector2 boundaryMax;

    public Transform characterToFollow;
    public Player activePlayer;
    private Camera mainCamera;
    private UnitSelectionManager selectionManager;
    private CommandExecutor commandExecutor;

    void Start()
    {
        mainCamera = Camera.main;
        selectionManager = FindObjectOfType<UnitSelectionManager>();
        commandExecutor = FindObjectOfType<CommandExecutor>();

        boundaryMin = new Vector2(Mathf.Min(panLimitMin.x, panLimitMax.x), Mathf.Min(panLimitMin.y, panLimitMax.y));
        boundaryMax = new Vector2(Mathf.Max(panLimitMin.x, panLimitMax.x), Mathf.Max(panLimitMin.y, panLimitMax.y));

        var maxZoomX = Mathf.Abs(boundaryMax.x - boundaryMin.x) / 6;
        var maxZoomY = Mathf.Abs(boundaryMax.y - boundaryMin.y) / 6;
        maxZoom = Mathf.Min(maxZoomX, maxZoomY);

        if (!isOwned)
        {
            mainCamera.enabled = false;
        }
    }

    void Update()
    {
        if (!isOwned) return;

        HandleZoom();
        HandlePan();
        HandleClick();

        lastMousePosition = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.C) && characterToFollow != null)
        {
            CenterCameraOnCharacter();
        }
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
            if (Input.mousePosition.y >= Screen.height - 1)
                pos.y += panAmount;
            if (Input.mousePosition.y <= 1)
                pos.y -= panAmount;
            if (Input.mousePosition.x >= Screen.width - 1)
                pos.x += panAmount;
            if (Input.mousePosition.x <= 1)
                pos.x -= panAmount;

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

            // Check for unit selection
            if (obj != null)
            {
                if (obj.TryGetComponent<Unit>(out Unit unit))
                {
                    selectionManager.SelectObject(obj, Input.GetKey(KeyCode.LeftShift));
                }
                else if (obj.TryGetComponent<MetalDeposit>(out MetalDeposit deposit))
                {
                    selectionManager.SelectObject(obj);
                }
                else
                {
                    // Issue move command
                    var command = new Command(CommandType.Move, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    commandExecutor.ExecuteCommand(command, activePlayer);
                }
            }
        }

        // Right click
        else if (Input.GetMouseButtonUp(1))
        {
            selectionManager.DeselectAll();
        }
    }
    
    private static GameObject GetObjectAtCursor()
    {
        int layerObject = 8;
        Vector2 ray = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero, Mathf.Infinity, layerObject);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private void CenterCameraOnCharacter()
    {
        if (characterToFollow != null)
        {
            Vector3 characterPosition = characterToFollow.position;
            characterPosition.z = transform.position.z; // Keep the camera's current z-position
            transform.position = characterPosition;
        }
    }
}
