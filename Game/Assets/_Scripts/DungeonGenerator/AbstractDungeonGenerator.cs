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
        dungeonData.Reset();
        tilemapVisualizer.Clear();
        RunProceduralGeneration();
        
        OnDungeonGenerated?.Invoke();
    }

    protected abstract void RunProceduralGeneration();
}
