using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonCleaner : MonoBehaviour
{
    private Dictionary<Vector2Int, Room> dungeonGrid;
    private List<Vector2Int> possiblePositions;
    private Dictionary<RoomType, int> roomCounts;

    public void Initialize(Dictionary<Vector2Int, Room> dungeonGrid, List<Vector2Int> possiblePositions, Dictionary<RoomType, int> roomCounts)
    {
        this.dungeonGrid = dungeonGrid;
        this.possiblePositions = possiblePositions;
        this.roomCounts = roomCounts;
    }

    public void ClearDungeon()
    {
        Debug.Log("DungeonCleaner: Clearing dungeon...");


        Room[] allRoomsInScene = FindObjectsOfType<Room>();

        foreach (Room room in allRoomsInScene)
        {
            if (room != null && room.gameObject != null)
            {
                Debug.Log($"Destroying room: {room.gameObject.name}");
                Destroy(room.gameObject);
            }
        }

  
        dungeonGrid.Clear();
        possiblePositions.Clear();

        if (roomCounts != null)
        {
            roomCounts[RoomType.Fight] = 0;
            roomCounts[RoomType.Loot] = 0;
            roomCounts[RoomType.Shop] = 0;
            roomCounts[RoomType.Start] = 0;
            roomCounts[RoomType.End] = 0;
        }

        Debug.Log("DungeonCleaner: Dungeon has been fully cleared.");

        Enemy[] allEnemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in allEnemies)
        {
            Destroy(enemy.gameObject);
        }
    }
}
