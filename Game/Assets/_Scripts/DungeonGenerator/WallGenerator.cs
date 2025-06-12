using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator 
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.Directions);
        var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.Diagonals);
        CreateWallsOfType(tilemapVisualizer, basicWallPositions, floorPositions, false);
        CreateWallsOfType(tilemapVisualizer, cornerWallPositions, floorPositions, true);
    }

    private static void CreateWallsOfType(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> wallPositions, HashSet<Vector2Int> floorPositions, bool isCorner)
    {
        foreach (var position in wallPositions)
        {
            string neighboursBinaryType = "";
            List<Vector2Int> directions = isCorner ? Direction2D.AllDirections : Direction2D.Directions;

            foreach (var direction in directions)
            {
                var neighbourPosition = position + direction;
                neighboursBinaryType += floorPositions.Contains(neighbourPosition) ? "1" : "0";
            }

            if (isCorner)
                tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType);
            else
                tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition) == false)
                    wallPositions.Add(neighbourPosition);
            }
        }
        return wallPositions;
    }
}
