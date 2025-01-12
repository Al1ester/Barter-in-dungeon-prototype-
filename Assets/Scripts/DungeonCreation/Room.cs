using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomType roomType; 
    public Vector2Int position;
    public bool isConnected = false;

    [HideInInspector] public int width;
    [HideInInspector] public int height;

    public List<Transform> spawnPoints;

    public Room upNeighbour, downNeighbour, leftNeighbour, rightNeighbour;

    public Room(RoomType type, Vector2Int pos)
    {
        roomType = type;
        position = pos;
    }

    void Awake()
    {
        
        CalculateRoomSize();

        spawnPoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("SpawnPoint")) 
            {
                spawnPoints.Add(child);
            }
        }
    }

    void CalculateRoomSize()
    {
        width = 60;
        height = 60;

    // there was code to calculate sizes based on rooms collinder, but now every room is 60x60 so I dont need it (it didnt work :(  )
     
    }

}
