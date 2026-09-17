using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnBlock : MonoBehaviour
{
[SerializeField] private Transform redBlock,blueBlock,greenBlock;
[SerializeField] private Vector3 spawnPosRed,spawnPosBlue,spawnPosGreen;
[SerializeField] private GameObject redPlayer,greenPlayer,bluePlayer;
[SerializeField] private float spawnOffset = .8f;

private void Awake()
    {
        spawnPosRed=redBlock.position + new Vector3(0,spawnOffset,0);
        spawnPosBlue=blueBlock.position + new Vector3(0,spawnOffset,0);
        spawnPosGreen=greenBlock.position + new Vector3(0,spawnOffset,0);
        GameObject.Instantiate(greenPlayer,spawnPosGreen,quaternion.identity);
        GameObject.Instantiate(redPlayer,spawnPosRed,quaternion.identity);
        GameObject.Instantiate(bluePlayer,spawnPosBlue,quaternion.identity);
    }
}
