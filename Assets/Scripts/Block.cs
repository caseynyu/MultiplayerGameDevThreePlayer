
using System.Data.Common;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Block : MonoBehaviour
{
    private BlockSprite sprite;
    private BlockData data;

    public void Setup(BlockData data, float rotation)
    {
        this.data = data;
        sprite = Instantiate(data.Sprite,transform.position,Quaternion.identity,transform);
        sprite.Rotate(rotation);
    }
}
