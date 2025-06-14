using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomsFirst : AbstractDungeonGenerator
{
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField] [Range(0, 10)] private int offset = 1;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomList = ProceduralGenerationAlgorithms.BinarySpacePartition(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)),
            minRoomWidth, minRoomHeight);
        if (roomList == null || roomList.Count == 0)
        {
            return;
        }

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        List<Vector2Int> roomCenters = new List<Vector2Int>();

        foreach (var roomBounds in roomList)
        {
            HashSet<Vector2Int> roomFloor = CreateRoomFloor(roomBounds);
            floor.UnionWith(roomFloor); Vector2Int center = (Vector2Int)Vector3Int.RoundToInt(roomBounds.center);
            Room room = new Room(center, roomFloor);

            dungeonData.Rooms.Add(room);
            roomCenters.Add(center);
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);
        dungeonData.Path.UnionWith(corridors); tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        // Process room data before prop placement
        var roomDataExtractor = FindObjectOfType<RoomDataExtractor>();
        if (roomDataExtractor != null)
        {
            roomDataExtractor.ProcessRooms();
            // Place props after room data is processed
            var propManager = FindObjectOfType<PropPlacementManager>();
            if (propManager != null)
            {
                propManager.ProcessRooms();
            }
        }
    }

    private HashSet<Vector2Int> CreateRoomFloor(BoundsInt bounds)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int col = offset; col < bounds.size.x - offset; col++)
        {
            for (int row = offset; row < bounds.size.y - offset; row++)
            {
                Vector2Int pos = (Vector2Int)bounds.min + new Vector2Int(col, row);
                floor.Add(pos);
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentCenter = roomCenters[UnityEngine.Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPoint(currentCenter, roomCenters);
            roomCenters.Remove(closest);
            corridors.UnionWith(CreateCorridor(currentCenter, closest));
            currentCenter = closest;
        }
        return corridors;
    }

    private Vector2Int FindClosestPoint(Vector2Int current, List<Vector2Int> points)
    {
        Vector2Int closest = Vector2Int.zero;
        float shortestDistance = float.MaxValue;
        foreach (var point in points)
        {
            float dist = Vector2.Distance(current, point);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                closest = point;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int start, Vector2Int end)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        Vector2Int position = start;
        corridor.Add(position);

        while (position.y != end.y)
        {
            position += (end.y > position.y) ? Vector2Int.up : Vector2Int.down;
            corridor.Add(position);
        }
        while (position.x != end.x)
        {
            position += (end.x > position.x) ? Vector2Int.right : Vector2Int.left;
            corridor.Add(position);
        }

        return corridor;
    }
}
