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

    public int lastPressedGamepadMouse;

    enum GameStates
    {
        Building,
        Playing,
        Results
    }
    private GameStates currentGameState;

    private void Start()
    {
        Cursor.visible=false;
        
        currentGameState=GameStates.Building;
        statusText.gameObject.SetActive(false);
        buildingUI.SetActive(true);
        playingUI.SetActive(false);
        ResetLevel();
        //redCursor.GetComponent<PlayerInput>().user.PerformPairingWithDevice(Gamepad.all[0]);
        //PlayButton();
        redActions.devices = new[] {Gamepad.all[0]};
        blueActions.devices = new[] {Gamepad.all[2]};
        greenActions.devices = new[] {Gamepad.all[1]};
        //Debug.Log(redActions.devices);
        //redCursor.GetComponent<VirtualMouseInput>().stickAction.action;
        if(Gamepad.all.Count > 2)
        {
            playerRedGamepad = Gamepad.all[0];
            playerBlueGamepad = Gamepad.all[2];
            playerGreenGamepad = Gamepad.all[1];
        }
        else
        {
            Debug.Log("Not enough controllers connected");
        }
    }

    private void Update()
    {
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
