using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] public UnityEvent OnDungeonGenerated;

    [SerializeField] public TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;

    protected DungeonData dungeonData;

    private void Awake()
    {
        dungeonData = FindObjectOfType<DungeonData>();
        if (dungeonData == null)
            dungeonData = gameObject.AddComponent<DungeonData>();
    }

    private void Start()
    {
        GenerateDungeon();
    }
    public void GenerateDungeon()
    {
        if (dungeonData == null)
        {
            dungeonData = FindObjectOfType<DungeonData>();
            if (dungeonData == null)
                dungeonData = gameObject.AddComponent<DungeonData>();
        }

        dungeonData.Reset();
        tilemapVisualizer.Clear();
        RunProceduralGeneration();
        
        OnDungeonGenerated?.Invoke();
    }

    protected abstract void RunProceduralGeneration();

    protected void AnalyzeRoomStructure(Room room)
    {
        foreach (var tilePosition in room.FloorTiles)
        {
            int neighbours = 4;
            if (!room.FloorTiles.Contains(tilePosition + Vector2Int.up))
            {
                room.NearWallTilesUp.Add(tilePosition);
                neighbours--;
            }
            if (!room.FloorTiles.Contains(tilePosition + Vector2Int.down))
            {
                room.NearWallTilesDown.Add(tilePosition);
                neighbours--;
            }
            if (!room.FloorTiles.Contains(tilePosition + Vector2Int.right))
            {
                room.NearWallTilesRight.Add(tilePosition);
                neighbours--;
            }
            if (!room.FloorTiles.Contains(tilePosition + Vector2Int.left))
            {
                room.NearWallTilesLeft.Add(tilePosition);
                neighbours--;
            }

            if (neighbours <= 2)
                room.CornerTiles.Add(tilePosition);
            else if (neighbours == 4)
                room.InnerTiles.Add(tilePosition);
        }

        room.NearWallTilesUp.ExceptWith(room.CornerTiles);
        room.NearWallTilesDown.ExceptWith(room.CornerTiles);
        room.NearWallTilesLeft.ExceptWith(room.CornerTiles);
        room.NearWallTilesRight.ExceptWith(room.CornerTiles);
    }    protected void AddRoomAndAnalyze(Room room)
    {
        AnalyzeRoomStructure(room);
        dungeonData.Rooms.Add(room);
    }
}
