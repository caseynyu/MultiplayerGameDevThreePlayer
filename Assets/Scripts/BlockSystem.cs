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
            HandlePreview(redMouseCursor,redPreview);
        }
        if (greenPreview != null)
        {
            HandlePreview(greenMouseCursor,greenPreview);
        }
        if (bluePreview != null)
        {
            HandlePreview(blueMouseCursor,bluePreview);
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
            redPreview = CreatePreview(blockData,MouseClickCursorGamepad.WorldPosition(redMouseCursor));
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
            greenPreview = CreatePreview(blockData,MouseClickCursorGamepad.WorldPosition(greenMouseCursor));
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
            bluePreview = CreatePreview(blockData,MouseClickCursorGamepad.WorldPosition(blueMouseCursor));
        }
        
        
    }

    private Vector3 MosuePositionToWorldPosition()
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }

    private void HandlePreview(GameObject cursor, BuildingPreview preview)
    {
        preview.transform.position = MouseClickCursorGamepad.WorldPosition(cursor);
        // A press over the palette or Play button belongs to the UI, not the map.
        bool overUI = MouseClickCursorGamepad.GetClickTarget(cursor, out _) != null;
        List<Vector3> buildPositions = preview.BlockSprite.GetAllBuildingPositions();
        bool canBuild = grid.CanBuild(buildPositions);
        if (canBuild)
        {
            preview.transform.position = GetSnappedCenterPosition(buildPositions);
            preview.ChangeState(BuildingPreview.BuildingPreviewState.Positive);
            int gamepadIndex = preview == redPreview ? 0 : preview == greenPreview ? 1 : 2;
            Gamepad gamepad = GameManager.GetGamepad(gamepadIndex);
            if (!overUI && gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)
            {
                GameObject selectedBlock = preview == redPreview ? redSelectedBlockClickObject
                    : preview == greenPreview ? greenSelectedBlockClickObject : blueSlectedBlockClickObject;
                PlaceBlock(buildPositions, preview);
                Destroy(selectedBlock);
                return;
            }
            if (gamepad != null && gamepad.buttonWest.wasPressedThisFrame)
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

    public void StopBuilding()
    {
        if (redPreview != null) Destroy(redPreview.gameObject);
        if (greenPreview != null) Destroy(greenPreview.gameObject);
        if (bluePreview != null) Destroy(bluePreview.gameObject);
        redPreview = greenPreview = bluePreview = null;
        redMouseCursor.SetActive(false);
        greenMouseCursor.SetActive(false);
        blueMouseCursor.SetActive(false);
        enabled = false;
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
