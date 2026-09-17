using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockGrid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    private BlockGridCell[,]grid;
    [SerializeField] BlockData startPrefab,endPrefab;
    [SerializeField] Block blockPrefab;
    private void Start()

    {
        //ResetGrid();
        /*grid = new BlockGridCell[width,height];
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x,y] = new();
            }
        }*/
    }

    public void ResetGrid()
    {
        if (grid !=null && grid.Length > 0)
        {
            List<BlockSprite> objectsToDelete = GameObject.FindObjectsByType<BlockSprite>().ToList();
            foreach (var objectToDelete in objectsToDelete)
            {
                Destroy(objectToDelete.gameObject);
            }
        }
        
        grid = new BlockGridCell[width,height];
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x,y] = new();
            }
        }
        //Vector2 startPos = transform.position;
        int randomX = UnityEngine.Random.Range(2,width-2);
        int randomY = UnityEngine.Random.Range(0,height-1);
        Vector2 startPos = new Vector2(.5f + transform.position.x + randomX*BlockSystem.CellSize,.5f+transform.position.y+randomY*BlockSystem.CellSize);
        Debug.Log(startPos);
        List<Vector3> blockPositions = new List<Vector3>();
        blockPositions.Add(startPos);blockPositions.Add(new Vector2(startPos.x-1,startPos.y));blockPositions.Add(new Vector2(startPos.x+1,startPos.y));

        Block block = Instantiate(blockPrefab,startPos,Quaternion.identity);
        block.Setup(startPrefab,0);
        //List<Vector3> spawnBlockPositions=spawnBlocks.GetComponentsInChildren<GridShapeUnit>().ToList();
        //block.Setup(startPrefab,Quaternion.identity);
        SetBuilding(block,blockPositions);
    }

    public void SetBuilding(Block block, List<Vector3> allBuildingPositions)
    {
        foreach (var position in allBuildingPositions)
        {
            (int x,int y) = WorldToGridPosition(position);
            grid[x,y].SetBlock(block);
        }
    }

    public bool CanBuild(List<Vector3> allBuildingPositions)
    {
        foreach (var position in allBuildingPositions)
        {
            (int x,int y) = WorldToGridPosition(position);
            if (x<0 || x>=width || y< 0 || y>=height) return false;
            if(!grid[x,y].isEmpty()) return false;
        }
        return true;
    }
    private (int x, int y) WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - transform.position).x / BlockSystem.CellSize);
        int y = Mathf.FloorToInt((worldPosition - transform.position).y / BlockSystem.CellSize);
        return (x,y);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(BlockSystem.CellSize <=0 || width <=0 || height<=0) return;
        Vector3 origin = transform.position;
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = origin + new Vector3(0, y*BlockSystem.CellSize,0.01f);
            Vector3 end = origin + new Vector3(width*BlockSystem.CellSize,y *BlockSystem.CellSize,0.01f);
            Gizmos.DrawLine(start,end);
        }
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = origin + new Vector3(x * BlockSystem.CellSize,0,0.01f);
            Vector3 end = origin + new Vector3(x *BlockSystem.CellSize,height*BlockSystem.CellSize,0.01f);
            Gizmos.DrawLine(start,end);
        }
    }
}



public class BlockGridCell
{
    private Block block;

    public void SetBlock(Block block)
    {
        this.block = block;
    }

    public bool isEmpty()
    {
        return block == null;
    }
}
