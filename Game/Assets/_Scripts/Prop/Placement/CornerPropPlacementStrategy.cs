using System.Collections.Generic;
using UnityEngine;

public class CornerPropPlacementStrategy : IPropPlacementStrategy
{
    private readonly PropPlacer propPlacer;

    public CornerPropPlacementStrategy(PropPlacer propPlacer)
    {
        this.propPlacer = propPlacer;
    }

    public void PlaceProps(Room room, List<Prop> props, DungeonData dungeonData, PropPlacementContext context)
    {
        float chanceFactor = context.CornerPropPlacementChance;

        foreach (Vector2Int cornerTile in room.CornerTiles)
        {
            if (dungeonData.Path.Contains(cornerTile))
                continue;

            if (UnityEngine.Random.value < chanceFactor)
            {
                Prop propToPlace = props[UnityEngine.Random.Range(0, props.Count)];
                propPlacer.PlacePropGameObjectAt(room, cornerTile, propToPlace, context);
                
                if (propToPlace.PlaceAsGroup)
                {
                    propPlacer.PlaceGroupObject(room, cornerTile, propToPlace, 2, dungeonData, context);
                }
            }
            else
            {
                chanceFactor = Mathf.Clamp01(chanceFactor + 0.1f);
            }
        }
    }
}
