using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class AgentPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private List<int> roomEnemiesCount;
    DungeonData dungeonData;
    [SerializeField] private bool showGizmo = false;
    
    private GameObject enemiesParent;

    private void Awake()
    {
        dungeonData = FindObjectOfType<DungeonData>();
    }


    public void OnDungeonGenerated()
    {
        PlaceAgents();
    }

    public void PlaceAgents()
    {
        if (dungeonData == null)
            return;

        enemiesParent = new GameObject("Enemies");

        for (int i = 0; i < dungeonData.Rooms.Count; i++)
        {
            Room room = dungeonData.Rooms[i];
            RoomGraph roomGraph = new RoomGraph(room.FloorTiles);

            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(room.FloorTiles);
            roomFloor.IntersectWith(dungeonData.Path);

            Dictionary<Vector2Int, Vector2Int> roomMap;

            if (roomFloor.Count == 0)
            {
                Vector2Int startPos = new Vector2Int((int)room.RoomCenterPos.x, (int)room.RoomCenterPos.y);
                
                if (!room.FloorTiles.Contains(startPos))
                {
                    startPos = room.FloorTiles.First();
                }
                
                roomMap = roomGraph.RunBFS(startPos, room.PropPositions);
            }
            else
            {
                roomMap = roomGraph.RunBFS(roomFloor.First(), room.PropPositions);
            }

            room.PositionsAccessibleFromPath = roomMap.Keys.OrderBy(x => Guid.NewGuid()).ToList();

            if(roomEnemiesCount.Count > i)
            {
                PlaceEnemies(room, roomEnemiesCount[i]);
            }

            // Place player in the first room (index 0)
            if(i == 0)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null)
                {
                    player = Instantiate(playerPrefab);
                }
                player.transform.localPosition = dungeonData.Rooms[i].RoomCenterPos + Vector2.one*0.5f;
                dungeonData.PlayerReference = player;

                // Set Cinemachine camera to follow the player
                var vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    vcam.Follow = player.transform;
                }
            }
        }

        // Place portal in the last room
        PlacePortal(dungeonData.Rooms[dungeonData.Rooms.Count - 1]);
    }

    private void PlacePortal(Room room)
    {
        if (portalPrefab == null) return;

        Vector2Int portalPosition;
        
        if (dungeonData.Rooms.Count == 1)
        {
            portalPosition = FindFurthestPositionFromCenter(room);
            if (portalPosition == Vector2Int.zero)
            {
                Debug.LogWarning("No suitable furthest position found for portal!");
                return;
            }
        }
        else
        {
            Vector2Int centerPos = new Vector2Int((int)room.RoomCenterPos.x, (int)room.RoomCenterPos.y);
            if (room.FloorTiles.Contains(centerPos) && 
                !room.PropPositions.Contains(centerPos) &&
                room.PositionsAccessibleFromPath.Contains(centerPos))
            {
                portalPosition = centerPos;
            }
            else
            {
                // Find the first available position from accessible positions
                portalPosition = room.PositionsAccessibleFromPath
                    .Where(pos => !room.PropPositions.Contains(pos))
                    .FirstOrDefault();
                
                if (portalPosition == Vector2Int.zero)
                {
                    Debug.LogWarning("No suitable position found for portal in the room!");
                    return;
                }
            }
        }

        // Instantiate the portal
        GameObject portal = Instantiate(portalPrefab);
        portal.transform.localPosition = (Vector2)portalPosition + Vector2.one * 0.5f;
        
        // Add the portal to the room's prop positions to prevent other objects from spawning there
        room.PropPositions.Add(portalPosition);
    }

    private Vector2Int FindFurthestPositionFromCenter(Room room)
    {
        Vector2Int centerPos = new Vector2Int((int)room.RoomCenterPos.x, (int)room.RoomCenterPos.y);
        
        // Find all available positions (accessible and not occupied by props)
        var availablePositions = room.PositionsAccessibleFromPath
            .Where(pos => !room.PropPositions.Contains(pos))
            .ToList();

        if (availablePositions.Count == 0)
            return Vector2Int.zero;

        // Calculate distances and find the furthest position
        Vector2Int furthestPosition = availablePositions[0];
        float maxDistance = Vector2Int.Distance(centerPos, furthestPosition);

        foreach (Vector2Int pos in availablePositions)
        {
            float distance = Vector2Int.Distance(centerPos, pos);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestPosition = pos;
            }
        }

        return furthestPosition;
    }

    private void PlaceEnemies(Room room, int enemysCount)
    {
        for (int k = 0; k < enemysCount; k++)
        {
            if (room.PositionsAccessibleFromPath.Count <= k)
            {
                return;
            }
            GameObject prefabToSpawn = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
            GameObject enemy = Instantiate(prefabToSpawn);
            enemy.transform.localPosition = (Vector2)room.PositionsAccessibleFromPath[k] + Vector2.one*0.5f;
            
            if (enemiesParent != null)
            {
                enemy.transform.SetParent(enemiesParent.transform);
            }
            
            room.EnemiesInTheRoom.Add(enemy);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (dungeonData == null || showGizmo == false)
            return;
        foreach (Room room in dungeonData.Rooms)
        {
            Color color = Color.green;
            color.a = 0.3f;
            Gizmos.color = color;

            foreach (Vector2Int pos in room.PositionsAccessibleFromPath)
            {
                Gizmos.DrawCube((Vector2)pos + Vector2.one * 0.5f, Vector2.one);
            }
        }
    }
}

public class RoomGraph
{
    public static List<Vector2Int> fourDirections = new()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();

    public RoomGraph(HashSet<Vector2Int> roomFloor)
    {
        foreach (Vector2Int pos in roomFloor)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();
            foreach (Vector2Int direction in fourDirections)
            {
                Vector2Int newPos = pos + direction;
                if (roomFloor.Contains(newPos))
                {
                    neighbours.Add(newPos);
                }
            }
            graph.Add(pos, neighbours);
        }
    }

    public Dictionary<Vector2Int, Vector2Int> RunBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedNodes)
    {
        Queue<Vector2Int> nodesToVisit = new Queue<Vector2Int>();
        nodesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();
        visitedNodes.Add(startPos);

        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int>();
        map.Add(startPos, startPos);

        while (nodesToVisit.Count > 0)
        {
            Vector2Int node = nodesToVisit.Dequeue();
            List<Vector2Int> neighbours = graph[node];

            foreach (Vector2Int neighbourPosition in neighbours)
            {
                if (visitedNodes.Contains(neighbourPosition) == false &&
                    occupiedNodes.Contains(neighbourPosition) == false)
                {
                    nodesToVisit.Enqueue(neighbourPosition);
                    visitedNodes.Add(neighbourPosition);
                    map[neighbourPosition] = node;
                }
            }
        }

        return map;
    }
}