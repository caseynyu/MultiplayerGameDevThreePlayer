using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class BlockSystem : MonoBehaviour
{
    public const float CellSize = 1f;
    [SerializeField] private BlockData blockData1,blockData2,blockData3;
    [SerializeField] private BuildingPreview previewPrefab;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private BlockGrid grid;
    [SerializeField] GameObject redMouseCursor,blueMouseCursor,greenMouseCursor;
    private BuildingPreview redPreview,bluePreview,greenPreview;

    Dictionary<GameObject, BuildingPreview> cursorToPreview = new Dictionary<GameObject, BuildingPreview>();

    private GameObject redSelectedBlockClickObject,greenSelectedBlockClickObject,blueSlectedBlockClickObject;

    private void Update()
    {
        //Vector3 mousePos = MosuePositionToWorldPosition();
        if (redPreview != null)
        {
            HandlePreview(redMouseCursor.transform.position,redPreview);
        }
        if (greenPreview != null)
        {
            HandlePreview(greenMouseCursor.transform.position,greenPreview);
        }
        if (bluePreview != null)
        {
            HandlePreview(blueMouseCursor.transform.position,bluePreview);
        }
        else
        {
            /*if (Keyboard.current.digit1Key.wasPressedThisFrame)
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
            }*/
        }
    }

    public void SwitchBlock(BlockData blockData, String cursorColor, GameObject blockClickObject)
    {
        if (cursorColor == "Red")
        {
            //Debug.Log("Red Click");
            redSelectedBlockClickObject = blockClickObject;
            if (redPreview != null)
            {
                Destroy(redPreview.gameObject);
                redPreview=null;
            }
            redPreview = CreatePreview(blockData,redMouseCursor.transform.position);
        }
        if (cursorColor == "Green")
        {
            //Debug.Log("Green Click");
            greenSelectedBlockClickObject = blockClickObject;
            if (greenPreview != null)
            {
                Destroy(greenPreview.gameObject);
                greenPreview=null;
            }
            greenPreview = CreatePreview(blockData,greenMouseCursor.transform.position);
        }
        if (cursorColor == "Blue")
        {
            //Debug.Log("Blue Click");
            blueSlectedBlockClickObject = blockClickObject;
            if (bluePreview != null)
            {
                Destroy(bluePreview.gameObject);
                bluePreview=null;
            }
            bluePreview = CreatePreview(blockData,blueMouseCursor.transform.position);
        }
        
        
    }

    private Vector3 MosuePositionToWorldPosition()
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }

    private void HandlePreview(Vector3 mosuePosition, BuildingPreview preview)
    {
        preview.transform.position = mosuePosition;
        List<Vector3> buildPositions = preview.BlockSprite.GetAllBuildingPositions();
        bool canBuild = grid.CanBuild(buildPositions);
        if (canBuild)
        {
            preview.transform.position = GetSnappedCenterPosition(buildPositions);
            preview.ChangeState(BuildingPreview.BuildingPreviewState.Positive);
            if (Gamepad.all[0].buttonSouth.wasPressedThisFrame && preview == redPreview)
            {
                Debug.Log("gamepad0pressed");
                PlaceBlock(buildPositions,preview);
                Destroy(redSelectedBlockClickObject);
            }
            if (Gamepad.all[0].buttonWest.wasPressedThisFrame && preview == redPreview)
            {
                preview.Rotate(90);
            }
            if (Gamepad.all[1].buttonSouth.wasPressedThisFrame && preview == greenPreview)
            {
                Debug.Log("gamepad1pressed");
                PlaceBlock(buildPositions,preview);
                Destroy(greenSelectedBlockClickObject);
            }
            if (Gamepad.all[1].buttonWest.wasPressedThisFrame && preview == greenPreview)
            {
                preview.Rotate(90);
            }
            if (Gamepad.all[2].buttonSouth.wasPressedThisFrame && preview == bluePreview)
            {
                Debug.Log("gamepad2pressed");
                PlaceBlock(buildPositions,preview);
                Destroy(blueSlectedBlockClickObject);
            }
            if (Gamepad.all[2].buttonWest.wasPressedThisFrame && preview == bluePreview)
            {
                preview.Rotate(90);
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

    private void PlaceBlock(List<Vector3> blockPositions, BuildingPreview preview)
    {
        Block block = Instantiate(blockPrefab,preview.transform.position,Quaternion.identity);
        block.Setup(preview.Data,preview.BlockSprite.Rotation);
        //Debug.Log(preview.transform.position);
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
