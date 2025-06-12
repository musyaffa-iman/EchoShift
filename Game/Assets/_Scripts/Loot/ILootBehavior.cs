using UnityEngine;

public interface ILootBehavior
{
    void PlayCollectionSound();
    void ApplyLootEffect(GameObject collector, int value, LootDrop lootDropComponent);
    LootType GetLootType();
}