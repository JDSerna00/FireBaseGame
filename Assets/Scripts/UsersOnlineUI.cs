using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using System.Collections.Generic;
using Firebase.Auth;
using TMPro;

public class OnlineUsersPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentParent; // ScrollView's Content object
    [SerializeField] private GameObject userEntryPrefab; // Prefab for each user entry

    private Dictionary<string, GameObject> userEntries = new Dictionary<string, GameObject>();
    [SerializeField] private string currentUserId;

    void Start()
    {
        InitializePanel();
    }

    private void InitializePanel()
    {
        // Get current user ID
        currentUserId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

        // Set up Firebase listeners
        DatabaseReference onlineUsersRef = FirebaseDatabase.DefaultInstance.GetReference("users-online");
        onlineUsersRef.ChildAdded += HandleUserConnected;
        onlineUsersRef.ChildRemoved += HandleUserDisconnected;

        // Load initial online users
        onlineUsersRef.GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot userSnapshot in snapshot.Children)
                {
                    UpdateUserEntry(userSnapshot.Key, userSnapshot.Value.ToString());
                }
            }
        });
    }

    private void HandleUserConnected(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        UpdateUserEntry(args.Snapshot.Key, args.Snapshot.Value);
    }

    private void HandleUserDisconnected(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        RemoveUserEntry(args.Snapshot.Key);
    }

    private void UpdateUserEntry(string userId, object userData)
    {
        // Skip current user
        if (userId == currentUserId) return;

        // Extract username from userData
        string username;

        if (userData is string)
        {
            // Simple string case
            username = (string)userData;
        }
        else if (userData is Dictionary<string, object> userDict)
        {
            // Dictionary case - look for a "username" field
            username = userDict.TryGetValue("username", out object nameObj)
                ? nameObj.ToString()
                : "Unknown User";
        }
        else
        {
            // Fallback
            username = userData?.ToString() ?? "Unknown User";
        }

        Debug.Log($"User ID: {userId} | Username: {username}");

        // Rest of your existing code
        if (userEntries.ContainsKey(userId))
        {
            userEntries[userId].GetComponentInChildren<TextMeshProUGUI>().text = username;
            return;
        }

        GameObject userEntry = Instantiate(userEntryPrefab, contentParent);
        userEntry.GetComponentInChildren<TextMeshProUGUI>().text = username;
        userEntries.Add(userId, userEntry);
    }

    private void RemoveUserEntry(string userId)
    {
        if (userEntries.TryGetValue(userId, out GameObject entry))
        {
            Destroy(entry);
            userEntries.Remove(userId);
        }
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (FirebaseDatabase.DefaultInstance != null)
        {
            DatabaseReference onlineUsersRef = FirebaseDatabase.DefaultInstance.GetReference("users-online");
            onlineUsersRef.ChildAdded -= HandleUserConnected;
            onlineUsersRef.ChildRemoved -= HandleUserDisconnected;
        }
    }
}