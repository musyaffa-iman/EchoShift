using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class RunData
{
    public string runId;
    public int score;
    public float timeElapsed;
    public int levelReached;
    public bool isActive;
    public bool isPaused;

    public RunData()
    {
        runId = "";
        score = 0;
        timeElapsed = 0f;
        levelReached = 1;
        isActive = false;
        isPaused = false;
    }
}

[Serializable]
public class RunResponse
{
    public string id;
    public string playerId;
    public float timeElapsed;
    public int score;
    public int levelReached;
}

[Serializable]
public class RunStartedEvent : UnityEvent<RunData> { }

[Serializable]
public class RunUpdatedEvent : UnityEvent<RunData> { }

[Serializable]
public class RunEndedEvent : UnityEvent<RunData> { }

[Serializable]
public class RunFailedEvent : UnityEvent<string> { }

[Serializable]
public class RunRequestData
{
    public int score;
    public float timeElapsed;
    public int levelReached;

    public RunRequestData(int score, float timeElapsed, int levelReached)
    {
        this.score = score;
        this.timeElapsed = timeElapsed;
        this.levelReached = levelReached;
    }
}

public class RunTracker : MonoBehaviour
{
    private static RunTracker _instance;
    public static RunTracker Instance => _instance;

    [Header("Run Data")]
    [SerializeField] private RunData currentRun = new RunData();
    
    [Header("Timing")]
    private float gameStartTime;
    private float totalPausedTime;
    private float lastPauseTime;
    
    [Header("Events")]
    public RunStartedEvent OnRunStarted = new RunStartedEvent();
    public RunUpdatedEvent OnRunUpdated = new RunUpdatedEvent();
    public RunEndedEvent OnRunEnded = new RunEndedEvent();
    public RunFailedEvent OnRunFailed = new RunFailedEvent();

    // Properties
    public bool IsRunActive => currentRun.isActive;
    public bool IsRunPaused => currentRun.isPaused;
    public RunData CurrentRun => currentRun;
    public float CurrentGameTime => CalculateCurrentGameTime();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Listen for pause events from Time.timeScale changes
        StartCoroutine(MonitorPauseState());
    }

    #region Public Methods

    public void StartRun(int initialLevel = 1)
    {
        if (currentRun.isActive)
        {
            Debug.LogWarning("[RunTracker] Run already active, ending previous run first");
            EndRun(false); // End without saving
        }

        // Initialize new run
        currentRun = new RunData();
        currentRun.levelReached = initialLevel;
        currentRun.isActive = true;
        currentRun.isPaused = false;
        
        // Reset timing
        gameStartTime = Time.time;
        totalPausedTime = 0f;
        lastPauseTime = 0f;

        if (PlayerAPI.Instance.IsLoggedIn)
        {
            StartCoroutine(StartRunCoroutine());
        }
        else
        {
            OnRunStarted?.Invoke(currentRun);
        }
    }

    public void UpdateScore(int newScore)
    {
        if (!currentRun.isActive)
        {
            Debug.LogWarning("[RunTracker] Cannot update score - no active run");
            return;
        }

        currentRun.score = newScore;

        // Only sync to backend if player is logged in and run has been created
        if (PlayerAPI.Instance.IsLoggedIn && !string.IsNullOrEmpty(currentRun.runId))
        {
            StartCoroutine(UpdateRunCoroutine());
        }
    }

    public void UpdateLevel(int newLevel)
    {
        if (!currentRun.isActive)
        {
            Debug.LogWarning("[RunTracker] Cannot update level - no active run");
            return;
        }

        if (newLevel > currentRun.levelReached)
        {
            currentRun.levelReached = newLevel;
            
            // Only sync to backend if player is logged in and run has been created
            if (PlayerAPI.Instance.IsLoggedIn && !string.IsNullOrEmpty(currentRun.runId))
            {
                StartCoroutine(UpdateRunCoroutine());
            }
        }
    }

    public void PauseRun()
    {
        if (!currentRun.isActive || currentRun.isPaused)
        {
            return;
        }

        currentRun.isPaused = true;
        lastPauseTime = Time.time;
    }

    public void ResumeRun()
    {
        if (!currentRun.isActive || !currentRun.isPaused)
        {
            return;
        }

        // Add the time we were paused to total paused time
        totalPausedTime += Time.time - lastPauseTime;
        currentRun.isPaused = false;
    }

    public void EndRun(bool saveToBackend = true)
    {
        if (!currentRun.isActive)
        {
            Debug.LogWarning("[RunTracker] No active run to end");
            return;
        }

        // Calculate final time
        currentRun.timeElapsed = CalculateCurrentGameTime();
        currentRun.isActive = false;
        currentRun.isPaused = false;

        if (saveToBackend && PlayerAPI.Instance.IsLoggedIn && !string.IsNullOrEmpty(currentRun.runId))
        {
            StartCoroutine(EndRunCoroutine());
        }
        else
        {
            OnRunEnded?.Invoke(currentRun);
        }
    }

    // Force saves the current run progress to backend
    public void SaveRunProgress()
    {
        if (!currentRun.isActive || !PlayerAPI.Instance.IsLoggedIn || string.IsNullOrEmpty(currentRun.runId))
        {
            return;
        }

        currentRun.timeElapsed = CalculateCurrentGameTime();
        StartCoroutine(UpdateRunCoroutine());
    }

    public int GetCurrentLevel()
    {
        return currentRun != null ? currentRun.levelReached : 1;
    }

    public int GetCurrentScore()
    {
        return currentRun != null ? currentRun.score : 0;
    }

    #endregion

    #region Private Methods

    private float CalculateCurrentGameTime()
    {
        if (!currentRun.isActive)
        {
            return currentRun.timeElapsed;
        }

        float totalTime = Time.time - gameStartTime;
        float pausedTime = totalPausedTime;
        
        if (currentRun.isPaused)
        {
            pausedTime += Time.time - lastPauseTime;
        }

        return Mathf.Max(0f, totalTime - pausedTime);
    }

    private IEnumerator MonitorPauseState()
    {
        bool wasPaused = false;
        
        while (true)
        {
            bool isPaused = Time.timeScale == 0f;
            
            if (isPaused && !wasPaused)
            {
                PauseRun();
            }
            else if (!isPaused && wasPaused)
            {
                ResumeRun();
            }
            
            wasPaused = isPaused;
            yield return new WaitForSecondsRealtime(0.1f); // Check every 0.1 seconds
        }
    }
    private IEnumerator StartRunCoroutine()
    {
        string url = APIConfig.RUNS_ENDPOINT;

        var requestData = new RunRequestData(
            currentRun.score,
            currentRun.timeElapsed,
            currentRun.levelReached
        );

        string runJson = JsonUtility.ToJson(requestData);

        yield return StartCoroutine(APIHelper.PostCoroutineWithAuth(url, runJson, OnRunStartCallback, PlayerAPI.Instance.SessionToken));
    }

    private void OnRunStartCallback(UnityEngine.Networking.UnityWebRequest request)
    {
        if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[RunTracker] Start run request failed: {request.error}");
            OnRunFailed?.Invoke($"Connection error: {request.error}");
            currentRun.isActive = false;
            return;
        }

        try
        {
            string resultJson = request.downloadHandler.text;
            BaseResponse<RunResponse> response = JsonUtility.FromJson<BaseResponse<RunResponse>>(resultJson);
            
            if (response.success && response.data != null)
            {
                currentRun.runId = response.data.id;
                OnRunStarted?.Invoke(currentRun);
            }
            else
            {
                Debug.LogError($"[RunTracker] Failed to start run: {response.message}");
                OnRunFailed?.Invoke(response.message);
                currentRun.isActive = false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[RunTracker] Failed to parse start run response: {e.Message}");
            OnRunFailed?.Invoke("Failed to process server response");
            currentRun.isActive = false;
        }
    }

    private IEnumerator UpdateRunCoroutine()
    {
        string url = $"{APIConfig.RUNS_ENDPOINT}/{currentRun.runId}";
        
        // Update current time before sending
        currentRun.timeElapsed = CalculateCurrentGameTime();
        
        // Create update request using proper serializable class
        var updateData = new RunRequestData(
            currentRun.score,
            currentRun.timeElapsed,
            currentRun.levelReached
        );
        
        string updateJson = JsonUtility.ToJson(updateData);
        
        yield return StartCoroutine(APIHelper.PutCoroutine(url, updateJson, OnRunUpdateCallback, PlayerAPI.Instance.SessionToken));
    }

    private void OnRunUpdateCallback(UnityEngine.Networking.UnityWebRequest request)
    {
        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            OnRunUpdated?.Invoke(currentRun);
        }
        else
        {
            Debug.LogWarning($"[RunTracker] Failed to save run progress: {request.error}");
        }
    }

    private IEnumerator EndRunCoroutine()
    {
        string url = $"{APIConfig.RUNS_ENDPOINT}/{currentRun.runId}/end";

        // Create end run request using proper serializable class
        var endRunData = new RunRequestData(
            currentRun.score,
            currentRun.timeElapsed,
            currentRun.levelReached
        );

        string endRunJson = JsonUtility.ToJson(endRunData);

        yield return StartCoroutine(APIHelper.PatchCoroutine(url, endRunJson, OnRunEndCallback, PlayerAPI.Instance.SessionToken));
    }

    private void OnRunEndCallback(UnityEngine.Networking.UnityWebRequest request)
    {
        if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[RunTracker] Failed to end run: {request.error}");
        }
        
        OnRunEnded?.Invoke(currentRun);
    }

    #endregion
}
