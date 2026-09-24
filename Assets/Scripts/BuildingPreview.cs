using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public enum BuildingPreviewState
    {
        Positive,
        Negative
    }
    [SerializeField] private Material positiveMaterial,negativeMaterial;
    public BuildingPreviewState State {get; private set; } = BuildingPreviewState.Negative;
    public BlockData Data {get;private set;}
    public BlockSprite BlockSprite  {get;private set;}
    private List<Renderer> renderers = new();
    private List<Collider> colliders = new();

    public void Setup(BlockData data)
    {
        Data = data;
        BlockSprite = Instantiate(data.Sprite,transform.position,Quaternion.identity,transform);
        renderers.AddRange(BlockSprite.GetComponentsInChildren<Renderer>());
        colliders.AddRange(BlockSprite.GetComponentsInChildren<Collider>());
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        SetPreviewMaterial(State);
    }

    public void ChangeState(BuildingPreviewState newState)
    {
        if (newState == State) return;
        State = newState;
        SetPreviewMaterial(State);
    }
    public void Rotate(int rotationStep)
    {
        BlockSprite.Rotate(rotationStep);
    }

    private void SetPreviewMaterial(BuildingPreviewState newState)
    {
        Material previewMat = newState == BuildingPreviewState.Positive ? positiveMaterial : negativeMaterial;
        foreach (var rend in renderers)
        {
            Material[] mats = new Material[rend.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = previewMat;
            }
            rend.materials = mats;
        }
    }
}
