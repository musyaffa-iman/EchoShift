using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public static class APIHelper
{
    public static IEnumerator PostCoroutine(string url, string jsonData, System.Action<UnityWebRequest> onComplete)
    {
        using UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        onComplete?.Invoke(request);
    }

    public static IEnumerator PostCoroutineWithAuth(string url, string jsonData, System.Action<UnityWebRequest> onComplete, string authToken)
    {
        using UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", authToken);
        }

        yield return request.SendWebRequest();

        onComplete?.Invoke(request);
    }

    public static IEnumerator GetCoroutine(string url, System.Action<UnityWebRequest> onComplete, string authToken = null)
    {
        using UnityWebRequest request = UnityWebRequest.Get(url);
        
        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", authToken);
        }
        
        yield return request.SendWebRequest();
        onComplete?.Invoke(request);
    }

    public static IEnumerator PutCoroutine(string url, string jsonData, System.Action<UnityWebRequest> onComplete, string authToken = null)
    {
        using UnityWebRequest request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", authToken);
        }

        yield return request.SendWebRequest();

        onComplete?.Invoke(request);
    }

    public static IEnumerator PatchCoroutine(string url, string jsonData, System.Action<UnityWebRequest> onComplete, string authToken = null)
    {
        using UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", authToken);
        }

        yield return request.SendWebRequest();

        onComplete?.Invoke(request);
    }

    public static IEnumerator DeleteCoroutine(string url, System.Action<UnityWebRequest> callback)
    {
        using UnityWebRequest request = UnityWebRequest.Delete(url);
        
        if (PlayerAPI.Instance != null && !string.IsNullOrEmpty(PlayerAPI.Instance.SessionToken))
        {
            request.SetRequestHeader("Authorization", PlayerAPI.Instance.SessionToken);
        }
        
        yield return request.SendWebRequest();
        callback?.Invoke(request);
    }
}
