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
        if(disableCursor)Cursor.visible=false;
        
        
        currentGameState=GameStates.Building;
        statusText.gameObject.SetActive(false);
        buildingUI.SetActive(true);
        playingUI.SetActive(false);
        ResetLevel();
        // Assign each available controller independently so solo testing works too.
        playerRedGamepad = GetGamepad(0);
        playerGreenGamepad = GetGamepad(1);
        playerBlueGamepad = GetGamepad(2);
        AssignCursorGamepad(redActions, playerRedGamepad);
        AssignCursorGamepad(greenActions, playerGreenGamepad);
        AssignCursorGamepad(blueActions, playerBlueGamepad);
    }

    public static Gamepad GetGamepad(int index)
    {
        return index >= 0 && index < Gamepad.all.Count ? Gamepad.all[index] : null;
    }

    private static void AssignCursorGamepad(InputActionAsset actions, Gamepad gamepad)
    {
        // An empty device list prevents an unassigned cursor from using another player's pad.
        actions.devices = gamepad != null
            ? new InputDevice[] { gamepad }
            : System.Array.Empty<InputDevice>();
    }

    private void Update()
    {
        if (IsBuilding)
        {
            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.startButton.wasPressedThisFrame)
                {
                    PlayButton();
                    break;
                }
            }
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
        if (!IsBuilding) return;
        if (redBlock == null || greenBlock == null || blueBlock == null)
        {
            statusText.text = "Waiting for the starting platform.";
            statusTextOn = true;
            return;
        }
        if (Gamepad.all.Count == 0)
        {
            statusText.text = "Connect a controller to start.";
            statusTextOn = true;
            return;
        }
        playerRedGamepad = GetGamepad(0);
        playerGreenGamepad = GetGamepad(1);
        playerBlueGamepad = GetGamepad(2);
        currentGameState=GameStates.Playing;
        gameTimer = 0;
        blockSystem.StopBuilding();
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
        playerRed = SpawnPlayer(redPlayerPrefab, redBlock, playerRedGamepad);
        playerGreen = SpawnPlayer(greenPlayerPrefab, greenBlock, playerGreenGamepad);
        playerBlue = SpawnPlayer(bluePlayerPrefab, blueBlock, playerBlueGamepad);
    }

    private GameObject SpawnPlayer(GameObject prefab, Transform spawn, Gamepad gamepad)
    {
        if (gamepad == null) return null;
        var player = Instantiate(prefab, spawn.position + Vector3.up * spawnOffset, Quaternion.identity);
        var controller = player.GetComponent<PlayerController>();
        controller.currentGamepad = gamepad;
        controller.gameManager = this;
        return player;
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
