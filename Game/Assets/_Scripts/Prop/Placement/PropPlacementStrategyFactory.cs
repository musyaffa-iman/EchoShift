using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PropPlacementStrategyFactory
{
    public static IPropPlacementStrategy CreateCornerStrategy(PropPlacer propPlacer)
    {
        return new CornerPropPlacementStrategy(propPlacer);
    }

    public static IPropPlacementStrategy CreateTileBasedStrategy(PropPlacer propPlacer, HashSet<Vector2Int> availableTiles, PlacementOriginCorner corner)
    {
        return new TileBasedPropPlacementStrategy(propPlacer, availableTiles, corner);
    }

    public static List<(IPropPlacementStrategy strategy, List<Prop> props)> CreateAllStrategies(
        List<Prop> allProps, Room room, PropPlacer propPlacer)
    {
        var strategies = new List<(IPropPlacementStrategy, List<Prop>)>();

        // Corner props
        var cornerProps = allProps.Where(x => x.Corner).ToList();
        if (cornerProps.Any())
        {
            strategies.Add((CreateCornerStrategy(propPlacer), cornerProps));
        }

        // Wall props
        if (room.NearWallTilesLeft.Any())
        {
            var leftWallProps = allProps.Where(x => x.NearWallLeft).ToList();
            if (leftWallProps.Any())
                strategies.Add((CreateTileBasedStrategy(propPlacer, room.NearWallTilesLeft, PlacementOriginCorner.BottomLeft), leftWallProps));
        }

        if (room.NearWallTilesRight.Any())
        {
            var rightWallProps = allProps.Where(x => x.NearWallRight).ToList();
            if (rightWallProps.Any())
                strategies.Add((CreateTileBasedStrategy(propPlacer, room.NearWallTilesRight, PlacementOriginCorner.TopRight), rightWallProps));
        }

        if (room.NearWallTilesUp.Any())
        {
            var topWallProps = allProps.Where(x => x.NearWallUP).ToList();
            if (topWallProps.Any())
                strategies.Add((CreateTileBasedStrategy(propPlacer, room.NearWallTilesUp, PlacementOriginCorner.TopLeft), topWallProps));
        }

        if (room.NearWallTilesDown.Any())
        {
            var downWallProps = allProps.Where(x => x.NearWallDown).ToList();
            if (downWallProps.Any())
                strategies.Add((CreateTileBasedStrategy(propPlacer, room.NearWallTilesDown, PlacementOriginCorner.BottomLeft), downWallProps));
        }

        // Inner props - uses the same tile-based strategy
        if (room.InnerTiles.Any())
        {
            var innerProps = allProps.Where(x => x.Inner).ToList();
            if (innerProps.Any())
                strategies.Add((CreateTileBasedStrategy(propPlacer, room.InnerTiles, PlacementOriginCorner.BottomLeft), innerProps));
        }

        return strategies;
    }
}
