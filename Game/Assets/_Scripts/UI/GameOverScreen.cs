using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI finalScoreText;

    public void Setup(int finalScore = 0)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore.ToString();
        }
        
        gameObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync("Level 1");
    }

    public void Quit()
    {
        // Abandon the current run before quitting
        if (RunTracker.Instance != null && RunTracker.Instance.IsRunActive)
        {
            RunTracker.Instance.EndRun(false); // Don't save abandoned runs
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
            Debug.Log("[PauseMenu] Player destroyed");
        }

        if (SceneController.Instance != null)
        {
            Destroy(SceneController.Instance.gameObject);
            Debug.Log("[PauseMenu] SceneController destroyed");
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}