using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    // Performs a random walk starting from a given position
    public static HashSet<Vector2Int> RandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>() { startPosition };
        Vector2Int currentPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            currentPosition += Direction2D.GetRandomDirection();
            path.Add(currentPosition);
        }
        return path;
    }

    // Generates a straight corridor starting from a given position
    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>() { startPosition };
        Vector2Int direction = Direction2D.GetRandomDirection();
        Vector2Int currentPosition = startPosition;

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return corridor;
    }

    // Uses Binary Space Partitioning (BSP) to divide a given space into rooms
    public static List<BoundsInt> BinarySpacePartition(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>(); // Queue to store rooms to be split
        List<BoundsInt> roomsList = new List<BoundsInt>();    // Final list of rooms
        
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();

            // Ensure room is large enough to be split
            if (room.size.x >= minWidth && room.size.y >= minHeight)
            {
                bool splitHorizontally = Random.value < 0.5f; // Randomly decide split direction
                
                if (splitHorizontally)
                {
                    if (room.size.y >= minHeight * 2)
                        SplitRoom(roomsQueue, room, true);
                    else if (room.size.x >= minWidth * 2)
                        SplitRoom(roomsQueue, room, false);
                    else
                        roomsList.Add(room); // If too small, add it to the final list
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                        SplitRoom(roomsQueue, room, false);
                    else if (room.size.y >= minHeight * 2)
                        SplitRoom(roomsQueue, room, true);
                    else
                        roomsList.Add(room);
                }
            }
        }
        return roomsList;
    }

    // Splits a room either horizontally or vertically and enqueues the new rooms
    private static void SplitRoom(Queue<BoundsInt> roomsQueue, BoundsInt room, bool splitHorizontally)
    {
        if (splitHorizontally)
        {
            // Randomly select a Y-coordinate to split the room
            int ySplit = Random.Range(1, room.size.y);
            
            // Create two new sub-rooms
            BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
            BoundsInt room2 = new BoundsInt(
                new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
                new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
            
            // Add new rooms to the queue
            roomsQueue.Enqueue(room1);
            roomsQueue.Enqueue(room2);
        }
        else
        {
            // Randomly select an X-coordinate to split the room
            int xSplit = Random.Range(1, room.size.x);
            
            // Create two new sub-rooms
            BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
            BoundsInt room2 = new BoundsInt(
                new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
                new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
            
            // Add new rooms to the queue
            roomsQueue.Enqueue(room1);
            roomsQueue.Enqueue(room2);
        }
    }
}
