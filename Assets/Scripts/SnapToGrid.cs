using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[ExecuteAlways]
public class SnapToGrid : MonoBehaviour
{
    private int tileSize = 1;
    private Vector3 tileOffset = new Vector3(0,.25f,0);

    void Update()
    {
        Vector3 currentPosition = transform.position;

        float snappedX = Mathf.Round(currentPosition.x / tileSize) * tileSize + tileOffset.x;
        float snappedY = Mathf.Round(currentPosition.y / tileSize) * tileSize + tileOffset.y;
        float snappedZ = tileOffset.z;

        Vector3 snappedPosition = new Vector3 (snappedX,snappedY,snappedZ);
        transform.position = snappedPosition;
    }

}
