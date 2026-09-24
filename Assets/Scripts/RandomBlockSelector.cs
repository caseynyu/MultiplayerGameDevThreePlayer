using System.Collections.Generic;
using UnityEngine;

public class RandomBlockSelector : MonoBehaviour
{
    [SerializeField, Range(0, 2)] private int playerIndex;
    [SerializeField] private BlockData[] availableBlocks;

    private void Start()
    {
        var slots = GetComponentsInChildren<BlockClick>(true);
        var remaining = new List<BlockData>();
        foreach (var block in availableBlocks)
        {
            if (block != null && block.Sprite != null && !remaining.Contains(block))
                remaining.Add(block);
        }

        if (remaining.Count < slots.Length)
        {
            Debug.LogError("The selector needs at least one distinct block type per slot.", this);
            return;
        }

        // Draw without replacement, independently for each player's selector.
        foreach (var slot in slots)
        {
            int choice = Random.Range(0, remaining.Count);
            slot.Configure(remaining[choice], playerIndex);
            remaining.RemoveAt(choice);
        }
    }
}
