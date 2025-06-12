using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

// Updated CorridorFirst
public class CorridorFirst : AbstractDungeonGenerator
{
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    [SerializeField] private RandomWalkData rw_parameters;
    [SerializeField] [Range(0.1f, 1)] private float roomPercent = 1f;

    protected override void RunProceduralGeneration()
    {
        GenerateCorridorFirstDungeon();
    }

    private void GenerateCorridorFirstDungeon()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        CreateCorridors(floorPositions, potentialRoomPositions);    
        dungeonData.Path.UnionWith(floorPositions);
        
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        CreateRoomsAtDeadEnds(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);

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
            else
            {
                Debug.LogWarning("[CorridorFirst] PropPlacementManager not found!");
            }
        }
        else
        {
            Debug.LogWarning("[CorridorFirst] RoomDataExtractor not found!");
        }
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        Vector2Int currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> potentialRooms = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomCount).ToList();
        foreach (var roomPosition in potentialRooms)
        {
            var roomFloor = RunRandomWalk(rw_parameters, roomPosition);
            roomPositions.UnionWith(roomFloor);

            Room room = new Room(roomPosition, roomFloor);
            AnalyzeRoomStructure(room);
            dungeonData.Rooms.Add(room);
        }
        return roomPositions;
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (var position in floorPositions)
        {
            int neighbourCount = 0;
            foreach (var direction in Direction2D.Directions)
            {
                if (floorPositions.Contains(position + direction))
                    neighbourCount++;
            }
            if (neighbourCount == 1)
                deadEnds.Add(position);
        }

        return deadEnds;
    }

    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (!roomFloors.Contains(position))
            {
                var roomFloor = RunRandomWalk(rw_parameters, position);
                roomFloors.UnionWith(roomFloor);

                Room room = new Room(position, roomFloor);
                AnalyzeRoomStructure(room);
                dungeonData.Rooms.Add(room);
            }
        }
    }

    private HashSet<Vector2Int> RunRandomWalk(RandomWalkData rw_parameters, Vector2Int position)
    {
        Vector2Int currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < rw_parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.RandomWalk(currentPosition, rw_parameters.walkLength);
            floorPositions.UnionWith(path);

            if (rw_parameters.startRandom)
                currentPosition = floorPositions.ElementAt(UnityEngine.Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }
}
