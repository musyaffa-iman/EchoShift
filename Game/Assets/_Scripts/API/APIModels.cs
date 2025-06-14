using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region Player Data Structures
[Serializable]
public class PlayerData
{
    public string id;
    public string username;
    public string password;
    public int experience;
    public int level;
}

[Serializable]
public class LoginRequest
{
    public string username;
    public string password;
    
    public LoginRequest(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

[Serializable]
public class LoginResponseData
{
    public PlayerData player;
    public string sessionToken;
}
#endregion

#region Run Data Structures
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

[Serializable]
public class RunListResponse
{
    public bool success;
    public string message;
    public List<RunResponse> data;
}
#endregion

#region Generic Response Structure
[Serializable]
public class BaseResponse<T>
{
    public bool success;
    public string message;
    public T data;
}
#endregion

#region Unity Events
[Serializable]
public class PlayerAuthenticatedEvent : UnityEvent<PlayerData> { }

[Serializable]
public class AuthenticationFailedEvent : UnityEvent<string> { }

[Serializable]
public class RunStartedEvent : UnityEvent<RunData> { }

[Serializable]
public class RunUpdatedEvent : UnityEvent<RunData> { }

[Serializable]
public class RunEndedEvent : UnityEvent<RunData> { }

[Serializable]
public class RunFailedEvent : UnityEvent<string> { }
#endregion