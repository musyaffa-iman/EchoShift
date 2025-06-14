using System.Collections.Generic;
using UnityEngine;

public interface IPropPlacementStrategy
{
    void PlaceProps(Room room, List<Prop> props, DungeonData dungeonData, PropPlacementContext context);
}

public class PropPlacementContext
{
    public GameObject PropsParent { get; set; }
    public GameObject PropPrefab { get; set; }
    public GameObject LootDropPrefab { get; set; }
    public float CornerPropPlacementChance { get; set; }
}
