using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class AuthStateHandler : MonoBehaviour
{
    [SerializeField] private GameObject _panelScore;
    private FirebaseAuth auth;

    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += HandleStateChanged;

        // Forzamos una primera llamada por si ya hay un usuario activo al iniciar
        HandleStateChanged(this, null);
    }

    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= HandleStateChanged;
        }
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
            Invoke("SetOnline", 2f);
            Debug.Log("Usuario autenticado");
        }
        else
        {
            Debug.Log("Usuario no autenticado.");
            _panelScore.SetActive(false); 
            // Puedes decidir mostrar login aquí si lo deseas
        }
    }

    private void SetAuth()
    {
        // Activar solo el panel de Score
        if (_panelScore != null) _panelScore.SetActive(true);
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
                        var UsersOnline = GetComponent<UsersOnline>();
                        UsersOnline.enabled = true; // Activar el script UsersOnline
                    }
                }
            });
    }
    
}
