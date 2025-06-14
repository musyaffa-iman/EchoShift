using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System;

public class RunsManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform runListParent;
    public GameObject runItemPrefab;

    private string baseUrl = APIConfig.API_BASE;
    
    private string currentPlayerId;

    void Start()
    {
        // Subscribe to PlayerAPI events
        if (PlayerAPI.Instance != null && Application.isPlaying)
        {
            PlayerAPI.Instance.OnPlayerLoggedIn.AddListener(OnPlayerLoggedIn);
            PlayerAPI.Instance.OnPlayerRegistered.AddListener(OnPlayerRegistered);
            PlayerAPI.Instance.OnPlayerLoggedOut.AddListener(OnPlayerLoggedOut);
            PlayerAPI.Instance.OnSessionValidated.AddListener(OnSessionValidated);
        }
        
        // Check if player is already logged in (for immediate UI updates)
        CheckExistingSession();
    }

    private void CheckExistingSession()
    {
        if (PlayerAPI.Instance != null && PlayerAPI.Instance.IsLoggedIn && PlayerAPI.Instance.CurrentPlayer != null)
        {
            string playerId = PlayerAPI.Instance.CurrentPlayer.id;
            currentPlayerId = playerId;
            
            // Only load runs if this panel is currently active
            if (gameObject.activeInHierarchy)
            {
                LoadPlayerRuns();
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (PlayerAPI.Instance != null && Application.isPlaying)
        {
            PlayerAPI.Instance.OnPlayerLoggedIn.RemoveListener(OnPlayerLoggedIn);
            PlayerAPI.Instance.OnPlayerRegistered.RemoveListener(OnPlayerRegistered);
            PlayerAPI.Instance.OnPlayerLoggedOut.RemoveListener(OnPlayerLoggedOut);
            PlayerAPI.Instance.OnSessionValidated.RemoveListener(OnSessionValidated);
        }
    }

    void OnEnable()
    {
        // Simply check current state and load if player is available
        if (PlayerAPI.Instance != null && Application.isPlaying)
        {
            if (PlayerAPI.Instance.IsLoggedIn && PlayerAPI.Instance.CurrentPlayer != null)
            {
                string newPlayerId = PlayerAPI.Instance.CurrentPlayer.id;
                
                if (newPlayerId != currentPlayerId)
                {
                    currentPlayerId = newPlayerId;
                }
                
                if (!string.IsNullOrEmpty(currentPlayerId))
                {
                    LoadPlayerRuns();
                }
            }
        }
    }

    // Event handlers for PlayerAPI events
    private void OnPlayerLoggedIn(PlayerData playerData)
    {
        currentPlayerId = playerData.id;
        
        // Only load runs if this panel is currently active
        if (gameObject.activeInHierarchy)
        {
            LoadPlayerRuns();
        }
    }

    private void OnPlayerRegistered(PlayerData playerData)
    {
        currentPlayerId = playerData.id;
        
        // Only load runs if this panel is currently active
        if (gameObject.activeInHierarchy)
        {
            LoadPlayerRuns();
        }
    }

    private void OnPlayerLoggedOut()
    {
        currentPlayerId = null;
        ClearRunList(); // Clear the UI when player logs out
    }

    private void OnSessionValidated(PlayerData playerData)
    {
        currentPlayerId = playerData.id;
        
        // Only load runs if this panel is currently active
        if (gameObject.activeInHierarchy)
        {
            LoadPlayerRuns();
        }
    }

    public void LoadPlayerRuns()
    {
        StartCoroutine(FetchPlayerRuns());
    }

    private IEnumerator FetchPlayerRuns()
    {
        string url = $"{baseUrl}/runs/{currentPlayerId}";
        
        yield return StartCoroutine(APIHelper.GetCoroutine(url, (request) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    RunListResponse response = JsonUtility.FromJson<RunListResponse>(request.downloadHandler.text);
                    
                    if (response.success && response.data != null)
                    {
                        Debug.Log($"[RunsManager] Found {response.data.Count} runs");
                        DisplayRuns(response.data);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[RunsManager] Error parsing run data: {e.Message}");
                    Debug.LogError($"[RunsManager] Stack trace: {e.StackTrace}");
                }
            }
            else
            {
                Debug.LogError($"[RunsManager] API Error: {request.error}");
                Debug.LogError($"[RunsManager] Response Text: {request.downloadHandler?.text}");
            }
        }));
    }

    private void DisplayRuns(List<RunResponse> runs)
    {
        ClearRunList();
        
        // Create UI item for each run
        foreach (RunResponse run in runs)
        {
            CreateRunItem(run);
        }
    }

    private void CreateRunItem(RunResponse run)
    {
        if (runItemPrefab == null)
        {
            Debug.LogError("[RunsManager] Run item prefab is null! Please assign a prefab in the inspector.");
            return;
        }
        
        if (runListParent == null)
        {
            Debug.LogError("[RunsManager] Run list parent is null! Please assign the Content object from Runs/Scroll Area/Viewport/Content.");
            return;
        }
        
        GameObject runItem = Instantiate(runItemPrefab, runListParent);
        
        // Find UI components
        Text scoreText = runItem.transform.Find("ScoreText")?.GetComponent<Text>();
        Text timeText = runItem.transform.Find("TimeText")?.GetComponent<Text>();
        Text levelText = runItem.transform.Find("LevelText")?.GetComponent<Text>();
        Button deleteButton = runItem.transform.Find("DeleteButton")?.GetComponent<Button>();
        
        // Set the data
        if (scoreText != null) scoreText.text = run.score.ToString();
        if (timeText != null) timeText.text = run.timeElapsed.ToString("F1") + "s";
        if (levelText != null) levelText.text = run.levelReached.ToString();
        
        // Setup delete button
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(() => DeleteRun(run.id, runItem));
        }
        else
        {
            Debug.LogWarning("[RunsManager] DeleteButton not found in run item prefab");
        }
    }

    private void ClearRunList()
    {
        if (runListParent == null)
        {
            Debug.LogWarning("[RunsManager] Cannot clear run list - parent is null");
            return;
        }
        
        foreach (Transform child in runListParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetPlayerId(string playerId)
    {
        currentPlayerId = playerId;
        LoadPlayerRuns();
    }

    private void DeleteRun(string runId, GameObject runItem)
    {
        StartCoroutine(DeleteRunCoroutine(runId, runItem));
    }

    private IEnumerator DeleteRunCoroutine(string runId, GameObject runItem)
    {
        string deleteUrl = $"{baseUrl}/runs/{runId}";
        
        yield return StartCoroutine(APIHelper.DeleteCoroutine(deleteUrl, (request) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                if (runItem != null)
                {
                    Destroy(runItem);
                }
            }
            else
            {
                Debug.LogError($"[RunsManager] Failed to delete run {runId}: {request.error}");
            }
        }));
    }
}
