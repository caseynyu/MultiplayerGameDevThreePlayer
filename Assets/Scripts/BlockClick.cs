using UnityEngine;
using UnityEngine.EventSystems;

public class BlockClick : MonoBehaviour,IPointerClickHandler
{
    private BlockSystem blockSystem;
    [SerializeField] private BlockData blockData;
    private GameManager gameManager;
    void Awake()
    {
        blockSystem = FindAnyObjectByType<BlockSystem>().GetComponent<BlockSystem>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(gameManager.lastPressedGamepadMouse==0)
        {
            blockSystem.SwitchBlock(blockData,"Red",gameObject);
        }
        if(gameManager.lastPressedGamepadMouse==1)
        {
            blockSystem.SwitchBlock(blockData,"Green",gameObject);
        }
        if(gameManager.lastPressedGamepadMouse==2)
        {
            blockSystem.SwitchBlock(blockData,"Blue",gameObject);
        }
    }

}
