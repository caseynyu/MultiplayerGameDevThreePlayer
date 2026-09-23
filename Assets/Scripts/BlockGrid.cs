using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BlockGrid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    private BlockGridCell[,]grid;
    [SerializeField] BlockData startPrefab,redEndPrefab,blueEndPrefab,greenEndPrefab;
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

        int randomX = UnityEngine.Random.Range(2,width-2);
        Vector3 startPos = new Vector3(.5f + transform.position.x + randomX*BlockSystem.CellSize,.5f+transform.position.y);
        List<Vector3> blockPositions = new List<Vector3>();
        blockPositions.Add(startPos);blockPositions.Add(new Vector2(startPos.x-1,startPos.y));blockPositions.Add(new Vector2(startPos.x+1,startPos.y));
        Block block = Instantiate(blockPrefab,startPos,Quaternion.identity);
        block.Setup(startPrefab,0);
        SetBuilding(block,blockPositions);
        blockPositions.Clear();

        //set ends

        List<BlockData> endPrefabs = new List<BlockData>();
        endPrefabs.Add(redEndPrefab);
        endPrefabs.Add(blueEndPrefab);
        endPrefabs.Add(greenEndPrefab);

        randomX = UnityEngine.Random.Range(0,width);
        int randomY = UnityEngine.Random.Range(4,9);
        startPos = new Vector3(.5f+transform.position.x+randomX*BlockSystem.CellSize,.5f+transform.position.x+randomY*BlockSystem.CellSize);
        blockPositions.Add(startPos);
        int randomEndBlock = UnityEngine.Random.Range(0,endPrefabs.Count-1);
        block = Instantiate(block,startPos,Quaternion.identity);
        block.Setup(endPrefabs[randomEndBlock],0);
        endPrefabs.RemoveAt(randomEndBlock);
        SetBuilding(block,blockPositions);
        blockPositions.Clear();

        //set end 2

        randomX = UnityEngine.Random.Range(0,width);
        randomY = UnityEngine.Random.Range(10,15);
        startPos = new Vector3(.5f+transform.position.x+randomX*BlockSystem.CellSize,.5f+transform.position.x+randomY*BlockSystem.CellSize);
        blockPositions.Add(startPos);
        randomEndBlock = UnityEngine.Random.Range(0,endPrefabs.Count-1);
        block = Instantiate(block,startPos,Quaternion.identity);
        block.Setup(endPrefabs[randomEndBlock],0);
        endPrefabs.RemoveAt(randomEndBlock);
        SetBuilding(block,blockPositions);
        blockPositions.Clear();

        //set end 3

        randomX = UnityEngine.Random.Range(0,width);
        randomY = UnityEngine.Random.Range(16,21);
        startPos = new Vector3(.5f+transform.position.x+randomX*BlockSystem.CellSize,.5f+transform.position.x+randomY*BlockSystem.CellSize);
        blockPositions.Add(startPos);
        randomEndBlock = UnityEngine.Random.Range(0,endPrefabs.Count-1);
        block = Instantiate(block,startPos,Quaternion.identity);
        block.Setup(endPrefabs[randomEndBlock],0);
        endPrefabs.RemoveAt(randomEndBlock);
        SetBuilding(block,blockPositions);
        blockPositions.Clear();

        


        /*//Vector2 startPos = transform.position;
        int randomX = UnityEngine.Random.Range(2,width-2);
        int randomY = UnityEngine.Random.Range(0,3);
        Vector3 startPos = new Vector3(.5f + transform.position.x + randomX*BlockSystem.CellSize,.5f+transform.position.y+randomY*BlockSystem.CellSize);
        //Debug.Log(startPos);
        List<Vector3> blockPositions = new List<Vector3>();
        blockPositions.Add(startPos);blockPositions.Add(new Vector2(startPos.x-1,startPos.y));blockPositions.Add(new Vector2(startPos.x+1,startPos.y));

        Block block = Instantiate(blockPrefab,startPos,Quaternion.identity);
        block.Setup(startPrefab,0);
        //List<Vector3> spawnBlockPositions=spawnBlocks.GetComponentsInChildren<GridShapeUnit>().ToList();
        //block.Setup(startPrefab,Quaternion.identity);
        


        //Set End

        List<BlockData> endPrefabs = new List<BlockData>();
        endPrefabs.Add(redEndPrefab);
        endPrefabs.Add(blueEndPrefab);
        endPrefabs.Add(greenEndPrefab);

        randomX = UnityEngine.Random.Range(0,width);
        randomY = UnityEngine.Random.Range(4,12);


        startPos = new Vector3(.5f + transform.position.x + randomX*BlockSystem.CellSize,.5f+transform.position.y+randomY*BlockSystem.CellSize);
        foreach (Vector3 checkBlock in blockPositions)
        {
            if(checkBlock == startPos)
            {
                randomX = UnityEngine.Random.Range(0,width);
                randomY = UnityEngine.Random.Range(0,height-1);
                startPos = new Vector3(.5f + transform.position.x + randomX*BlockSystem.CellSize,.5f+transform.position.y+randomY*BlockSystem.CellSize);
            }
        }
        blockPositions.Add(startPos);
        block = Instantiate(blockPrefab,startPos,Quaternion.identity);
        block.Setup(endPrefab,0);
        SetBuilding(block,blockPositions);*/
    }

    /*private void SpawnBlock(float xRangeStart, float xRangeEnd, float yRangeStart, float yRangeEnd)
    {
        int randomX = UnityEngine.Random.Range(0,width);
        int randomY = UnityEngine.Random.Range(4,12);
        Vector3 startPos = new Vector3(.5f + transform.position.x + randomX*BlockSystem.CellSize,.5f+transform.position.y+randomY*BlockSystem.CellSize);
        foreach (Vector3 checkBlock in blockPositions)
        {
            if(checkBlock == startPos)
            {
                x = UnityEngine.Random.Range(0,width);
                y = UnityEngine.Random.Range(0,height-1);
                startPos = new Vector3(.5f + transform.position.x + x*BlockSystem.CellSize,.5f+transform.position.y+y*BlockSystem.CellSize);
            }
        }
        List<Vector3> blockPositions = new List<Vector3>();
        blockPositions.Add(startPos);
        block = Instantiate(blockPrefab,startPos,Quaternion.identity);
        block.Setup(endPrefab,0);
        SetBuilding(block,blockPositions);
    }*/
    

    private void CheckAlreadyPlaced(BlockData data,float rotation)
    {
        Block spawnedBlock = Instantiate(blockPrefab,transform.position,Quaternion.identity);
        spawnedBlock.Setup(data,rotation);
        List<Vector3> buildPositionsToCheck = spawnedBlock.GetComponentInChildren<BlockSprite>().GetAllBuildingPositions();
        while (true)
        {
            float randomX = UnityEngine.Random.Range(0,width);
            float randomY = UnityEngine.Random.Range(0,height-1);
            Vector3 startPos = new Vector3(.5f + transform.position.x + randomX*BlockSystem.CellSize,.5f+transform.position.y+randomY*BlockSystem.CellSize);
            //List<Vector3>() = data.Sprite.GetAllBuildingPositions()

            foreach (Vector3 position in buildPositionsToCheck)
            {
                (int x,int y) = WorldToGridPosition(position);
                if (x<0 || x>=width || y< 0 || y>=height) return;
                if(!grid[x,y].isEmpty()) return;
            }
            break;
        }
        SetBuilding(spawnedBlock,buildPositionsToCheck);
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
