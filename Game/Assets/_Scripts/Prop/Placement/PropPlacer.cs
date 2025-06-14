using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropPlacer
{
    public bool TryPlaceProp(Room room, Prop prop, List<Vector2Int> availablePositions, PlacementOriginCorner placement, DungeonData dungeonData, PropPlacementContext context)
    {
        for (int i = 0; i < availablePositions.Count; i++)
        {
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;

            List<Vector2Int> freeSpaces = CheckPropFit(prop, availablePositions, position, placement);

            if (freeSpaces.Count == prop.PropSize.x * prop.PropSize.y)
            {
                PlacePropGameObjectAt(room, position, prop, context);

                foreach (Vector2Int pos in freeSpaces)
                {
                    room.PropPositions.Add(pos);
                }

                if (prop.PlaceAsGroup)
                {
                    PlaceGroupObject(room, position, prop, 1, dungeonData, context);
                }
                return true;
            }
        }

        return false;
    }

    public void PlaceGroupObject(Room room, Vector2Int groupOriginPosition, Prop propToPlace, int searchOffset, DungeonData dungeonData, PropPlacementContext context)
    {
        int groupSize = UnityEngine.Random.Range(propToPlace.GroupMinCount, propToPlace.GroupMaxCount) - 1;
        groupSize = Mathf.Clamp(groupSize, 0, 8);

        List<Vector2Int> nearbyFreeTiles = new List<Vector2Int>();
        for (int xOffset = -searchOffset; xOffset <= searchOffset; xOffset++)
        {
            for (int yOffset = -searchOffset; yOffset <= searchOffset; yOffset++)
            {
                Vector2Int tilePosition = groupOriginPosition + new Vector2Int(xOffset, yOffset);
                if (room.FloorTiles.Contains(tilePosition) &&
                    !dungeonData.Path.Contains(tilePosition) &&
                    !room.PropPositions.Contains(tilePosition))
                {
                    nearbyFreeTiles.Add(tilePosition);
                }
            }
        }

        nearbyFreeTiles = nearbyFreeTiles.OrderBy(x => Guid.NewGuid()).ToList();

        int propsToPlace = Mathf.Min(groupSize, nearbyFreeTiles.Count);
        for (int i = 0; i < propsToPlace; i++)
        {
            PlacePropGameObjectAt(room, nearbyFreeTiles[i], propToPlace, context);
        }
    }

    public GameObject PlacePropGameObjectAt(Room room, Vector2Int placementPosition, Prop propToPlace, PropPlacementContext context)
    {
        GameObject prop = UnityEngine.Object.Instantiate(context.PropPrefab);

        if (context.PropsParent != null)
        {
            prop.transform.SetParent(context.PropsParent.transform);
        }

        SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();
        propSpriteRenderer.sprite = propToPlace.PropSprite;

        CapsuleCollider2D collider = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();
        collider.offset = Vector2.zero;

        if (propToPlace.PropSize.x > propToPlace.PropSize.y)
        {
            collider.direction = CapsuleDirection2D.Horizontal;
        }

        Vector2 colliderSize = new Vector2(propToPlace.PropSize.x * 0.8f, propToPlace.PropSize.y * 0.8f);
        collider.size = colliderSize;

        if (propToPlace.IsBreakable)
        {
            Destructible destructibleComponent = prop.AddComponent<Destructible>();
            destructibleComponent.InitializeDurability(propToPlace.Health);

            PropDestroyer propDestroyer = prop.AddComponent<PropDestroyer>();
            propDestroyer.SetPropData(propToPlace);
            propDestroyer.SetLootDropPrefab(context.LootDropPrefab);
        }

        prop.transform.localPosition = (Vector2)placementPosition;
        propSpriteRenderer.transform.localPosition = (Vector2)propToPlace.PropSize * 0.5f;

        room.PropPositions.Add(placementPosition);
        room.PropObjectReferences.Add(prop);

        return prop;
    }

    private List<Vector2Int> CheckPropFit(Prop prop, List<Vector2Int> availablePositions, Vector2Int originPosition, PlacementOriginCorner placement)
    {
        List<Vector2Int> freePositions = new();

        if (placement == PlacementOriginCorner.BottomLeft)
        {
            for (int xOffset = 0; xOffset < prop.PropSize.x; xOffset++)
            {
                for (int yOffset = 0; yOffset < prop.PropSize.y; yOffset++)
                {
                    Vector2Int tilePosition = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tilePosition))
                        freePositions.Add(tilePosition);
                }
            }
        }
        else if (placement == PlacementOriginCorner.BottomRight)
        {
            for (int xOffset = -prop.PropSize.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = 0; yOffset < prop.PropSize.y; yOffset++)
                {
                    Vector2Int tilePosition = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tilePosition))
                        freePositions.Add(tilePosition);
                }
            }
        }
        else if (placement == PlacementOriginCorner.TopLeft)
        {
            for (int xOffset = 0; xOffset < prop.PropSize.x; xOffset++)
            {
                for (int yOffset = -prop.PropSize.y + 1; yOffset <= 0; yOffset++)
                {
                    Vector2Int tilePosition = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tilePosition))
                        freePositions.Add(tilePosition);
                }
            }
        }
        else // TopRight
        {
            for (int xOffset = -prop.PropSize.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = -prop.PropSize.y + 1; yOffset <= 0; yOffset++)
                {
                    Vector2Int tilePosition = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tilePosition))
                        freePositions.Add(tilePosition);
                }
            }
        }

        return freePositions;
    }
}
