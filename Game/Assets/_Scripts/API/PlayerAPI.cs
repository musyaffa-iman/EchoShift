using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Text;

[Serializable]
public class PlayerAuthenticatedEvent : UnityEvent<PlayerData> { }

[Serializable]
public class AuthenticationFailedEvent : UnityEvent<string> { }

public class PlayerAPI : MonoBehaviour
{
    private static PlayerAPI _instance;
    
    public static PlayerAPI Instance => _instance;

    private string baseUrl = APIConfig.PLAYERS_ENDPOINT;
    
    private string currentSessionToken;
    private PlayerData currentPlayer;
    private bool isLoggedIn = false;

    [Header("Events")]
    public PlayerAuthenticatedEvent OnPlayerRegistered = new PlayerAuthenticatedEvent();
    public PlayerAuthenticatedEvent OnPlayerLoggedIn = new PlayerAuthenticatedEvent();
    public AuthenticationFailedEvent OnRegistrationFailed = new AuthenticationFailedEvent();
    public AuthenticationFailedEvent OnLoginFailed = new AuthenticationFailedEvent();
    public UnityEvent OnPlayerLoggedOut = new UnityEvent();
    public PlayerAuthenticatedEvent OnSessionValidated = new PlayerAuthenticatedEvent();
    public UnityEvent<string> OnPlayerDeleted = new UnityEvent<string>();
    public UnityEvent<string> OnPlayerDeleteFailed = new UnityEvent<string>();

    public bool IsLoggedIn => isLoggedIn;
    public PlayerData CurrentPlayer => currentPlayer;
    public string SessionToken => currentSessionToken;
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    void OnApplicationQuit()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    void Start()
    {
        RestoreSession();
    }

    #region Registration
    public void RegisterPlayer(string username, string password)
    {
        PlayerData newPlayer = new PlayerData
        {
            username = username,
            password = password,
            experience = 0
        };

        string json = JsonUtility.ToJson(newPlayer);

        StartCoroutine(APIHelper.PostCoroutine(baseUrl, json, OnPlayerRegistered_Callback));
    }

    private void OnPlayerRegistered_Callback(UnityWebRequest request)
    {
        if (!string.IsNullOrEmpty(request.downloadHandler.text))
        {
            try
            {
                string resultJson = request.downloadHandler.text;
                BaseResponse<LoginResponseData> response = JsonUtility.FromJson<BaseResponse<LoginResponseData>>(resultJson);
                
                if (response.success && response.data != null)
                {
                    SetPlayerSession(response.data.sessionToken, response.data.player);
                    OnPlayerRegistered?.Invoke(currentPlayer);
                }
                else
                {
                    Debug.LogError($"[PlayerAPI] Registration failed: {response.message}");
                    OnRegistrationFailed?.Invoke(response.message);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PlayerAPI] Failed to parse registration response: {e.Message}");
                OnRegistrationFailed?.Invoke("Failed to process server response");
            }
        }
        else
        {
            Debug.LogError($"[PlayerAPI] Registration request failed: {request.error}");
            OnRegistrationFailed?.Invoke($"Connection error: {request.error}");
        }
    }
    #endregion

    #region Login
    public void LoginPlayer(string username, string password)
    {
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("[PlayerAPI] Username is null or empty!");
            OnLoginFailed?.Invoke("Username cannot be empty");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("[PlayerAPI] Password is null or empty!");
            OnLoginFailed?.Invoke("Password cannot be empty");
            return;
        }

        string json = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";
        string loginUrl = $"{baseUrl}/login";

        StartCoroutine(APIHelper.PostCoroutine(loginUrl, json, OnPlayerLogin_Callback));
    }

    private void OnPlayerLogin_Callback(UnityWebRequest request)
    {
        if (!string.IsNullOrEmpty(request.downloadHandler.text))
        {
            try
            {
                string resultJson = request.downloadHandler.text;
                BaseResponse<LoginResponseData> response = JsonUtility.FromJson<BaseResponse<LoginResponseData>>(resultJson);
                
                if (response.success && response.data != null)
                {
                    SetPlayerSession(response.data.sessionToken, response.data.player);
                    OnPlayerLoggedIn?.Invoke(currentPlayer);
                }
                else
                {
                    OnLoginFailed?.Invoke(response.message);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PlayerAPI] Failed to parse login response: {e.Message}");
                OnLoginFailed?.Invoke("Failed to process server response");
            }
        }
        else
        {
            Debug.LogError($"[PlayerAPI] Login request failed: {request.error}");
            OnLoginFailed?.Invoke($"Connection error: {request.error}");
        }
    }
    #endregion

    #region Session Management
    private void SetPlayerSession(string sessionToken, PlayerData player)
    {
        currentSessionToken = sessionToken;
        currentPlayer = player;
        isLoggedIn = true;
        
        PlayerPrefs.SetString("SessionToken", sessionToken);
        PlayerPrefs.SetString("PlayerData", JsonUtility.ToJson(player));
        PlayerPrefs.Save();
    }
    
    public void LogoutPlayer()
    {
        if (!IsLoggedIn)
        {
            Debug.LogWarning("[PlayerAPI] No active session to logout");
            return;
        }
        
        string logoutUrl = $"{baseUrl}/logout";
        StartCoroutine(LogoutCoroutine(logoutUrl));
    }
    
    private IEnumerator LogoutCoroutine(string url)
    {
        using UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", currentSessionToken);
        
        yield return request.SendWebRequest();
        
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[PlayerAPI] Logout failed: {request.error}");
        }
        
        ClearSession();
        OnPlayerLoggedOut?.Invoke();
    }
    
    private void ClearSession()
    {
        currentSessionToken = null;
        currentPlayer = null;
        isLoggedIn = false;
        PlayerPrefs.DeleteKey("SessionToken");
        PlayerPrefs.DeleteKey("PlayerData");
        PlayerPrefs.Save();
    }
    
    public void ValidateSession()
    {
        if (!IsLoggedIn)
        {
            Debug.LogWarning("[PlayerAPI] No session to validate");
            return;
        }
        
        string validateUrl = $"{baseUrl}/session/validate";
        StartCoroutine(ValidateSessionCoroutine(validateUrl));
    }
    
    private IEnumerator ValidateSessionCoroutine(string url)
    {
        yield return StartCoroutine(APIHelper.GetCoroutine(url, OnSessionValidation_Callback, currentSessionToken));
    }
    
    private void OnSessionValidation_Callback(UnityWebRequest request)
    {
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[PlayerAPI] Session validation failed - clearing session");
            HandleInvalidSession();
        }
        else
        {
            try
            {
                string resultJson = request.downloadHandler.text;
                BaseResponse<PlayerData> response = JsonUtility.FromJson<BaseResponse<PlayerData>>(resultJson);
                
                if (response.success && response.data != null)
                {
                    currentPlayer = response.data;
                    isLoggedIn = true;
                    OnSessionValidated?.Invoke(currentPlayer);
                }
                else
                {
                    Debug.LogWarning($"[PlayerAPI] Session validation response indicates failure: {response.message}");
                    HandleInvalidSession();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PlayerAPI] Failed to parse session validation response: {e.Message}");
                HandleInvalidSession();
            }
        }
    }
    
    private void HandleInvalidSession()
    {
        ClearSession();
        OnPlayerLoggedOut?.Invoke();
    }
    
    public void RestoreSession()
    {
        string savedToken = PlayerPrefs.GetString("SessionToken", "");
        string savedPlayerData = PlayerPrefs.GetString("PlayerData", "");
        
        if (!string.IsNullOrEmpty(savedToken) && !string.IsNullOrEmpty(savedPlayerData))
        {
            currentSessionToken = savedToken;
            currentPlayer = JsonUtility.FromJson<PlayerData>(savedPlayerData);
            isLoggedIn = true;
            
            ValidateSession();
        }
        else
        {
            isLoggedIn = false;
        }
    }
    #endregion

    #region Player Management
    public void DeleteCurrentPlayer()
    {
        if (!IsLoggedIn)
        {
            Debug.LogWarning("[PlayerAPI] No logged in player to delete");
            OnPlayerDeleteFailed?.Invoke("No player logged in");
            return;
        }
        
        string playerIdToDelete = currentPlayer.id;
        
        StartCoroutine(LogoutThenDeleteCoroutine(playerIdToDelete));
    }
    
    private IEnumerator LogoutThenDeleteCoroutine(string playerIdToDelete)
    {
        string logoutUrl = $"{baseUrl}/logout";
        
        using UnityWebRequest request = new UnityWebRequest(logoutUrl, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", currentSessionToken);
        
        yield return request.SendWebRequest();
        
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[PlayerAPI] Logout failed, but proceeding with deletion anyway: {request.error}");
        }
        
        ClearSession();
        OnPlayerLoggedOut?.Invoke();
        
        string deleteUrl = $"{baseUrl}/{playerIdToDelete}";
        
        using UnityWebRequest deleteRequest = UnityWebRequest.Delete(deleteUrl);
        yield return deleteRequest.SendWebRequest();
        
        if (deleteRequest.result == UnityWebRequest.Result.Success && deleteRequest.responseCode == 200)
        {
            OnPlayerDeleted?.Invoke(playerIdToDelete);
        }
        else
        {
            Debug.LogError($"[PlayerAPI] Player deletion failed: {deleteRequest.error} (Status: {deleteRequest.responseCode})");
            OnPlayerDeleteFailed?.Invoke($"Deletion failed: {deleteRequest.error}");
        }
    }
    #endregion
}
