using System.Collections.Generic;
using TarodevController;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text statusText,timerText;
    private bool statusTextOn;
    [SerializeField] float statusTextTimer,statusTextTimerMax, gameTimer,gameTimerMax;
    [SerializeField] BlockSystem blockSystem;
    [SerializeField] BlockGrid blockGrid;
    private GameObject playerRed,playerBlue,playerGreen;
    [SerializeField] GameObject buildingUI,playingUI;
    private Gamepad playerRedGamepad;
    private Gamepad playerBlueGamepad;
    private Gamepad playerGreenGamepad;
    public Transform redBlock,blueBlock,greenBlock;
    [SerializeField] private float spawnOffset=.5f;
    [SerializeField] private GameObject redPlayerPrefab,greenPlayerPrefab,bluePlayerPrefab;
    [SerializeField] private GameObject redCursor,blueCursor,greenCursor;
    private Vector3 spawnPosRed,spawnPosBlue,spawnPosGreen;
    [SerializeField] InputActionAsset redActions,greenActions,blueActions;
    [SerializeField] private bool disableCursor=false;

    [SerializeField] private List<GameObject> placebaleBlocksPrefabs=new List<GameObject>();
    [SerializeField] private GameObject redMenu,greenMenu,blueMenu;

    [SerializeField] private GameObject winButton;

    public int lastPressedGamepadMouse;

    enum GameStates
    {
        Building,
        Playing,
        Results
    }
    private GameStates currentGameState;
    public bool IsBuilding => currentGameState == GameStates.Building;

    private void Start()
    {
        winButton.SetActive(false);
        if(disableCursor)Cursor.visible=false;
        
        
        currentGameState=GameStates.Building;
        statusText.gameObject.SetActive(false);
        buildingUI.SetActive(true);
        playingUI.SetActive(false);
        ResetLevel();
        playerRedGamepad = GetGamepad(0);
        playerGreenGamepad = GetGamepad(1);
        playerBlueGamepad = GetGamepad(2);
        AssignCursorGamepad(redActions, playerRedGamepad);
        AssignCursorGamepad(greenActions, playerGreenGamepad);
        AssignCursorGamepad(blueActions, playerBlueGamepad);
        DealBlocks(redMenu, 0);
        DealBlocks(greenMenu, 1);
        DealBlocks(blueMenu, 2);
    }

    public static Gamepad GetGamepad(int index) => index >= 0 && index < Gamepad.all.Count ? Gamepad.all[index] : null;

    private static void AssignCursorGamepad(InputActionAsset actions, Gamepad gamepad)
    {
        actions.devices = gamepad != null ? new InputDevice[] { gamepad } : System.Array.Empty<InputDevice>();
    }

    private void DealBlocks(GameObject menu, int playerIndex)
    {
        // Each menu owns its own draw pool; never mutate the configured prefab list.
        var pool = new List<GameObject>(placebaleBlocksPrefabs);
        for (int i = 0; i < 4; i++)
        {
            if (pool.Count == 0) pool.AddRange(placebaleBlocksPrefabs);
            if (pool.Count == 0) return;
            int choice = Random.Range(0, pool.Count);
            var item = Instantiate(pool[choice], menu.transform);
            item.GetComponent<BlockClick>().AssignOwner(playerIndex);
            pool.RemoveAt(choice);
        }
    }

    private bool HasRemainingBlocks(GameObject menu, int playerIndex)
    {
        return GetGamepad(playerIndex) != null && menu.GetComponentInChildren<BlockClick>() != null;
    }

    private void Update()
    {
        if(currentGameState== GameStates.Building)
        {
            bool ready = !HasRemainingBlocks(redMenu, 0) && !HasRemainingBlocks(greenMenu, 1) &&
                !HasRemainingBlocks(blueMenu, 2) && !blockSystem.HasPendingBlocks;
            winButton.SetActive(ready);
        }
        if (currentGameState == GameStates.Playing)
        {
            gameTimer+=Time.deltaTime;
            timerText.text = "Time left: " + (gameTimerMax-gameTimer);
            if (gameTimer >= gameTimerMax)
            {
                EndRound();
            } 
        }
        if (statusTextOn)
        {
            statusText.gameObject.SetActive(true);
            statusTextTimer += Time.deltaTime;
            if (statusTextTimer >= statusTextTimerMax)
            {
                statusTextTimer=0;
                statusTextOn=false;
                statusText.gameObject.SetActive(false);
            }
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetLevel();
        }
    }

    private  void EndRound()
    {
        
    }

    public void PlayButton()
    {
        if (!IsBuilding || blockSystem.HasPendingBlocks || HasRemainingBlocks(redMenu, 0) ||
            HasRemainingBlocks(greenMenu, 1) || HasRemainingBlocks(blueMenu, 2)) return;
        currentGameState=GameStates.Playing;
        buildingUI.SetActive(false);
        playingUI.SetActive(true);
        PlayerSpawn();
    }

    private void ResetLevel()
    {
        if (playerRed != null)
        {
            Destroy(playerRed);Destroy(playerGreen);Destroy(playerBlue);
        }
        blockGrid.ResetGrid();
        
    }

    private void PlayerSpawn()
    {
        spawnPosRed=redBlock.position + new Vector3(0,spawnOffset,0);
        spawnPosBlue=blueBlock.position + new Vector3(0,spawnOffset,0);
        spawnPosGreen=greenBlock.position + new Vector3(0,spawnOffset,0);
        playerGreen = GameObject.Instantiate(greenPlayerPrefab,spawnPosGreen,Quaternion.identity);
        playerRed = GameObject.Instantiate(redPlayerPrefab,spawnPosRed,Quaternion.identity);
        playerBlue = GameObject.Instantiate(bluePlayerPrefab,spawnPosBlue,Quaternion.identity);
        playerRed.GetComponent<PlayerController>().currentGamepad = playerRedGamepad;
        playerBlue.GetComponent<PlayerController>().currentGamepad = playerBlueGamepad;
        playerGreen.GetComponent<PlayerController>().currentGamepad = playerGreenGamepad;
        playerRed.GetComponent<PlayerController>().gameManager = this;
        playerGreen.GetComponent<PlayerController>().gameManager = this;
        playerBlue.GetComponent<PlayerController>().gameManager = this;

    }
    public void Death(GameObject playerDied)
    {
        if (playerDied == playerRed)
        {
            RedDeath();
        }
        if (playerDied == playerGreen)
        {
            GreenDeath();
        }
        if (playerDied == playerBlue)
        {
            BlueDeath();
        }
    }
    public void PlayerWin(GameObject playerWon)
    {
        if (playerWon == playerRed)
        {
            RedWin();
        }
        if (playerWon == playerGreen)
        {
            GreenWin();
        }
        if (playerWon == playerBlue)
        {
            BlueWin();
        }
    }
    public void RedDeath()
    {
        Destroy(playerRed);
        statusText.text = "Red Player Died";
        statusTextOn=true;
        playerRed = null;
        if(playerBlue==null && playerGreen == null)
        {
            EndRound();
        }
    }
    public void BlueDeath()
    {
        Destroy(playerBlue);
        statusText.text = "Blue Player Died";
        statusTextOn=true;
        playerRed = null;
        if(playerRed==null && playerGreen == null)
        {
            EndRound();
        }
    }
    public void GreenDeath()
    {
        Destroy(playerGreen);
        statusText.text = "Green Player Died";
        statusTextOn=true;
        playerRed = null;
        if(playerRed==null && playerBlue == null)
        {
            EndRound();
        }
    }
    public void RedWin()
    {
        Destroy(playerRed);
        statusText.text = "Red Player Won!";
        statusTextOn=true;
        playerRed = null;
        if(playerBlue==null && playerGreen == null)
        {
            EndRound();
        }
    }
    public void BlueWin()
    {
        Destroy(playerBlue);
        statusText.text = "Blue Player Won!";
        statusTextOn=true;
        playerRed = null;
        if(playerRed==null && playerGreen == null)
        {
            EndRound();
        }
    }
    public void GreenWin()
    {
        Destroy(playerGreen);
        statusText.text = "Green Player Won!";
        statusTextOn=true;
        playerRed = null;
        if(playerRed==null && playerBlue == null)
        {
            EndRound();
        }
    }
}
