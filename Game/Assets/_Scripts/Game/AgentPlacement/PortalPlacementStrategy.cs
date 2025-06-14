using System.Linq;
using UnityEngine;

public class PortalPlacementStrategy : IPlacementStrategy
{
    private readonly GameObject portalPrefab;

    public PortalPlacementStrategy(GameObject portalPrefab)
    {
        this.portalPrefab = portalPrefab;
    }

    public void PlaceAgent(Room room, DungeonData dungeonData, int roomIndex = 0)
    {
        if (portalPrefab == null) return;

        Vector2Int portalPosition = DeterminePortalPosition(room, dungeonData);
        if (portalPosition == Vector2Int.zero)
        {
            Debug.LogWarning("No suitable position found for portal!");
            return;
        }

        CreatePortal(room, portalPosition);
    }

    private Vector2Int DeterminePortalPosition(Room room, DungeonData dungeonData)
    {
        if (dungeonData.Rooms.Count == 1)
        {
            return FindFurthestPositionFromCenter(room);
        }

        Vector2Int centerPos = new Vector2Int((int)room.RoomCenterPos.x, (int)room.RoomCenterPos.y);
        if (room.FloorTiles.Contains(centerPos) && 
            !room.PropPositions.Contains(centerPos) &&
            room.PositionsAccessibleFromPath.Contains(centerPos))
        {
            return centerPos;
        }

        return room.PositionsAccessibleFromPath
            .Where(pos => !room.PropPositions.Contains(pos))
            .FirstOrDefault();
    }

    private Vector2Int FindFurthestPositionFromCenter(Room room)
    {
        Vector2Int centerPos = new Vector2Int((int)room.RoomCenterPos.x, (int)room.RoomCenterPos.y);
        
        var availablePositions = room.PositionsAccessibleFromPath
            .Where(pos => !room.PropPositions.Contains(pos))
            .ToList();

        if (availablePositions.Count == 0)
            return Vector2Int.zero;

        Vector2Int furthestPosition = availablePositions[0];
        float maxDistance = Vector2Int.Distance(centerPos, furthestPosition);

        foreach (Vector2Int pos in availablePositions)
        {
            float distance = Vector2Int.Distance(centerPos, pos);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestPosition = pos;
            }
        }

        return furthestPosition;
    }

    private void CreatePortal(Room room, Vector2Int position)
    {
        GameObject portal = Object.Instantiate(portalPrefab);
        portal.transform.localPosition = (Vector2)position + Vector2.one * 0.5f;
        room.PropPositions.Add(position);
    }
}
