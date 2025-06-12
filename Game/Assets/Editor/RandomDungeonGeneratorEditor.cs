using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractDungeonGenerator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    AbstractDungeonGenerator generator;
    TilemapVisualizer tilemapVisualizer;

    private void Awake()
    {
        generator = (AbstractDungeonGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Dungeon"))
        {
            generator.GenerateDungeon();
        }
        if (GUILayout.Button("Clear Dungeon"))
        {
            generator.tilemapVisualizer.Clear();
        }
    }
}
