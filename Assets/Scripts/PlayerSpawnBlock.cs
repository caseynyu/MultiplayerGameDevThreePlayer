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
        GameObject.FindAnyObjectByType<GameManager>().GetComponent<GameManager>().redBlock = redBlock;
        GameObject.FindAnyObjectByType<GameManager>().GetComponent<GameManager>().blueBlock = blueBlock;
        GameObject.FindAnyObjectByType<GameManager>().GetComponent<GameManager>().greenBlock = greenBlock;
        /*spawnPosRed=redBlock.position + new Vector3(0,spawnOffset,0);
        spawnPosBlue=blueBlock.position + new Vector3(0,spawnOffset,0);
        spawnPosGreen=greenBlock.position + new Vector3(0,spawnOffset,0);
        GameObject playerGreen = GameObject.Instantiate(greenPlayer,spawnPosGreen,quaternion.identity);
        GameObject playerRed = GameObject.Instantiate(redPlayer,spawnPosRed,quaternion.identity);
        GameObject playerBlue = GameObject.Instantiate(bluePlayer,spawnPosBlue,quaternion.identity);
        GameObject.FindAnyObjectByType<GameObject>().GetComponent<GameManager>().playerRed = playerRed;
        GameObject.FindAnyObjectByType<GameObject>().GetComponent<GameManager>().playerBlue = playerBlue;
        GameObject.FindAnyObjectByType<GameObject>().GetComponent<GameManager>().playerGreen = playerGreen;*/
    }
}
