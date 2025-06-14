using System.Collections.Generic;
using UnityEngine;

public class EnemyPlacementStrategy : IPlacementStrategy
{
    private readonly List<GameObject> enemyPrefabs;
    private readonly List<int> roomEnemiesCount;
    private readonly Transform enemiesParent;

    public EnemyPlacementStrategy(List<GameObject> enemyPrefabs, List<int> roomEnemiesCount, Transform enemiesParent)
    {
        this.enemyPrefabs = enemyPrefabs;
        this.roomEnemiesCount = roomEnemiesCount;
        this.enemiesParent = enemiesParent;
    }

    public void PlaceAgent(Room room, DungeonData dungeonData, int roomIndex = 0)
    {
        if (roomEnemiesCount.Count <= roomIndex)
            return;

        int enemyCount = roomEnemiesCount[roomIndex];
        PlaceEnemies(room, enemyCount);
    }

    private void PlaceEnemies(Room room, int enemyCount)
    {
        for (int k = 0; k < enemyCount; k++)
        {
            if (room.PositionsAccessibleFromPath.Count <= k)
                return;

            GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject enemy = Object.Instantiate(prefabToSpawn);
            enemy.transform.localPosition = (Vector2)room.PositionsAccessibleFromPath[k] + Vector2.one * 0.5f;
            
            if (enemiesParent != null)
            {
                enemy.transform.SetParent(enemiesParent);
            }
            
            room.EnemiesInTheRoom.Add(enemy);
        }
    }
}
