using UnityEngine;
using UnityEngine.EventSystems;

public class BlockClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BlockData blockData;
    private BlockSystem blockSystem;
    private GameManager gameManager;
    private int ownerIndex = -1;
    private bool pickedUp;

    private void Awake()
    {
        blockSystem = FindAnyObjectByType<BlockSystem>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void AssignOwner(int playerIndex) => ownerIndex = playerIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Only an identified controller cursor can take an item from its own menu.
        if (!(eventData is ControllerPointerEventData controllerEvent) || pickedUp ||
            !gameManager.IsBuilding || controllerEvent.PlayerIndex != ownerIndex)
            return;

        if (!blockSystem.TryPickUp(blockData, ownerIndex)) return;
        pickedUp = true;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
