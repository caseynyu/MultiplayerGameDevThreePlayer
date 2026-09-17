using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockSprite : MonoBehaviour
{
    [SerializeField] private Transform wrapper;

    public float Rotation => wrapper.transform.eulerAngles.z;
    private GridShapeUnit[] shapeUnits;

    private void Awake()
    {
        shapeUnits = GetComponentsInChildren<GridShapeUnit>();

    }

    public void Rotate(float rotationStep)
    {
        wrapper.Rotate(new(0,0,rotationStep));
    }
    public List<Vector3> GetAllBuildingPositions()
    {
        return shapeUnits.Select(unit => unit.transform.position).ToList(); 
    }
}
