using System.Reflection;
using TarodevController;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public bool item_activated = false;

    [Header("item selection")]
    public bool isJetBoots;
    public bool isSpringShoes;
    public bool isSlimeBoots;
    public bool isCloudBottle;
    public bool isIronBoots;
    public bool isClimbingMittens;
    public bool isJetPack;

    [Header("item effects")]
    public float jetMultNum = 1.5f;
    public float springMultNum = 1.5f;
    public float slimeMultNum = 0.8f;
    public float slimeVelNum =10f;
    public float cloudJumpPowerNum =15f;
    public int cloudJumpNum = 1;
    public float ironMultNum = 0.6f;
    public string spikeName = "Spike";

    private PlayerController playerController;
    private Rigidbody2D rigidbody2D;
    //private FieldInfo velocityField, 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       if (playerController == null) playerController = GetComponentInParent<PlayerController>(); 
       if (rigidbody2D == null) rigidbody2D = playerController.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (item_activated)
        {
            if (isJetBoots)
            {
                Vector2 vector2 = rigidbody2D.linearVelocity;
                vector2.x *= jetMultNum;
                rigidbody2D.linearVelocity = vector2;
            }
            else if(isSpringShoes)
            {
                Vector2 vector2 = rigidbody2D.linearVelocity;
                vector2.y *= jetMultNum;
                rigidbody2D.linearVelocity = vector2;
            }
            else if(isSlimeBoots)
            {
                
            }
            else if(isCloudBottle)
            {
                
            }
            else if(isIronBoots)
            {
                
            }
            else if(isClimbingMittens)
            {
                
            }
            else if(isJetPack)
            {
                
            }
        }
    }
}
