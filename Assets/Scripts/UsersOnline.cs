using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UsersOnline : MonoBehaviour
{

    [SerializeField] private GameObject alertPrefab;
    [SerializeField] private GameObject userEntryPrefab;
    [SerializeField] private string currentUserId;
    void Start()
    {
        currentUserId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        var reference = FirebaseDatabase.DefaultInstance.GetReference("users-online");
        reference.ChildAdded += HandleChildAdded;
        reference.ChildRemoved += HandleChildRemoved;
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        string userId = snapshot.Key;
        string username = ExtractUsername(snapshot.Value);

        Debug.Log($"{username} (ID: {userId}) has connected");
        ShowConnectionAlert(userId, $"{username} has connected");
    }
    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        string userId = snapshot.Key;
        string username = ExtractUsername(snapshot.Value);

        Debug.Log($"{username} (ID: {userId}) has disconnected");
        ShowConnectionAlert(userId, $"{username} has disconnected");
    }
    private string ExtractUsername(object userData)
    {
        if (userData == null) return "Unknown User";

        if (userData is string)
        {
            return (string)userData;
        }
        else if (userData is Dictionary<string, object> userDict)
        {
            return userDict.TryGetValue("username", out object nameObj)
                ? nameObj.ToString()
                : "Unknown User";
        }

        return userData.ToString();
    }

    private void ShowConnectionAlert(string userId, string message)
    {
        // Skip if current user or message is empty
        if (userId == currentUserId || string.IsNullOrEmpty(message))
            return;

        if (alertPrefab == null)
        {
            Debug.LogError("ConnectionAlertPrefab not assigned");
            return;
        }

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene");
            return;
        }

        GameObject alert = Instantiate(alertPrefab, canvas.transform);
        TextMeshProUGUI textComponent = alert.GetComponentInChildren<TextMeshProUGUI>();

        if (textComponent != null)
        {
            textComponent.text = message;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in alert prefab");
        }

        Destroy(alert, 2f);
    }

    private void OnApplicationQuit()
    {
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        mDatabaseRef.Child("users-online").Child(userId).SetValueAsync(null);
        Debug.Log("User logged out: " + userId); //Debug log to check if the user is logged out
    }
    
}
