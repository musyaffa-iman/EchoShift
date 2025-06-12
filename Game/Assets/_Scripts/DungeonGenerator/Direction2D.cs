using System.Collections.Generic;
using UnityEngine;

public static class Direction2D
{
    public static List<Vector2Int> Directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1),   //Up
        new Vector2Int(1, 0),   //Right
        new Vector2Int(0, -1),  //Down
        new Vector2Int(-1, 0)   //Left
    };

    public static List<Vector2Int> Diagonals = new List<Vector2Int>
    {
        new Vector2Int(1, 1),   //Up-Right
        new Vector2Int(1, -1),  //Right-Down
        new Vector2Int(-1, -1), //Down-Left
        new Vector2Int(-1, 1)   //Left-Up
    };

    public static List<Vector2Int> AllDirections = new List<Vector2Int>
    {
        new Vector2Int(0, 1),   //Up
        new Vector2Int(1, 1),   //Up-Right    
        new Vector2Int(1, 0),   //Right
        new Vector2Int(1, -1),  //Right-Down
        new Vector2Int(0, -1),  //Down
        new Vector2Int(-1, -1), //Down-Left
        new Vector2Int(-1, 0),  //Left
        new Vector2Int(-1, 1)   //Left-Up
    };

    public static Vector2Int GetRandomDirection()
    {
        return Directions[Random.Range(0, Directions.Count)];
    }
}
