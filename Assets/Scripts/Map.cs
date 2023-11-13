using System;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPoints;

    public Vector3 GetSpawnPoint(int spawnPointIndex)
    {
        if(spawnPointIndex < 0) throw new IndexOutOfRangeException();

        var transforms = spawnPoints.GetComponentsInChildren<Transform>();

        if (spawnPointIndex >= transforms.Length - 1) throw new NotImplementedException("Can't yet generate a random spawn location.");

        // Offset because transforms[0] is *this* transform, not the first child.
        return transforms[spawnPointIndex + 1].position;
    }
}
