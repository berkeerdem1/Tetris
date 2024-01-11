using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_Objects : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject tutorial;
    public int numberOfObjects = 7;

    void Start()
    {
        InvokeRepeating("SpawnObject", 0f, 2f);
        InvokeRepeating("SpawnObject2", 0.4f, 2.3f);
    }

    void SpawnObject()
    {
        // Rastgele bir prefab sec
        GameObject selectedPrefab = prefabs[Random.Range(0, prefabs.Length)];
        // Rastgele konum belirle
        Vector3 spawnPosition = new Vector3(Random.Range(-19f, -7f), 15f, 0f);

        GameObject newObj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        Destroy(newObj, 6f);
        Destroy(tutorial, 15f);

    }
    void SpawnObject2()
    {
        GameObject selectedPrefab = prefabs[Random.Range(0, prefabs.Length)];
        Vector3 spawnPosition = new Vector3(Random.Range(7f, 19f), 15f, 0f);

        GameObject newObj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        Destroy(newObj, 6f);
    }
}
