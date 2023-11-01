using UnityEngine;

public class GameController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var obj = GetObjectAtPosition(worldPos);
            if (obj != null && obj.GetComponent<Unit>() is Unit u)
                u.Select(Input.GetKey(KeyCode.LeftShift));
            else
                foreach (var unit in Unit.unitList)
                {
                    if (unit.IsSelected)
                        unit.Move(worldPos);
                }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            foreach (var unit in Unit.unitList)
                if (unit.IsSelected)
                    unit.Deselect();
        }
    }

    public static GameObject GetObjectAtPosition(Vector2 position)
    {
        int layerObject = 8;
        var ray = new Vector2(position.x, position.y);
        var hit = Physics2D.Raycast(ray, ray, layerObject);
        return hit.collider?.gameObject;
    }
}
