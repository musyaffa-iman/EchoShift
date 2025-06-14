using System;
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
    private PlayerPlacementStrategy playerStrategy;
    private EnemyPlacementStrategy enemyStrategy;
    private PortalPlacementStrategy portalStrategy;

    private void Awake()
    {
        dungeonData = FindObjectOfType<DungeonData>();
        InitializeStrategies();
    }

    private void InitializeStrategies()
    {
        playerStrategy = new PlayerPlacementStrategy(playerPrefab);
        portalStrategy = new PortalPlacementStrategy(portalPrefab);
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
        enemyStrategy = new EnemyPlacementStrategy(enemyPrefabs, roomEnemiesCount, enemiesParent.transform);

        ProcessRooms();
        PlacePortalInLastRoom();
    }

    private void ProcessRooms()
    {
        for (int i = 0; i < dungeonData.Rooms.Count; i++)
        {
            Room room = dungeonData.Rooms[i];
            SetupRoomAccessibility(room);

            // Place player in the first room
            if (i == 0)
            {
                playerStrategy.PlaceAgent(room, dungeonData, i);
            }
            
            // Place enemies
            enemyStrategy.PlaceAgent(room, dungeonData, i);
        }
    }

    private void SetupRoomAccessibility(Room room)
    {
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
    }

    private void PlacePortalInLastRoom()
    {
        if (dungeonData.Rooms.Count > 0)
        {
            Room lastRoom = dungeonData.Rooms[dungeonData.Rooms.Count - 1];
            portalStrategy.PlaceAgent(lastRoom, dungeonData);
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