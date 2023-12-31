using System;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPointsContainer;

    private List<Transform> spawnPoints = new List<Transform>();
    private List<int> usedSpawnPoints = new List<int>();

    void Awake()
    {
        if (spawnPointsContainer == null)
        {
            Debug.LogError("Spawn Points Container is not assigned in the Inspector.");
            return;
        }

        foreach (Transform child in spawnPointsContainer.transform)
        {
            if (child != transform)
            {
                spawnPoints.Add(child);
            }
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points found in the Spawn Points Container.");
        }
    }
    

    public Vector3 GetUniqueRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            throw new InvalidOperationException("No spawn points available.");
        }

        if (usedSpawnPoints.Count >= spawnPoints.Count)
        {
            ResetUsedSpawnPoints();
        }

        int randomIndex;
        do
        {
            randomIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
        } 
        while (usedSpawnPoints.Contains(randomIndex));

        usedSpawnPoints.Add(randomIndex);
        return spawnPoints[randomIndex].position;
    }

    public void ResetUsedSpawnPoints()
    {
        usedSpawnPoints.Clear();
    }
}