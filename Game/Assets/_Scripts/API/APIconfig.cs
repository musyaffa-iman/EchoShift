using UnityEngine;

public static class APIConfig
{
    public const string BASE_URL = "https://echoshift-be.z99h2u.easypanel.host";
    
    // API endpoints
    public const string API_BASE = BASE_URL + "/api";
    public const string RUNS_ENDPOINT = API_BASE + "/runs";
    public const string PLAYERS_ENDPOINT = API_BASE + "/players";
    
    // URL patterns
    public const string LOGIN_URL = PLAYERS_ENDPOINT + "/login";
    public const string LOGOUT_URL = PLAYERS_ENDPOINT + "/logout";
    public const string SESSION_VALIDATE_URL = PLAYERS_ENDPOINT + "/session/validate";
    
    // PlayerPrefs keys
    public const string SESSION_TOKEN_KEY = "SessionToken";
    public const string PLAYER_DATA_KEY = "PlayerData";
}