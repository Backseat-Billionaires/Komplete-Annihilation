using System;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPoints;

    private List<int> usedSpawnPoints = new List<int>();

    public Vector3 GetSpawnPoint(int spawnPointIndex)
    {
        var transforms = spawnPoints.GetComponentsInChildren<Transform>();
        if (spawnPointIndex < 0 || spawnPointIndex >= transforms.Length - 1)
        {
            throw new IndexOutOfRangeException("Invalid spawn point index.");
        }

        // Offset because transforms[0] is *this* transform, not the first child.
        return transforms[spawnPointIndex + 1].position;
    }

    public Vector3 GetUniqueRandomSpawnPoint()
    {
        var transforms = spawnPoints.GetComponentsInChildren<Transform>();
        if (transforms.Length > 1)
        {
            int randomIndex;
            do
            {
                randomIndex = UnityEngine.Random.Range(1, transforms.Length);
            } 
            while (usedSpawnPoints.Contains(randomIndex));

            usedSpawnPoints.Add(randomIndex); // Mark this spawn point as used
            return transforms[randomIndex].position;
        }
        throw new NotImplementedException("No spawn locations defined or not enough spawn points for all players.");
    }

    // Call this method at the end of a game or match to reset used spawn points
    public void ResetUsedSpawnPoints()
    {
        usedSpawnPoints.Clear();
    }
}