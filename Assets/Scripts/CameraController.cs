using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, -10);
    public float minZoom = 20f;
    public float maxZoom = 50f;
    public float zoomLerpSpeed = 10f;

    private Camera cam;
    private float targetZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera component not found on the object");
        }

        targetZoom = cam.orthographicSize;
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;

        AdjustZoom();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetZoomLevel(float weaponRange, float maxWeaponRange)
    {
        // Map weapon range to zoom level, clamping within min and max zoom
        targetZoom = Mathf.Clamp(Mathf.Lerp(minZoom, maxZoom, weaponRange / maxWeaponRange), minZoom, maxZoom);
    }

    private void AdjustZoom()
    {
        if (Mathf.Abs(cam.orthographicSize - targetZoom) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomLerpSpeed * Time.deltaTime);
        }
    }
}