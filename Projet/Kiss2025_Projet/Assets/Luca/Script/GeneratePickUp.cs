using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GeneratePickUp : MonoBehaviour
{
    public GameObject[] pickupPrefabs;
    public int[] numberOfPickups;
    public Transform parentObject; // Objet parent contenant les points de spawn
    private Transform[] spawnPoints; // Points de spawn définis
    public float spawnRadius = 50f;
    public float yOffset = 0.5f; // Offset en Y pour ajuster la hauteur des pickups

    private List<Transform> usedSpawnPoints = new List<Transform>();

    void Start()
    {
        InitializeSpawnPoints();
        GeneratePickups();
    }

    void InitializeSpawnPoints()
    {
        // Remplir le tableau spawnPoints avec chaque enfant de parentObject
        spawnPoints = new Transform[parentObject.childCount];
        for (int i = 0; i < parentObject.childCount; i++)
        {
            spawnPoints[i] = parentObject.GetChild(i);
        }
    }

    void GeneratePickups()
    {
        for (int i = 0; i < pickupPrefabs.Length; i++)
        {
            for (int j = 0; j < numberOfPickups[i]; j++)
            {
                GameObject pickupPrefab = pickupPrefabs[i];
                Transform spawnPoint = GetUniqueSpawnPoint(); // Obtenir un point de spawn unique

                if (spawnPoint != null)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(spawnPoint.position, out hit, spawnRadius, NavMesh.AllAreas))
                    {
                        Vector3 spawnPosition = hit.position;
                        spawnPosition.y += yOffset; // Ajouter l'offset en Y
                        Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);
                    }
                }
            }
        }
    }

    Transform GetUniqueSpawnPoint()
    {
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);
        availableSpawnPoints.RemoveAll(sp => usedSpawnPoints.Contains(sp));

        if (availableSpawnPoints.Count > 0)
        {
            Transform spawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
            usedSpawnPoints.Add(spawnPoint);
            return spawnPoint;
        }
        return null; // Aucun point de spawn disponible
    }
}

