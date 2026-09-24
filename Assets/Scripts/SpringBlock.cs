using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpringBlock : MonoBehaviour
{
    [SerializeField, Min(1f)]
    [Tooltip("Bounce height relative to a full-height normal jump.")]
    private float jumpHeightMultiplier = 2.5f;

    public float JumpHeightMultiplier => jumpHeightMultiplier;
}
