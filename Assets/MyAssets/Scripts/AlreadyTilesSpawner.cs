using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlreadyTilesSpawner : MonoBehaviour
{
    // Define a class to hold a group of spawn points
    [System.Serializable]
    public class SpawnPointGroup
    {
        public List<Transform> spawnPoints = new List<Transform>();
    }

    // List of spawn point groups
    public List<SpawnPointGroup> spawnPointGroups = new List<SpawnPointGroup>();
    public GameObject tileHolderPrefab;

    private void Start()
    {
        SpawnPointGroup spawnPointGroup = spawnPointGroups[Random.Range(0, spawnPointGroups.Count)];
        StartCoroutine(CorotineSpawnTiles(spawnPointGroup));
    }

    IEnumerator CorotineSpawnTiles(SpawnPointGroup spawnPointGroup)
    {
        foreach (Transform t in spawnPointGroup.spawnPoints)
        {
            Vector3 pos = t.position;
            pos.y += .25f;

            GameObject tileHolder = Instantiate(tileHolderPrefab, pos, Quaternion.identity);
        
            yield return new WaitForSeconds(.1f);

            Destroy(tileHolder.GetComponent<TileController>());
            tileHolder.GetComponent<TilesHolder>().enabled = true;
            tileHolder.GetComponent<TilesHolder>().groundTile = t;
            tileHolder.gameObject.layer = 15;

            t.tag = "Untagged";
        }
    }
}