using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockSystem : MonoBehaviour
{
    public const float CellSize = 1f;
    [SerializeField] private BuildingPreview previewPrefab;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private BlockGrid grid;
    [SerializeField] private GameObject redMouseCursor, blueMouseCursor, greenMouseCursor;
    private readonly BuildingPreview[] previews = new BuildingPreview[3];
    private readonly int[] pickupFrames = new int[3];

    public bool HasPendingBlocks => previews.Any(preview => preview != null);
    private GameObject CursorFor(int index) => index == 0 ? redMouseCursor : index == 1 ? greenMouseCursor : blueMouseCursor;

    public bool TryPickUp(BlockData data, int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= previews.Length || data == null || previews[playerIndex] != null)
            return false;

        var preview = Instantiate(previewPrefab, MouseClickCursorGamepad.WorldPosition(CursorFor(playerIndex)), Quaternion.identity);
        preview.Setup(data);
        previews[playerIndex] = preview;
        pickupFrames[playerIndex] = Time.frameCount;
        return true;
    }

    private void Update()
    {
        for (int index = 0; index < previews.Length; index++)
        {
            if (previews[index] != null) HandlePreview(index);
        }
    }

    private void HandlePreview(int playerIndex)
    {
        var preview = previews[playerIndex];
        var cursor = CursorFor(playerIndex);
        preview.transform.position = MouseClickCursorGamepad.WorldPosition(cursor);
        var gamepad = GameManager.GetGamepad(playerIndex);
        if (gamepad != null && gamepad.buttonWest.wasPressedThisFrame) preview.Rotate(90);

        List<Vector3> positions = preview.BlockSprite.GetAllBuildingPositions();
        bool canBuild = grid.CanBuild(positions);
        preview.ChangeState(canBuild ? BuildingPreview.BuildingPreviewState.Positive : BuildingPreview.BuildingPreviewState.Negative);
        if (!canBuild) return;
        preview.transform.position = GetSnappedCenterPosition(positions);

        // The pickup press must not also place the block, or consume a second menu item.
        if (gamepad == null || !gamepad.buttonSouth.wasPressedThisFrame || pickupFrames[playerIndex] == Time.frameCount ||
            MouseClickCursorGamepad.GetClickTarget(cursor, playerIndex, out _) != null)
            return;

        var block = Instantiate(blockPrefab, preview.transform.position, Quaternion.identity);
        block.Setup(preview.Data, preview.BlockSprite.Rotation);
        grid.SetBuilding(block, positions);
        Destroy(preview.gameObject);
        previews[playerIndex] = null;
    }

    private Vector3 GetSnappedCenterPosition(List<Vector3> positions)
    {
        var xs = positions.Select(p => Mathf.FloorToInt(p.x)).ToList();
        var ys = positions.Select(p => Mathf.FloorToInt(p.y)).ToList();
        return new Vector3((xs.Min() + xs.Max()) / 2f + CellSize / 2f,
            (ys.Min() + ys.Max()) / 2f + CellSize / 2f, 0);
    }
}
