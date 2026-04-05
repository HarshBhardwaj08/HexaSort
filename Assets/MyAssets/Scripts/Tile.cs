using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int colorId;

    public void GotoTarget(TilesHolder tilesHolder)
    {
        // Move To Other Tile Stack
        transform.DOLocalJump(tilesHolder.spawnPoint.localPosition, 3, 1, .2f);
        transform.DOLocalRotate(new Vector3(180, 0, 0), .5f);

        tilesHolder.IncreaseYSpawnPointYPos();
        tilesHolder.tiles.Add(this);
        AudioManager.Instance.Play("Click");
    }

    public void PlayDestroyAnim()
    {
        AudioManager.Instance.Play("TileDestroy");

        transform.DOScale(Vector3.zero, .5f)
        .OnComplete(() => Destroy(gameObject));
    }
}