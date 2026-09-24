using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockClick : MonoBehaviour,IPointerClickHandler
{
    private BlockSystem blockSystem;
    [SerializeField] private BlockData blockData;
    private GameManager gameManager;
    private int ownerIndex = -1;
    void Awake()
    {
        blockSystem = FindAnyObjectByType<BlockSystem>().GetComponent<BlockSystem>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void Configure(BlockData data, int playerIndex)
    {
        blockData = data;
        ownerIndex = playerIndex;
        var icon = GetComponent<Image>();
        var blockRenderer = data.Sprite.GetComponentInChildren<SpriteRenderer>(true);
        icon.sprite = blockRenderer.sprite;
        icon.preserveAspect = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!gameManager.IsBuilding) return;
        int playerIndex = eventData.pointerId <= -100 && eventData.pointerId >= -102
            ? -100 - eventData.pointerId : gameManager.lastPressedGamepadMouse;
        if (ownerIndex >= 0 && playerIndex != ownerIndex) return;
        string color = playerIndex == 0 ? "Red" : playerIndex == 1 ? "Green" : "Blue";
        blockSystem.SwitchBlock(blockData, color, gameObject);
    }
}
