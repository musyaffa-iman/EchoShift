using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject PauseButton;
    AudioManager audioManager;

    // private void Awake()
    // {
    //     audioManager = AudioManager.Instance;
    //     if (audioManager == null)
    //     {
    //         Debug.LogError("[PauseMenu] AudioManager not found in the scene!");
    //     }
    //     else
    //     {
    //         Debug.Log("[PauseMenu] AudioManager found: " + audioManager.name);
    //     }
    // }

    public void Pause()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
        PauseButton.SetActive(false);
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
        PauseButton.SetActive(true);
        PausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
        if (RunTracker.Instance != null && RunTracker.Instance.IsRunActive)
        {
            RunTracker.Instance.EndRun(false);
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }

        if (SceneController.Instance != null)
        {
            Destroy(SceneController.Instance.gameObject);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}
