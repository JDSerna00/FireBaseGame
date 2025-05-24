using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class AuthStateHandler : MonoBehaviour
{
    [SerializeField]
    GameObject _panelAuth;

    [SerializeField]
    GameObject _panelScore;

    void Awake()
    {
        // Initialize panel references (ensure they exist in the scene with correct names)
        _panelAuth = GameObject.Find("PanelAuth");
        _panelScore = GameObject.Find("PanelScore");

        if (_panelAuth != null) _panelAuth.SetActive(true);
        if (_panelScore != null) _panelScore.SetActive(false);
    }

    void Start()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("(Clone)")) // Checks for Unity's default clone suffix
            {
                Destroy(obj);
            }
        }
        FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;

    }

    void OnDestroy()
    {
        // Clean up event listener
        FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
    }

    void Update()
    {
        /*var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;

        if (currentUser != null)
        {
            var userId = currentUser.UserId;
            Debug.Log(userId + " and " + mDatabaseRef);
        }*/
    }

    private void HandleStateChanged(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            Invoke("SetAuth", 2f);
            SetOnline();
        }
        else
        {
            if (_panelAuth != null) _panelAuth.SetActive(true);
            if (_panelScore != null) _panelScore.SetActive(false);
        }
    }

    private void SetAuth()
    {
        if (_panelAuth != null) _panelAuth.SetActive(false);
        if (_panelScore != null) _panelScore.SetActive(true);

        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser != null)
        {
            Debug.Log("User authenticated: " + currentUser.UserId);
        }
    }

    private void SetOnline()
    {
        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser == null) return;

        var userId = currentUser.UserId;
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + userId + "/username")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning("Error retrieving username.");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    string username = snapshot.Value?.ToString();
                    PlayerPrefs.SetString("username", username);
                    if (!string.IsNullOrEmpty(username))
                    {
                        mDatabaseRef.Child("users-online").Child(userId).SetValueAsync(username);
                        Debug.Log("User logged in: " + userId);
                    }
                }
            });
    }
    
}
