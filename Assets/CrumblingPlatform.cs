using System.Collections;
using TarodevController;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CrumblingPlatform : MonoBehaviour
{
    [SerializeField, Min(0f)] private float crumbleDelay = 2f;
    private bool crumbleStarted;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (crumbleStarted || collision.gameObject.GetComponentInParent<PlayerController>() == null)
            return;

        crumbleStarted = true;
        StartCoroutine(Crumble());
    }

    private IEnumerator Crumble()
    {
        yield return new WaitForSeconds(crumbleDelay);

        // Remove the entire placed block so its occupied grid cells become available too.
        var block = GetComponentInParent<Block>();
        var blockSprite = GetComponentInParent<BlockSprite>();
        Destroy(block != null ? block.gameObject : blockSprite != null ? blockSprite.gameObject : gameObject);
    }
}
