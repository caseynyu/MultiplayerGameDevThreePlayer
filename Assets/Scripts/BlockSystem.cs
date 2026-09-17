using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class BlockSystem : MonoBehaviour
{
    public const float CellSize = 1f;
    [SerializeField] private BlockData blockData1,blockData2,blockData3;
    [SerializeField] private BuildingPreview previewPrefab;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private BlockGrid grid;
    private BuildingPreview preview;

    private void Update()
    {
        Vector3 mousePos = MosuePositionToWorldPosition();
        if (preview != null)
        {
            HandlePreview(mousePos);
        }
        else
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                preview = CreatePreview(blockData1,mousePos);
            }
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                preview = CreatePreview(blockData2,mousePos);
            }
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                preview = CreatePreview(blockData3,mousePos);
            }
        }
    }

    private Vector3 MosuePositionToWorldPosition()
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }

    private void HandlePreview(Vector3 mosuePosition)
    {
        preview.transform.position = mosuePosition;
        List<Vector3> buildPositions = preview.BlockSprite.GetAllBuildingPositions();
        bool canBuild = grid.CanBuild(buildPositions);
        if (canBuild)
        {
            preview.transform.position = GetSnappedCenterPosition(buildPositions);
            preview.ChangeState(BuildingPreview.BuildingPreviewState.Positive);
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                PlaceBlock(buildPositions);
            }
        }
        else
        {
            preview.ChangeState(BuildingPreview.BuildingPreviewState.Negative);
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            preview.Rotate(90);
        }
    }

    private void PlaceBlock(List<Vector3> blockPositions)
    {
        Block block = Instantiate(blockPrefab,preview.transform.position,Quaternion.identity);
        block.Setup(preview.Data,preview.BlockSprite.Rotation);
        Debug.Log(preview.transform.position);
        grid.SetBuilding(block,blockPositions);
        Destroy(preview.gameObject);
        preview=null;
    }

    private Vector3 GetSnappedCenterPosition(List<Vector3> allBuildingPositions)
    {
        List<int> xs = allBuildingPositions.Select(p => Mathf.FloorToInt(p.x)).ToList();
        List<int> ys = allBuildingPositions.Select(p => Mathf.FloorToInt(p.y)).ToList();
        float centerX = (xs.Min()+xs.Max())/2f+CellSize/2f;
        float centerY = (ys.Min()+ys.Max())/2f+CellSize/2f;
        return new(centerX,centerY,0);
    }

    private BuildingPreview CreatePreview(BlockData data, Vector3 position)
    {
        BuildingPreview buildingPreview = Instantiate(previewPrefab,position,Quaternion.identity);
        buildingPreview.Setup(data);
        return buildingPreview;
    }

    public void ChangePrefab()
    {
        
    }
}
