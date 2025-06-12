using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AbstractDungeonGenerator dungeonGenerator;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraHeight = -10f;
    
    [Header("Camera Movement")]
    [SerializeField] private float moveSpeed = 2f;
    
    private List<Vector2> pathPoints;
    private int currentPathIndex = 0;
    private Coroutine cameraMovementCoroutine;
    
    public void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.mainMenuMusic);
        
        // Generate dungeon for visual effect in main menu
        if (dungeonGenerator != null)
        {
            // Subscribe to the dungeon generation event
            dungeonGenerator.OnDungeonGenerated.AddListener(SetupCameraPath);
            dungeonGenerator.GenerateDungeon();
        }
    }
    
    private void SetupCameraPath()
    {
        if (dungeonGenerator != null)
        {
            DungeonData dungeonData = dungeonGenerator.GetComponent<DungeonData>();
            if (dungeonData != null)
            {
                // Create path points from rooms and path tiles
                pathPoints = new List<Vector2>();
                
                // Add room centers as main waypoints
                foreach (Room room in dungeonData.Rooms)
                {
                    pathPoints.Add(room.RoomCenterPos);
                }
                
                if (pathPoints.Count > 0)
                {
                    // Start camera at first point
                    if (mainCamera != null)
                    {
                        mainCamera.transform.position = new Vector3(pathPoints[0].x, pathPoints[0].y, cameraHeight);
                        StartCameraMovement();
                    }
                }
            }
        }
    }
    
    private void StartCameraMovement()
    {
        if (cameraMovementCoroutine != null)
        {
            StopCoroutine(cameraMovementCoroutine);
        }
        cameraMovementCoroutine = StartCoroutine(CameraPathCoroutine());
    }
    
    private IEnumerator CameraPathCoroutine()
    {
        while (pathPoints != null && pathPoints.Count > 1)
        {
            // Move to next point in path
            int nextIndex = (currentPathIndex + 1) % pathPoints.Count;
            Vector2 targetPoint = pathPoints[nextIndex];
            Vector3 startPos = mainCamera.transform.position;
            Vector3 targetPos = new Vector3(targetPoint.x, targetPoint.y, cameraHeight);
            
            // Calculate movement duration based on distance and speed
            float distance = Vector2.Distance(new Vector2(startPos.x, startPos.y), targetPoint);
            float moveDuration = distance / moveSpeed;
            
            // Smoothly move camera to target
            float timer = 0f;
            while (timer < moveDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / moveDuration;
                mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, progress);
                yield return null;
            }
            
            // Ensure camera reaches exact target position
            mainCamera.transform.position = targetPos;
            currentPathIndex = nextIndex;
        }
    }
    
    public void Play()
    {
        SceneManager.LoadSceneAsync("Level 1");
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    private void OnDestroy()
    {
        // Clean up event subscription and coroutine
        if (dungeonGenerator != null)
        {
            dungeonGenerator.OnDungeonGenerated.RemoveListener(SetupCameraPath);
        }
        
        if (cameraMovementCoroutine != null)
        {
            StopCoroutine(cameraMovementCoroutine);
        }
    }
}
