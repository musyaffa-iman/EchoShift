using UnityEngine;

public static class APIConfig
{
    //public const string BASE_URL = "http://localhost:8080";
    public const string BASE_URL = "https://echoshift-be.z99h2u.easypanel.host";
    
    // API endpoints
    public const string API_BASE = BASE_URL + "/api";
    public const string RUNS_ENDPOINT = API_BASE + "/runs";
    public const string PLAYERS_ENDPOINT = API_BASE + "/players";
}