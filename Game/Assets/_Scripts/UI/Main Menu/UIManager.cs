using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI statusText;
    public Button loginButton;
    public Button registerButton;
    public Button logoutButton;
    public Button accountButton;
    public Button runsButton;
    
    [Header("Input Fields")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    
    [Header("Panel References")]
    public GameObject mainMenuPanel;
    public GameObject loginRegisterPanel;
    public GameObject userPanel;
    public GameObject runsPanel;
    
    [Header("User Panel References")]
    public TextMeshProUGUI userUsernameText;
    
    private PlayerAPI playerAPI;

    void Start()
    {
        playerAPI = PlayerAPI.Instance;
        
        if (playerAPI == null)
        {
            Debug.LogError("[UIManager] Could not find PlayerAPI instance!");
            return;
        }

        playerAPI.OnPlayerLoggedIn.AddListener(OnLoginSuccess);
        playerAPI.OnPlayerRegistered.AddListener(OnRegisterSuccess);
        playerAPI.OnLoginFailed.AddListener(OnLoginFailed);
        playerAPI.OnRegistrationFailed.AddListener(OnRegistrationFailed);
        playerAPI.OnPlayerLoggedOut.AddListener(OnLogoutSuccess);
        playerAPI.OnSessionValidated.AddListener(OnSessionValidated);
        
        // Subscribe to PlayerAPI events
        if (PlayerAPI.Instance != null)
        {
            PlayerAPI.Instance.OnPlayerDeleteFailed.AddListener(OnPlayerDeleteFailed);
        }
        
        if (accountButton != null)
        {
            accountButton.onClick.AddListener(OnAccountButtonClicked);
        }
        
        if (runsButton != null)
        {
            runsButton.onClick.AddListener(OnRunsButtonClicked);
        }

        StartCoroutine(WaitForSessionValidation());
    }
    
    private System.Collections.IEnumerator WaitForSessionValidation()
    {
        yield return null;
        
        if (playerAPI != null && !string.IsNullOrEmpty(PlayerPrefs.GetString("SessionToken", "")))
        {
            yield return new WaitUntil(() => HasSessionValidationCompleted());
        }
        
        UpdateUI();
    }
    
    private bool HasSessionValidationCompleted()
    {
        return playerAPI.IsLoggedIn || string.IsNullOrEmpty(PlayerPrefs.GetString("SessionToken", ""));
    }
    
    private void OnSessionValidated(PlayerData playerData)
    {
        UpdateUI();
    }
    
    public void OnAccountButtonClicked()
    {        
        if (playerAPI != null && playerAPI.IsLoggedIn)
        {
            ShowUserPanel();
        }
        else
        {
            ShowLoginRegisterPanel();
        }
    }
    
    private void ShowLoginRegisterPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (loginRegisterPanel != null) loginRegisterPanel.SetActive(true);
        if (userPanel != null) userPanel.SetActive(false);
        if (runsPanel != null) runsPanel.SetActive(false);
    }
    
    private void ShowUserPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (loginRegisterPanel != null) loginRegisterPanel.SetActive(false);
        if (userPanel != null) userPanel.SetActive(true);
        if (runsPanel != null) runsPanel.SetActive(false);
        
        if (playerAPI != null && playerAPI.CurrentPlayer != null && userUsernameText != null)
        {
            userUsernameText.text = $"Welcome, {playerAPI.CurrentPlayer.username}!";
        }
    }
    
    private void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (loginRegisterPanel != null) loginRegisterPanel.SetActive(false);
        if (userPanel != null) userPanel.SetActive(false);
        if (runsPanel != null) runsPanel.SetActive(false);
    }
    
    public void OnLoginSuccess(PlayerData player)
    {
        SetStatusText($"Welcome back, {player.username}!");
        ClearInputs();
        ShowUserPanel();
        UpdateUI();
    }
    
    public void OnRegisterSuccess(PlayerData player)
    {
        SetStatusText($"Account created! Welcome, {player.username}!");
        ClearInputs();
        ShowUserPanel();
        UpdateUI();
    }
    
    public void OnLogoutSuccess()
    {
        SetStatusText("Logged out successfully");
        ShowMainMenu();
        UpdateUI();
    }
    
    public void OnBackButtonClicked()
    {
        ShowMainMenu();
    }
    
    public void OnLoginButtonClicked()
    {        
        string username = usernameInput.text;
        string password = passwordInput.text;
                
        if (ValidateInputs(username, password))
        {
            SetStatusText("Logging in...");
            
            playerAPI.LoginPlayer(username, password);
        }
    }
    
    public void OnRegisterButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        
        if (ValidateInputs(username, password))
        {
            SetStatusText("Creating account...");
            
            playerAPI.RegisterPlayer(username, password);
        }
    }
    
    private bool ValidateInputs(string username, string password)
    {
        if (string.IsNullOrEmpty(username))
        {
            SetStatusText("Please enter a username");
            return false;
        }
        
        if (string.IsNullOrEmpty(password))
        {
            SetStatusText("Please enter a password");
            return false;
        }
        
        return true;
    }
    
    public void SetStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
    
    public void OnLoginFailed(string error)
    {
        SetStatusText($"Login failed: {error}");
        Debug.LogError($"[UIManager] Login failed: {error}");
    }
    
    public void OnRegistrationFailed(string error)
    {
        SetStatusText($"Registration failed: {error}");
        Debug.LogError($"[UIManager] Registration failed: {error}");
    }
    
    private void ClearInputs()
    {
        if (usernameInput != null) usernameInput.text = "";
        if (passwordInput != null) passwordInput.text = "";
    }
    
    public void LoginButtonPressed()
    {
        OnLoginButtonClicked();
    }
    
    public void RegisterButtonPressed()
    {
        OnRegisterButtonClicked();
    }
    
    public void OnLogoutButtonClicked()
    {
        if (playerAPI != null && playerAPI.IsLoggedIn)
        {
            SetStatusText("Logging out...");
            playerAPI.LogoutPlayer();
        }
        else
        {
            SetStatusText("No active session to logout");
        }
    }

    private void UpdateUI()
    {
        if (runsButton != null)
        {
            runsButton.gameObject.SetActive(playerAPI.IsLoggedIn);
        }
    }

    public void OnRunsButtonClicked()
    {
        if (playerAPI != null && playerAPI.IsLoggedIn)
        {
            ShowRunsPanel();
        }
        else
        {
            SetStatusText("Please log in to view your runs");
        }
    }

    private void ShowRunsPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (loginRegisterPanel != null) loginRegisterPanel.SetActive(false);
        if (userPanel != null) userPanel.SetActive(false);
        if (runsPanel != null) runsPanel.SetActive(true);
    }

    public void OnPlayerDeleteFailed(string error)
    {
        Debug.LogError($"[UIManager] Player deletion failed: {error}");
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (PlayerAPI.Instance != null)
        {
            PlayerAPI.Instance.OnPlayerDeleteFailed.RemoveListener(OnPlayerDeleteFailed);
        }
    }
}
