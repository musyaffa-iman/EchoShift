using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum PlacementOriginCorner
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}

public class PropPlacementManager : MonoBehaviour
{
    private DungeonData dungeonData;
    private GameObject propsParent;
    private PropPlacer propPlacer;

    [SerializeField] private List<Prop> propsToPlace;

    [SerializeField, Range(0, 1)] private float cornerPropPlacementChance = 0.7f;

    [SerializeField] private GameObject propPrefab;

    [SerializeField] private GameObject lootDropPrefab;

    public UnityEvent OnFinished;

    private void Awake()
    {
        dungeonData = FindObjectOfType<DungeonData>();
        propPlacer = new PropPlacer();
    }

    public void ProcessRooms()
    {
        if (dungeonData == null)
            return;

        propsParent = GameObject.Find("Props");
        if (propsParent == null)
        {
            propsParent = new GameObject("Props");
        }

        var context = new PropPlacementContext
        {
            PropsParent = propsParent,
            PropPrefab = propPrefab,
            LootDropPrefab = lootDropPrefab,
            CornerPropPlacementChance = cornerPropPlacementChance
        };

        foreach (Room room in dungeonData.Rooms)
        {
            var strategies = PropPlacementStrategyFactory.CreateAllStrategies(propsToPlace, room, propPlacer);
            
            foreach (var (strategy, props) in strategies)
            {
                strategy.PlaceProps(room, props, dungeonData, context);
            }
        }
        
        TriggerFinishedEvent();
    }

    public void TriggerFinishedEvent()
    {
        OnFinished?.Invoke();
    }
}