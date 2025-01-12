using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;

    private void Awake()
    {
        if (dungeonGenerator == null)
        {
            dungeonGenerator = FindObjectOfType<DungeonGenerator>();
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (dungeonGenerator != null)
            {
                dungeonGenerator.Generate();
                dungeonGenerator.PositionPlayer();
            }
            else
            {
                Debug.LogWarning("DungeonGenerator reference is not set!");
            }
        }
    }
}
