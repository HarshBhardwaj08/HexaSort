using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesHolderChecker : MonoBehaviour
{
    private int matchedTilesCount = 0;
    private TilesHolder[] tilesHolders;
    private bool runCheckAllTilesHol;
    public float speed;
    private bool tilesMatched;

    public void CheckAllTilesHolder()
    {
        FindFillTilesHolderArray();
        StartCoroutine(CoroutineCheckAllTilesHolder());
    }

    private void FindFillTilesHolderArray()
    {
        List<TilesHolder> tempTilesHolders = new List<TilesHolder>();

        foreach (var tileHol in FindObjectsOfType<TilesHolder>())
        {
            if (tileHol.enabled)
                tempTilesHolders.Add(tileHol);
        }

        tilesHolders = tempTilesHolders.ToArray();

        // Sort the array based on list sizes
        Array.Sort(tilesHolders, new TilesHolderComparer());
    }

    IEnumerator CoroutineCheckAllTilesHolder()
    {
        foreach (TilesHolder tilesHol in tilesHolders)
        {
            if (!tilesHol)
                yield return null;

            if (tilesHol.tiles.Count == 0)
                yield return null;

            foreach (Collider col in tilesHol.colliders)
            {
                if (tilesHol.tiles.Count > 0 && col.GetComponent<TilesHolder>().tiles.Count > 0)
                {
                    if (tilesHol.tiles[tilesHol.tiles.Count - 1].colorId == col.GetComponent<TilesHolder>().tiles[col.GetComponent<TilesHolder>().tiles.Count - 1].colorId)
                    {
                        matchedTilesCount++;
                        col.GetComponent<TilesHolder>().ThrowTiles(tilesHol);
                        yield return new WaitForSeconds(speed);

                    }
                }
            }
        }

        Debug.Log("Finished processing all objects");

        if (matchedTilesCount > 0)
        {
            matchedTilesCount = 0;
            runCheckAllTilesHol = true;
        }
        else
        {
            foreach (TilesHolder tilesHolder in FindObjectsOfType<TilesHolder>())
            {
                tilesHolder.CheckLimitReached();

                if (tilesHolder.isTilesMatched)
                {
                    tilesHolder.isTilesMatched = false;
                    tilesMatched = true;
                }
            }

            if (tilesMatched)
            {
                yield return new WaitForSeconds(speed);

                tilesMatched = false;
                runCheckAllTilesHol = true;
            }
            else
            {
                foreach (TilesHolder tilesHolder in FindObjectsOfType<TilesHolder>())
                {
                    if (!tilesHolder.enabled)
                        tilesHolder.gameObject.AddComponent<TileController>();
                }
            }
        }

        tilesHolders = null;
        CheckEmptyGround();
        yield return null;
    }

    private void CheckEmptyGround()
    {
        if (!GameObject.FindGameObjectWithTag("GroundTile"))
        {
            FindObjectOfType<GameManager>().Retry();
        }
    }

    public void CheckOneTilesHolder(TilesHolder _tilesHolder)
    {
        StartCoroutine(CoroutineCheckOneTilesHolder(_tilesHolder));
    }

    IEnumerator CoroutineCheckOneTilesHolder(TilesHolder _tilesHolder)
    {     
        foreach (Collider col in _tilesHolder.colliders)
        {
            if (col.GetComponent<TilesHolder>().tiles.Count == 0)
                yield return null;

                if (_tilesHolder.tiles[_tilesHolder.tiles.Count - 1].colorId == col.GetComponent<TilesHolder>().tiles[col.GetComponent<TilesHolder>().tiles.Count - 1].colorId)
                {
                    matchedTilesCount++;
                    col.GetComponent<TilesHolder>().ThrowTiles(_tilesHolder);
                    yield return new WaitForSeconds(speed);
        
                }
        }
        
        Debug.Log("Finished processing all objects");

        if (matchedTilesCount > 0)
        {
            matchedTilesCount = 0;
            runCheckAllTilesHol = true;
        }
        else
        {
            foreach (TilesHolder tilesHolder in FindObjectsOfType<TilesHolder>())
            {
                if (!tilesHolder.enabled)
                    tilesHolder.gameObject.AddComponent<TileController>();
            }

            CheckEmptyGround();
        }

        yield return null;
    }


    private void Update()
    {
        if (runCheckAllTilesHol)
        {
            runCheckAllTilesHol = false;
            CheckAllTilesHolder();
        }
    }
}

public class TilesHolderComparer : IComparer<TilesHolder>
{
    public int Compare(TilesHolder x, TilesHolder y)
    {
        // Compare based on list size
        return x.GetListSize().CompareTo(y.GetListSize());
    }
}