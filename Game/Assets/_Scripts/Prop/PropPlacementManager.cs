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

    [SerializeField]
    private List<Prop> propsToPlace;

    [SerializeField, Range(0, 1)]
    private float cornerPropPlacementChance = 0.7f;

    [SerializeField]
    private GameObject propPrefab;

    [SerializeField] private GameObject lootDropPrefab;

    public UnityEvent OnFinished;

    private void Awake()
    {
        dungeonData = FindObjectOfType<DungeonData>();
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

        foreach (Room room in dungeonData.Rooms)
        {
            //Place props in the corners
            List<Prop> cornerProps = propsToPlace.Where(x => x.Corner).ToList();
            PlaceCornerProps(room, cornerProps);

            //Place props near LEFT wall
            List<Prop> leftWallProps = propsToPlace
            .Where(x => x.NearWallLeft)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();

            PlaceProps(room, leftWallProps, room.NearWallTilesLeft, PlacementOriginCorner.BottomLeft);

            //Place props near RIGHT wall
            List<Prop> rightWallProps = propsToPlace
            .Where(x => x.NearWallRight)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();

            PlaceProps(room, rightWallProps, room.NearWallTilesRight, PlacementOriginCorner.TopRight);

            //Place props near UP wall
            List<Prop> topWallProps = propsToPlace
            .Where(x => x.NearWallUP)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();

            PlaceProps(room, topWallProps, room.NearWallTilesUp, PlacementOriginCorner.TopLeft);

            //Place props near DOWN wall
            List<Prop> downWallProps = propsToPlace
            .Where(x => x.NearWallDown)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();

            PlaceProps(room, downWallProps, room.NearWallTilesDown, PlacementOriginCorner.BottomLeft);

            //Place inner props
            List<Prop> innerProps = propsToPlace
                .Where(x => x.Inner)
                .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
                .ToList();
            PlaceProps(room, innerProps, room.InnerTiles, PlacementOriginCorner.BottomLeft);
        }
        TriggerFinishedEvent();
    }

    public void TriggerFinishedEvent()
    {
        OnFinished?.Invoke();
    }

    private void PlaceProps(
        Room room, List<Prop> propsToPlace, HashSet<Vector2Int> availableTiles, PlacementOriginCorner placement)
    {
        //Remove path positions to ensure clear path to traverse dungeon
        HashSet<Vector2Int> freeTiles = new HashSet<Vector2Int>(availableTiles);
        freeTiles.ExceptWith(dungeonData.Path);

        //Try to place all the props
        foreach (Prop prop in propsToPlace)
        {
            //Generate random quantity for this prop type
            int quantity = UnityEngine.Random.Range(prop.PlacementQuantityMin, prop.PlacementQuantityMax + 1);

            for (int i = 0; i < quantity; i++)
            {
                //Remove already occupied positions
                freeTiles.ExceptWith(room.PropPositions);

                //Randomize positions for variety
                List<Vector2Int> randomizedPositions = freeTiles.OrderBy(x => Guid.NewGuid()).ToList();

                //If placement failed, don't try again with this prop type
                if (!TryPlacePropAtAvailablePosition(room, prop, randomizedPositions, placement))
                    break;
            }
        }
    }

    private bool TryPlacePropAtAvailablePosition(
        Room room, Prop prop, List<Vector2Int> availablePositions, PlacementOriginCorner placement)
    {
        //Try placing from corner specified by placement parameter
        for (int i = 0; i < availablePositions.Count; i++)
        {
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;

            //Check if there's enough space for this prop
            List<Vector2Int> freeSpaces = CheckPropFit(prop, availablePositions, position, placement);

            //If we have enough spaces for the prop, place it
            if (freeSpaces.Count == prop.PropSize.x * prop.PropSize.y)
            {
                //Place the prop GameObject
                PlacePropGameObjectAt(room, position, prop);

                //Mark all required positions as occupied
                foreach (Vector2Int pos in freeSpaces)
                {
                    room.PropPositions.Add(pos);
                }

                //Handle group placement if needed
                if (prop.PlaceAsGroup)
                {
                    PlaceGroupObject(room, position, prop, 1);
                }
                return true;
            }
        }

        return false;
    }

    private List<Vector2Int> CheckPropFit(
        Prop prop,
        List<Vector2Int> availablePositions,
        Vector2Int originPosition,
        PlacementOriginCorner placement)
    {
        List<Vector2Int> freePositions = new();

        // Check positions based on the placement corner
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
        // TopRight case
        else
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

    private void PlaceCornerProps(Room room, List<Prop> cornerProps)
    {
        float chanceFactor = cornerPropPlacementChance;

        foreach (Vector2Int cornerTile in room.CornerTiles)
        {
            if (dungeonData.Path.Contains(cornerTile))
                continue;

            if (UnityEngine.Random.value < chanceFactor)
            {
                Prop propToPlace = cornerProps[UnityEngine.Random.Range(0, cornerProps.Count)];

                PlacePropGameObjectAt(room, cornerTile, propToPlace);
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, cornerTile, propToPlace, 2);
                }
            }
            else
            {
                // Increase chance for next corner
                chanceFactor = Mathf.Clamp01(chanceFactor + 0.1f);
            }
        }
    }

    private void PlaceGroupObject(
        Room room, Vector2Int groupOriginPosition, Prop propToPlace, int searchOffset)
    {

        // Calculate how many props to place in the group (minus the central one already placed)
        int groupSize = UnityEngine.Random.Range(propToPlace.GroupMinCount, propToPlace.GroupMaxCount) - 1;
        groupSize = Mathf.Clamp(groupSize, 0, 8);

        // Find available spaces around the center point within searchOffset
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

        // Randomize the positions
        nearbyFreeTiles = nearbyFreeTiles.OrderBy(x => Guid.NewGuid()).ToList();

        // Place props (as many as requested or as many as space allows)
        int propsToPlace = Mathf.Min(groupSize, nearbyFreeTiles.Count);
        for (int i = 0; i < propsToPlace; i++)
        {
            PlacePropGameObjectAt(room, nearbyFreeTiles[i], propToPlace);
        }
    }

    private GameObject PlacePropGameObjectAt(Room room, Vector2Int placementPosition, Prop propToPlace)
    {
        // Instantiate the prop
        GameObject prop = Instantiate(propPrefab);

        if (propsParent != null)
        {
            prop.transform.SetParent(propsParent.transform);
        }

        SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();

        // Set the sprite
        propSpriteRenderer.sprite = propToPlace.PropSprite;

        // Add and configure collider
        CapsuleCollider2D collider = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();
        collider.offset = Vector2.zero;

        // Set collider direction based on prop dimensions
        if (propToPlace.PropSize.x > propToPlace.PropSize.y)
        {
            collider.direction = CapsuleDirection2D.Horizontal;
        }

        // Set collider size (slightly smaller than the prop)
        Vector2 colliderSize = new Vector2(propToPlace.PropSize.x * 0.8f, propToPlace.PropSize.y * 0.8f);
        collider.size = colliderSize;

        // Add Health component if prop is breakable
        if (propToPlace.IsBreakable)
        {
            // Add Destructible
            Destructible destructibleComponent = prop.AddComponent<Destructible>();
            destructibleComponent.InitializeDurability(propToPlace.Health);

            // Add PropDestroyer
            PropDestroyer propDestroyer = prop.AddComponent<PropDestroyer>();
            propDestroyer.SetPropData(propToPlace);
            propDestroyer.SetLootDropPrefab(lootDropPrefab); // Don't forget this!
        }

        // Position the prop
        prop.transform.localPosition = (Vector2)placementPosition;

        // Adjust the sprite position
        propSpriteRenderer.transform.localPosition = (Vector2)propToPlace.PropSize * 0.5f;

        // Register prop in room data
        room.PropPositions.Add(placementPosition);
        room.PropObjectReferences.Add(prop);

        return prop;
    }
}