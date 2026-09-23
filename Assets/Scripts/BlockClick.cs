using UnityEngine;
using UnityEngine.EventSystems;

public class BlockClick : MonoBehaviour,IPointerClickHandler
{
    private BlockSystem blockSystem;
    [SerializeField] private BlockData blockData;
    void Awake()
    {
        blockSystem = FindAnyObjectByType<BlockSystem>().GetComponent<BlockSystem>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("test");
        blockSystem.SwitchBlock(blockData);
    }

}
