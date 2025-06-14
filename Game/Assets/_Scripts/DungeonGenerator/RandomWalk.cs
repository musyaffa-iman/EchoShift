using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RandomWalk : AbstractDungeonGenerator
{
    [SerializeField] protected RandomWalkData rw_parameters;

    protected override void RunProceduralGeneration()
    {
        GenerateRandomWalkDungeon();
    }

    private void GenerateRandomWalkDungeon()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(rw_parameters, startPosition);
        Vector2Int center = startPosition;

        Room room = new Room(center, floorPositions);
        dungeonData.Rooms.Add(room);

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
                Debug.LogWarning("[RandomWalk] PropPlacementManager not found!");
            }
        }
        else
        {
            Debug.LogWarning("[RandomWalk] RoomDataExtractor not found!");
        }
    }

    protected HashSet<Vector2Int> RunRandomWalk(RandomWalkData rw_parameters, Vector2Int position)
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
