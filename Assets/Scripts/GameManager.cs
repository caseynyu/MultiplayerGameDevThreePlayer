using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] BlockSystem blockSystem;
    [SerializeField] BlockGrid blockGrid;
    public GameObject playerRed,playerBlue,playerGreen;
    [SerializeField] GameObject buildingUI;

    enum GameStates
    {
        Building,
        Playing
    }
    private GameStates currentGameState;

    private void Start()
    {
        currentGameState=GameStates.Building;
        buildingUI.SetActive(true);
        ResetLevel();
    }

    public void PlayButton()
    {
        currentGameState=GameStates.Playing;
        buildingUI.SetActive(false);
    }

    private void ResetLevel()
    {
        if (playerRed != null)
        {
            Destroy(playerRed);Destroy(playerGreen);Destroy(playerBlue);
        }
        blockGrid.ResetGrid();
    }
}
