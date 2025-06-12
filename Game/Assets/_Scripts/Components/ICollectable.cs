using UnityEngine;

public interface ICollectable
{
    void Collect(GameObject collector);
    int GetValue();
    LootType GetLootType();
}