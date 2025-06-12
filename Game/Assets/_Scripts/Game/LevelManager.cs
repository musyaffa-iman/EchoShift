using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Game State")]
    public int currentScore = 0;
    public int currentLevel = 1;
    
    [Header("Level Settings")]
    public bool IsBossLevel = false;
    [Range(0f, 1f)]
    public float bossVolumeMultiplier = 0.4f;

    [Header("Run Tracker")]
    public RunTracker runTracker;

    [Header("UI")]
    public HUD hud;
    public GameOverScreen GameOverScreen;

    void Awake()
    {
        StartCoroutine(ResetReferences());
    }

    void Start()
    {
        StartCoroutine(InitializeWithRunTracker());
        PlayLevelMusic();
    }
    
    private void PlayLevelMusic()
    {
        if (AudioManager.Instance != null)
        {
            if (IsBossLevel)
            {
                AudioManager.Instance.PlayMusic(AudioManager.Instance.bossMusic, bossVolumeMultiplier);
            }
            else
            {
                AudioManager.Instance.PlayMusic(AudioManager.Instance.backgroundMusic);
            }
        }
        else
        {
            Debug.LogWarning("[LevelManager] AudioManager instance not found!");
        }
    }

    private IEnumerator InitializeWithRunTracker()
    {
        while (runTracker == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (runTracker.IsRunActive)
        {
            currentLevel = runTracker.GetCurrentLevel();
            currentScore = runTracker.GetCurrentScore();
        }
        else
        {
            runTracker.StartRun(currentLevel);
        }

        hud = FindObjectOfType<HUD>();
        if (hud == null)
        {
            Debug.LogError("[LevelManager] HUD not found in the scene!");
        }
        else
        {
            hud.UpdateScore(currentScore);
        }
    }

    private IEnumerator ResetReferences()
    {
        while (GameOverScreen == null)
        {
            GameOverScreen = FindObjectOfType<GameOverScreen>();
            if (GameOverScreen == null)
            {
                GameOverScreen[] screens = Resources.FindObjectsOfTypeAll<GameOverScreen>();
                foreach (GameOverScreen screen in screens)
                {
                    if (screen.gameObject.scene.IsValid())
                    {
                        GameOverScreen = screen;
                        break;
                    }
                }
            }
            
            if (GameOverScreen == null)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Ensure GameOverScreen starts inactive
        if (GameOverScreen != null)
        {
            GameOverScreen.gameObject.SetActive(false);
        }

        while (runTracker == null)
        {
            runTracker = FindObjectOfType<RunTracker>();
            if (runTracker == null)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;

        if (hud != null)
        {
            hud.UpdateScore(currentScore);
        }

        if (runTracker != null && runTracker.IsRunActive)
        {
            runTracker.UpdateScore(currentScore);
        }
        else
        {
            Debug.LogWarning("[LevelManager] Cannot update score - no active run");
        }
    }

    public void AdvanceLevel()
    {
        currentLevel++;
        AddScore(150);

        if (runTracker != null && runTracker.IsRunActive)
        {
            runTracker.UpdateLevel(currentLevel);
        }
        else
        {
            Debug.LogWarning("[LevelManager] Cannot update level - no active run");
        }
        
        // Play appropriate music for new level
        PlayLevelMusic();
    }

    public void EndGame()
    {
        if (runTracker != null && runTracker.IsRunActive)
        {
            runTracker.EndRun(true);
        }
        else
        {
            Debug.LogWarning("[LevelManager] Cannot end game - no active run");
        }
    }

    public void OnPlayerDeath()
    {
        if (GameOverScreen != null)
        {
            GameOverScreen.Setup(currentScore);
        }
        else
        {
            Debug.LogError("[LevelManager] Cannot show game over screen - GameOverScreen not found in scene!");
        }
        
        EndGame();
    }
}