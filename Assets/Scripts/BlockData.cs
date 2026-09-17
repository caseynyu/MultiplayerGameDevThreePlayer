using UnityEngine;


[CreateAssetMenu(menuName = "Data/Block")]
public class BlockData : ScriptableObject
{
    [field:SerializeField] public BlockSprite Sprite {get;private set;}
}
