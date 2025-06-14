using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallPropPlacementStrategy : IPropPlacementStrategy
{
    private readonly PropPlacer propPlacer;
    private readonly HashSet<Vector2Int> availableTiles;
    private readonly PlacementOriginCorner placementCorner;

    public WallPropPlacementStrategy(PropPlacer propPlacer, HashSet<Vector2Int> availableTiles, PlacementOriginCorner placementCorner)
    {
        this.propPlacer = propPlacer;
        this.availableTiles = availableTiles;
        this.placementCorner = placementCorner;
    }

    public void PlaceProps(Room room, List<Prop> props, DungeonData dungeonData, PropPlacementContext context)
    {
        HashSet<Vector2Int> freeTiles = new HashSet<Vector2Int>(availableTiles);
        freeTiles.ExceptWith(dungeonData.Path);

        var sortedProps = props.OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();

        foreach (Prop prop in sortedProps)
        {
            int quantity = UnityEngine.Random.Range(prop.PlacementQuantityMin, prop.PlacementQuantityMax + 1);

            for (int i = 0; i < quantity; i++)
            {
                freeTiles.ExceptWith(room.PropPositions);
                List<Vector2Int> randomizedPositions = freeTiles.OrderBy(x => System.Guid.NewGuid()).ToList();

                if (!propPlacer.TryPlaceProp(room, prop, randomizedPositions, placementCorner, dungeonData, context))
                    break;
            }
        }
    }
}
