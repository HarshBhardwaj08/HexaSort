using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using RDG;

public class TilesHolder : MonoBehaviour
{
    public int maxTiles;
    [HideInInspector] public List<Tile> tiles;
    public List<GameObject> tilesPrefabs;
    public Transform spawnPoint;
    private float yIncVal;
    private int totalTilesCount;
    private int firstColorTilesCount;
    private int secondColorTilesCount;

    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    public LayerMask layerMask;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public Transform groundTile;
    [HideInInspector] public bool isTilesMatched;

    private void Start()
    {
        totalTilesCount = Random.Range(2, maxTiles+1);
        firstColorTilesCount = Random.Range(0, totalTilesCount);
        SpawnTiles(firstColorTilesCount);
        secondColorTilesCount = totalTilesCount - firstColorTilesCount;
        SpawnTiles(secondColorTilesCount);
        this.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        colliders = Physics.OverlapBox(transform.position, boxSize / 2f, Quaternion.identity, layerMask);
        colliders = FilterOutSelfColliders(colliders);
    }

    Collider[] FilterOutSelfColliders(Collider[] colliders)
    {
        // Filter out all colliders attached to the current GameObject
        return colliders.Where(c => c.transform != transform).ToArray();
    }

    private void SpawnTiles(int tilesToSpawn)
    {
        if (tilesToSpawn == 0)
            return;

        GameObject tilePrefab = tilesPrefabs[Random.Range(0, tilesPrefabs.Count)];

        for (int i = 0; i < tilesToSpawn; i++)
        {
            GameObject tile = Instantiate(tilePrefab, spawnPoint.position, Quaternion.identity, transform);
            tiles.Add(tile.GetComponent<Tile>());

            IncreaseYSpawnPointYPos();
        }
    }

    public void IncreaseYSpawnPointYPos()
    {
        yIncVal += .25f;
        spawnPoint.localPosition = new Vector3(spawnPoint.localPosition.x, yIncVal, spawnPoint.localPosition.z);
    }

    public void DecreaseYSpawnPointYPos()
    {
        yIncVal -= .25f;
        spawnPoint.localPosition = new Vector3(spawnPoint.localPosition.x, yIncVal, spawnPoint.localPosition.z);
    }

    public int GetListSize()
    {
        return tiles.Count;
    }

    public void ThrowTiles(TilesHolder matchedTileHolder)
    {
        StartCoroutine(CoroutineThrowTiles(matchedTileHolder));
    }

    IEnumerator CoroutineThrowTiles(TilesHolder matchedTileHolder)
    {
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            if (tiles[i].colorId == matchedTileHolder.tiles[matchedTileHolder.tiles.Count - 1].colorId)
            {
                // Set other tileholder ColerId to My Color Id
                tiles[i].GotoTarget(matchedTileHolder);
                tiles[i].transform.parent = matchedTileHolder.transform;

                tiles.RemoveAt(i);

                if (tiles.Count == 0)
                {
                    groundTile.tag = "GroundTile";

                    GameManager gameManager = FindObjectOfType<GameManager>();                
                    groundTile.GetComponent<MeshRenderer>().material = gameManager.groundMat;

                    gameObject.SetActive(false);
                    Destroy(gameObject, 1);
                    enabled = false;
                    yield return null;
                }

                DecreaseYSpawnPointYPos();
                yield return new WaitForSeconds(.05f);
            }
            else
            {
                break;
            }
        }
    }

    private int matchedCount;
    // Function to check if all tiles have the same index
    public void CheckLimitReached()
    {
        if (tiles.Count < 10)
        {
            Debug.Log("Not Much Tiles");
            return;
        }

        int firstIndex = tiles[tiles.Count-1].colorId;
        bool sameIndex = true;
        matchedCount = 0;

        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            if (tiles[i].colorId != firstIndex)
            {
                sameIndex = false;
                break;
            }

            matchedCount++;
        }

        if (matchedCount >= 10)
        {
            FindObjectOfType<GameManager>().incrementAmount = matchedCount;

            isTilesMatched = true;
            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                matchedCount--;
                tiles[i].PlayDestroyAnim();
                tiles.RemoveAt(i);
                DecreaseYSpawnPointYPos();  

                if (matchedCount == 0)
                    break;
            }

            Vibration.Vibrate(50);

            FindObjectOfType<GameManager>().IncreaseLevelSlider();

            Debug.Log("SameColor");

            if (sameIndex)
            {
                Destroy(gameObject, 1);
                groundTile.tag = "GroundTile";

                GameManager gameManager = FindObjectOfType<GameManager>();
                groundTile.GetComponent<MeshRenderer>().material = gameManager.groundMat;
            }
        }
        else
        {
            Debug.Log("NotSameColor");
        }      
    }
}