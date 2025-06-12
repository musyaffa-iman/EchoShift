using System;
using UnityEngine;

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
public class LoginResponseData
{
    public PlayerData player;
    public string sessionToken;
}

[Serializable]
public class BaseResponse<T>
{
    public bool success;
    public string message;
    public T data;
}
