using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesHolderSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public GameObject tilesHolderPrefab;
    private int spawTilesHolCount;

    private void Start()
    {
        SpawnTileHolders();

        Invoke(nameof(AddTileControllers), .3f);
    }

    private void AddTileControllers()
    {
        foreach (TilesHolder tilesHolder in FindObjectsOfType<TilesHolder>())
        {
            if (!tilesHolder.enabled)
                tilesHolder.gameObject.AddComponent<TileController>();
        }
    }

    public void DecreaseTileHolderCount()
    {
        spawTilesHolCount--;

        if(spawTilesHolCount == 0)
        {
            SpawnTileHolders();
        }
    }

    private void SpawnTileHolders()
    {
        foreach (Transform spaPoint in spawnPoints)
        {
            GameObject obj = Instantiate(tilesHolderPrefab, spaPoint.position, Quaternion.identity);
            obj.name = "Name " + Time.timeScale + Random.Range(0, 100);
            spawTilesHolCount++;
        }

        GetComponent<AudioSource>().Play();
    }
}