using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Item", menuName = "Loot/Loot Item")]
public class LootItem : ScriptableObject
{
    [Header("Loot Data")]
    public string itemName;
    public Sprite itemSprite;
    public LootType lootType;
    public int value;
    
    [Header("Drop Settings")]
    [Range(0f, 1f)]
    public float dropChance = 0.3f;
}

public enum LootType
{
    Coin,
    Health
}