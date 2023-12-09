using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class HighScoreSender : MonoBehaviour
{
    private string apiUrl = "http://localhost:5000/add_score"; // url of flask api
    public static HighScoreSender Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the instance alive across scenes
        }
        else
        {
            Debug.LogError("Attempted to instantiate second object of a singleton class.");
            Destroy(gameObject);
        }
    }

    public void SendHighScore(string connectionId, int score)
    {
        StartCoroutine(PostHighScore(connectionId, score));
    }

    private IEnumerator PostHighScore(string connectionId, int score)
    {
        string jsonData = JsonUtility.ToJson(new HighScoreData(connectionId, score));
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(jsonToSend),
            downloadHandler = new DownloadHandlerBuffer(),
            timeout = 10 // Set a timeout (in seconds)
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error uploading high score: {request.error}");
        }
        else
        {
            Debug.Log($"High score uploaded successfully. Server response: {request.downloadHandler.text}");
        }
    }

    private class HighScoreData
    {
        public string connectionId;
        public int score;

        public HighScoreData(string connectionId, int score)
        {
            this.connectionId = connectionId;
            this.score = score;
        }
    }
}
