using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTilemap;
    [SerializeField] private RuleTile floorRuleTile;
    [SerializeField] private TileBase wallTop, wallRight, wallLeft, wallBottom, wallFull, 
                    wallInnerCornerDownRight, wallInnerCornerDownLeft, 
                    wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    private void Awake()
    {
        wallTilemap.gameObject.layer = LayerMask.NameToLayer("Walls");
        
        if (!wallTilemap.TryGetComponent<TilemapCollider2D>(out var wallCollider))
        {
            wallCollider = wallTilemap.gameObject.AddComponent<TilemapCollider2D>();
        }
        wallCollider.usedByComposite = true;

        if (!wallTilemap.TryGetComponent<Rigidbody2D>(out var wallRigidbody))
        {
            wallRigidbody = wallTilemap.gameObject.AddComponent<Rigidbody2D>();
        }
        wallRigidbody.bodyType = RigidbodyType2D.Static;

        if (!wallTilemap.TryGetComponent<CompositeCollider2D>(out var compositeCollider))
        {
            compositeCollider = wallTilemap.gameObject.AddComponent<CompositeCollider2D>();
        }
    }

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorRuleTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
        tilemap.RefreshAllTiles();
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypes.wallTop.Contains(typeInt))
            tile = wallTop;
        else if (WallTypes.wallRight.Contains(typeInt))
            tile = wallRight;
        else if (WallTypes.wallLeft.Contains(typeInt))
            tile = wallLeft;
        else if (WallTypes.wallBottom.Contains(typeInt))
            tile = wallBottom;
        else if (WallTypes.wallFull.Contains(typeInt))
            tile = wallFull;

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypes.wallInnerCornerDownLeft.Contains(typeInt))
            tile = wallInnerCornerDownLeft;
        else if (WallTypes.wallInnerCornerDownRight.Contains(typeInt))
            tile = wallInnerCornerDownRight;
        else if (WallTypes.wallDiagonalCornerDownLeft.Contains(typeInt))
            tile = wallDiagonalCornerDownLeft;
        else if (WallTypes.wallDiagonalCornerDownRight.Contains(typeInt))
            tile = wallDiagonalCornerDownRight;
        else if (WallTypes.wallDiagonalCornerUpLeft.Contains(typeInt))
            tile = wallDiagonalCornerUpLeft;
        else if (WallTypes.wallDiagonalCornerUpRight.Contains(typeInt))
            tile = wallDiagonalCornerUpRight;
        else if (WallTypes.wallFullEightDirections.Contains(typeInt))
            tile = wallFull;
        else if (WallTypes.wallBottomEightDirections.Contains(typeInt))
            tile = wallBottom;

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
    }
}
