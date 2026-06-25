using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class LLMService : MonoBehaviour
{

    [Header("API Settings")]

    [SerializeField]
    private string apiKey = "your_api_key";

    [SerializeField]
    private string apiUrl = "https://api.deepseek.com/v1/chat/completions";

    [SerializeField]
    private string modelName = "deepseek-chat";

    //==================================================
    // MODEL SETTINGS
    //==================================================

    [Header("Model Settings")]

    [Range(0f, 2f)]
    [SerializeField]
    private float temperature = 0.7f;

    [Range(1, 2048)]
    [SerializeField]
    private int maxTokens = 256;

    
    // CALLBACK

    public delegate void ResponseCallback(string response, bool success);

    /// <summary>
    /// Send messages to LLM
    /// </summary>
    public void SendRequest(
        List<Message> messages,
        ResponseCallback callback)
    {
        StartCoroutine(ProcessRequest(messages, callback));
    }

    //==================================================
    // REQUEST PROCESS
    //==================================================

    private IEnumerator ProcessRequest(
        List<Message> messages,
        ResponseCallback callback)
    {
        ChatRequest requestBody = new ChatRequest
        {
            model = modelName,
            messages = messages,
            temperature = temperature,
            max_tokens = maxTokens
        };

        string jsonBody = JsonUtility.ToJson(requestBody);

        /*Debug.Log("Sending Request:");
        Debug.Log(jsonBody);*/

        UnityWebRequest request = CreateWebRequest(jsonBody);

        yield return request.SendWebRequest();

        if (IsRequestError(request))
        {
            Debug.LogError("LLM Request Failed");
            Debug.LogError(request.downloadHandler.text);

            callback?.Invoke(null, false);

            yield break;
        }

        DeepSeekResponse response =
            ParseResponse(request.downloadHandler.text);

        if (response != null &&
            response.choices != null &&
            response.choices.Length > 0)
        {
            string content =
                response.choices[0].message.content;

            callback?.Invoke(content, true);
        }
        else
        {
            Debug.LogError("Invalid Response");

            callback?.Invoke(null, false);
        }
    }

    //==================================================
    // WEB REQUEST
    //==================================================

    private UnityWebRequest CreateWebRequest(string jsonBody)
    {
        byte[] bodyRaw =
            System.Text.Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request =
            new UnityWebRequest(apiUrl, "POST");

        request.uploadHandler =
            new UploadHandlerRaw(bodyRaw);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json");

        request.SetRequestHeader(
            "Authorization",
            $"Bearer {apiKey}");

        request.SetRequestHeader(
            "Accept",
            "application/json");

        return request;
    }

    //==================================================
    // ERROR CHECK
    //==================================================

    private bool IsRequestError(UnityWebRequest request)
    {
        return request.result ==
               UnityWebRequest.Result.ConnectionError ||

               request.result ==
               UnityWebRequest.Result.ProtocolError ||

               request.result ==
               UnityWebRequest.Result.DataProcessingError;
    }

    //==================================================
    // RESPONSE PARSE
    //==================================================

    private DeepSeekResponse ParseResponse(string jsonResponse)
    {
        try
        {
            return JsonUtility.FromJson<DeepSeekResponse>(
                jsonResponse);
        }
        catch (Exception e)
        {
            Debug.LogError("JSON Parse Error");
            Debug.LogError(e.Message);
            Debug.LogError(jsonResponse);

            return null;
        }
    }

    //==================================================
    // REQUEST DATA
    //==================================================

    [Serializable]
    private class ChatRequest
    {
        public string model;

        public List<Message> messages;

        public float temperature;

        public int max_tokens;
    }

    //==================================================
    // MESSAGE
    //==================================================

    [Serializable]
    public class Message
    {
        public string role;

        public string content;
    }

    //==================================================
    // RESPONSE DATA
    //==================================================

    [Serializable]
    private class DeepSeekResponse
    {
        public Choice[] choices;
    }

    [Serializable]
    private class Choice
    {
        public Message message;
    }
}

