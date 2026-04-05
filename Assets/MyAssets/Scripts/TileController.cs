using UnityEngine;

public class TileController : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    public Transform targetPos;
    private Vector3 initialPos;
    private bool isMouseDown;
    private Material selectedGroundMat, groundMat;

    private void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        selectedGroundMat = gameManager.selectedGroundMat; 
        groundMat = gameManager.groundMat;

        initialPos = transform.position;
    }

    void OnMouseDown()
    {
        isMouseDown = true;
        mZCoord = Camera.main.WorldToScreenPoint(
        gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        if (!isMouseDown)
            return;

        transform.position = new Vector3(GetMouseAsWorldPoint().x + mOffset.x, 3, GetMouseAsWorldPoint().z + mOffset.z);
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
        if (targetPos == null)
        {
            transform.position = initialPos;
            return;
        }

        foreach (TileController tileController in FindObjectsOfType<TileController>())
        {
            if(tileController != this)
            {
                Destroy(tileController);
            }
        }

        AudioManager.Instance.Play("TilePlace");

        Vector3 pos = targetPos.position;
        pos.y += .25f;
        transform.position = pos;
        targetPos.tag = "Untagged";
        GetComponent<TilesHolder>().enabled = true;
        GetComponent<TilesHolder>().groundTile = targetPos;
        gameObject.layer = 15;

        Invoke(nameof(InvokeCallTilesHolder), .1f);

        FindObjectOfType<TilesHolderSpawner>().DecreaseTileHolderCount();
    }

    private void InvokeCallTilesHolder()
    {
        FindObjectOfType<TilesHolderChecker>().CheckOneTilesHolder(GetComponent<TilesHolder>());
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GroundTile") && isMouseDown)
        {
            targetPos = other.transform;
            other.GetComponent<MeshRenderer>().material = selectedGroundMat;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GroundTile"))
        {
            targetPos = null;
            other.GetComponent<MeshRenderer>().material = groundMat;
        }
    }
}
