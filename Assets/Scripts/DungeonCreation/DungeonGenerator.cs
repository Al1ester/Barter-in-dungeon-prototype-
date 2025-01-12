using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum RoomType { Start, Fight, Loot, Shop, End }

public class DungeonGenerator : MonoBehaviour
{
    public List<GameObject> startRoomPrefabs;
    public List<GameObject> fightRoomPrefabs;
    public List<GameObject> lootRoomPrefabs;
    public List<GameObject> shopRoomPrefabs;
    public List<GameObject> endRoomPrefabs; 

    public List<GameObject> corridorPrefabs;

    public int maxShopRooms = 3;
    public int maxFightRooms = 3;
    public int maxLootRooms = 2;
    public int totalRoomCount = 10;
    public int maxEndRooms = 1;

    private Dictionary<Vector2Int, Room> dungeonGrid = new Dictionary<Vector2Int, Room>();
    private Dictionary<RoomType, int> roomCounts = new Dictionary<RoomType, int>();
    private List<Vector2Int> possiblePositions = new List<Vector2Int>();

    private Vector2Int startRoomPosition = Vector2Int.zero;  

   
    public Transform dungeonParent;

    private DungeonCleaner dungeonCleaner;
    
    public SpawnEnemy enemySpawner;

    void Start()
    {
        PositionPlayer();
        Generate();
      
        roomCounts[RoomType.Fight] = 0;
        roomCounts[RoomType.Loot] = 0;
        roomCounts[RoomType.Shop] = 0;
        roomCounts[RoomType.Start] = 0;
        roomCounts[RoomType.End] = 0;

        enemySpawner = GetComponent<SpawnEnemy>();
        dungeonCleaner = GetComponent<DungeonCleaner>();

        if (dungeonCleaner != null)
        {
            dungeonCleaner.Initialize(dungeonGrid, possiblePositions, roomCounts);
        }
        else
        {
            Debug.LogError("DungeonCleaner component not found!");
        }

    }

    public void Generate()
    {
        Debug.Log("Generate() called!");

        if (dungeonCleaner != null)
        {
            dungeonCleaner.ClearDungeon();
            Debug.Log("ClearDungeon() called!");

        }

        StartCoroutine(DelayedGeneration());
    }

    private IEnumerator DelayedGeneration()
    {       
        yield return new WaitForEndOfFrame();

        Debug.Log("Starting dungeon generation after clearing...");
        dungeonGrid = new Dictionary<Vector2Int, Room>();
        possiblePositions = new List<Vector2Int>();
        roomCounts[RoomType.Fight] = 0;
        roomCounts[RoomType.Loot] = 0;
        roomCounts[RoomType.Shop] = 0;
        roomCounts[RoomType.Start] = 0;
        roomCounts[RoomType.End] = 0;

        StartCoroutine(GenerateDungeonWithDelay());
        yield return new WaitForEndOfFrame();

       
    }

    private IEnumerator GenerateDungeonWithDelay()
    {

        yield return new WaitForSeconds(1f);

        StartCoroutine(GenerateDungeonCoroutine());

        yield return new WaitForSeconds(2f);
        enemySpawner.SpawnRandomEnemy();
        
    }

    IEnumerator GenerateDungeonCoroutine()
    {
        Debug.Log("Starting dungeon generation...");

        Debug.Log("Attempting to place the Start room...");
        PlaceRoom(RoomType.Start, Vector2Int.zero);
        startRoomPosition = Vector2Int.zero; 
        int roomsPlaced = 1;
        Debug.Log($"Placed Start room at {startRoomPosition}. Total rooms placed: {roomsPlaced}");

        while (roomsPlaced < totalRoomCount - 1)  
        {
            Vector2Int randomExistingPosition = GetRandomExistingPosition();
            Vector2Int newRoomPosition = GetRandomDirectionPosition(randomExistingPosition);

            RoomType randomRoomType = GetRandomRoomType();
            Debug.Log($"Trying to place room type: {randomRoomType}");

            GameObject roomPrefab = GetRandomPrefabForType(randomRoomType);
            if (roomPrefab == null) continue;

            Room roomComponent = roomPrefab.GetComponent<Room>();
            int width = roomComponent.width;
            int height = roomComponent.height;

            if (CanPlaceRoom(randomRoomType, newRoomPosition, width, height))
            {
                PlaceRoom(randomRoomType, newRoomPosition);
                roomsPlaced++;
                Debug.Log($"Placed {randomRoomType} room at {newRoomPosition}. Total rooms placed: {roomsPlaced}");
            }

            yield return null;
        }

        PlaceEndRoomAtFurthestPosition();

        Debug.Log("Dungeon generation complete!");
    }

    void PlaceRoom(RoomType roomType, Vector2Int position)
    {
        if (!dungeonGrid.ContainsKey(position))
        {
            GameObject roomPrefab = GetRandomPrefabForType(roomType);
            if (roomPrefab == null) return;

            Room newRoom = roomPrefab.GetComponent<Room>();
            newRoom.position = position;
            newRoom.roomType = roomType;

            int width = newRoom.width;
            int height = newRoom.height;

            if (!CanPlaceRoom(roomType, position, width, height)) return;

            Vector3 placementPosition = new Vector3(
     position.x * 60 - (width / 2f),
     position.y * 60 - (height / 2f),
     0);


            GameObject newRoomObj = Instantiate(roomPrefab, placementPosition, Quaternion.identity);
            newRoomObj.transform.SetParent(null);
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.MoveGameObjectToScene(newRoomObj, activeScene);

            Debug.Log($"Placed {roomType} room at position {position} (grid) -> {placementPosition} (world). Size: {width}x{height}");

          
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int tilePosition = position + new Vector2Int(x, y);
                    dungeonGrid[tilePosition] = newRoom;
                }
            }

        
            dungeonGrid[position] = newRoom;
            possiblePositions.Add(position);

        
            if (roomCounts.ContainsKey(roomType))
                roomCounts[roomType]++;
        }
    }


    void PlaceEndRoomAtFurthestPosition()
    {
      
        Vector2Int furthestRoomPosition = startRoomPosition;
        int maxDistance = 0;

        foreach (var room in dungeonGrid)
        {
            Vector2Int roomPosition = room.Key;
            int distance = GetManhattanDistance(startRoomPosition, roomPosition);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestRoomPosition = roomPosition;
            }
        }

        Vector2Int[] adjacentPositions = {
        furthestRoomPosition + Vector2Int.left,
        furthestRoomPosition + Vector2Int.right,
        furthestRoomPosition + Vector2Int.up,
        furthestRoomPosition + Vector2Int.down
    };

        foreach (var position in adjacentPositions)
        {
            if (CanPlaceRoom(RoomType.End, position, 1, 1)) 
            {
                PlaceRoom(RoomType.End, position);
                Debug.Log($"Placed End room at {position}");
                return;
            }
        }

        Debug.LogWarning("No available adjacent positions for End room.");
    }


    int GetManhattanDistance(Vector2Int start, Vector2Int end)
    {
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
    }

    GameObject GetRandomPrefabForType(RoomType roomType)
    {
        List<GameObject> prefabList = null;

        switch (roomType)
        {
            case RoomType.Start:
                prefabList = startRoomPrefabs;
                break;
            case RoomType.Fight:
                prefabList = fightRoomPrefabs;
                break;
            case RoomType.Loot:
                prefabList = lootRoomPrefabs;
                break;
            case RoomType.Shop:
                prefabList = shopRoomPrefabs;
                break;
            case RoomType.End:
                prefabList = endRoomPrefabs;
                break;
        }

        return prefabList != null && prefabList.Count > 0 ? prefabList[Random.Range(0, prefabList.Count)] : null;
    }

    RoomType GetRandomRoomType()
    {
        RoomType randomType;
        do
        {
            randomType = (RoomType)Random.Range(1, System.Enum.GetValues(typeof(RoomType)).Length);
        } while (!IsRoomTypeAvailable(randomType) || randomType == RoomType.End);
        return randomType;
    }

    bool IsRoomTypeAvailable(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Shop:
                return roomCounts[RoomType.Shop] < maxShopRooms;
            case RoomType.Fight:
                return roomCounts[RoomType.Fight] < maxFightRooms;
            case RoomType.Loot:
                return roomCounts[RoomType.Loot] < maxLootRooms;
            case RoomType.End:
                return roomCounts[RoomType.End] < maxEndRooms;
            default:
                return true;
        }
    }

    bool CanPlaceRoom(RoomType roomType, Vector2Int position, int width, int height)
    {
   
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int tilePosition = position + new Vector2Int(x, y);

                if (dungeonGrid.ContainsKey(tilePosition))
                    return false;
            }
        }

   
        Room leftNeighbor = GetRoomAtPosition(position + Vector2Int.left);
        Room rightNeighbor = GetRoomAtPosition(position + Vector2Int.right);
        Room upNeighbor = GetRoomAtPosition(position + Vector2Int.up);
        Room downNeighbor = GetRoomAtPosition(position + Vector2Int.down);

        if (roomType == RoomType.Shop)
        {
            // Ensure no Loot rooms are adjacent
            if ((leftNeighbor != null && leftNeighbor.roomType == RoomType.Loot) ||
                (rightNeighbor != null && rightNeighbor.roomType == RoomType.Loot) ||
                (upNeighbor != null && upNeighbor.roomType == RoomType.Loot) ||
                (downNeighbor != null && downNeighbor.roomType == RoomType.Loot) ||
                (leftNeighbor != null && leftNeighbor.roomType == RoomType.Shop) ||
                (rightNeighbor != null && rightNeighbor.roomType == RoomType.Shop) ||
                (upNeighbor != null && upNeighbor.roomType == RoomType.Shop) ||
                (downNeighbor != null && downNeighbor.roomType == RoomType.Shop))
            {
                return false;
            }
        }
        else if (roomType == RoomType.Loot)
        {
            // Ensure no Shop rooms are adjacent
            if ((leftNeighbor != null && leftNeighbor.roomType == RoomType.Shop) ||
                (rightNeighbor != null && rightNeighbor.roomType == RoomType.Shop) ||
                (upNeighbor != null && upNeighbor.roomType == RoomType.Shop) ||
                (downNeighbor != null && downNeighbor.roomType == RoomType.Shop) ||
                (leftNeighbor != null && leftNeighbor.roomType == RoomType.Loot) ||
                (rightNeighbor != null && rightNeighbor.roomType == RoomType.Loot) ||
                (upNeighbor != null && upNeighbor.roomType == RoomType.Loot) ||
                (downNeighbor != null && downNeighbor.roomType == RoomType.Loot))
            {
                return false;
            }
        }

        return true; 
    }



    Vector2Int GetRandomExistingPosition()
    {
        return possiblePositions[Random.Range(0, possiblePositions.Count)];
    }

    Vector2Int GetRandomDirectionPosition(Vector2Int currentPos)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector2Int randomDir = directions[Random.Range(0, directions.Length)];
        return currentPos + randomDir;
    }

    Room GetRoomAtPosition(Vector2Int position)
    {
        dungeonGrid.TryGetValue(position, out Room room);
        return room;
    }

   

   /* void GenerateCorridors()    One day I will fully redo this script, till than I cry
    {
        foreach (var position in possiblePositions)
        {
            Room currentRoom = dungeonGrid[position];

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            foreach (Vector2Int direction in directions)
            {
                Vector2Int corridorPosition = position + direction;

                // Only add corridors in empty tiles
                if (!dungeonGrid.ContainsKey(corridorPosition))
                {
                    GameObject corridorPrefab = corridorPrefabs[Random.Range(0, corridorPrefabs.Count)];
                    GameObject corridor = Instantiate(corridorPrefab, new Vector3(corridorPosition.x, corridorPosition.y, 0), Quaternion.identity);
                    dungeonGrid[corridorPosition] = corridor.GetComponent<Room>(); // Optionally mark it as Room
                }
            }
        }
    }*/




    public void PositionPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(30.5f, -20f, 0);
        }
    }


}


