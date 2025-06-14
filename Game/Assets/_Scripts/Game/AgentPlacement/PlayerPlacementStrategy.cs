using UnityEngine;
using Cinemachine;

public class PlayerPlacementStrategy : IPlacementStrategy
{
    private readonly GameObject playerPrefab;

    public PlayerPlacementStrategy(GameObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
    }

    public void PlaceAgent(Room room, DungeonData dungeonData, int roomIndex = 0)
    {
        GameObject player = GetOrCreatePlayer();
        player.transform.localPosition = room.RoomCenterPos + Vector2.one * 0.5f;
        dungeonData.PlayerReference = player;

        SetupCameraFollow(player);
    }

    private GameObject GetOrCreatePlayer()
    {
        if (Player.Instance != null)
            return Player.Instance.gameObject;
        
        return Object.Instantiate(playerPrefab);
    }

    private void SetupCameraFollow(GameObject player)
    {
        var vcam = Object.FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = player.transform;
        }
    }
}
