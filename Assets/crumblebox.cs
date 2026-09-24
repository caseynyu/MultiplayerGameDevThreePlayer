using System.Collections;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    [SerializeField] private float crumbleDelay = 2f;

    private bool crumbleStarted;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (crumbleStarted)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            crumbleStarted = true;
            StartCoroutine(Crumble());
        }
    }

    private IEnumerator Crumble()
    {
        yield return new WaitForSeconds(crumbleDelay);

        Destroy(gameObject);
    }
}