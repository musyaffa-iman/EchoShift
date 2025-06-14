using UnityEngine;

public interface IPlacementStrategy
{
    void PlaceAgent(Room room, DungeonData dungeonData, int roomIndex = 0);
}
