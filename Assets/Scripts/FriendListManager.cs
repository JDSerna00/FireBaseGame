using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.UI;
using System.Collections.Generic;

public class FriendListManager : MonoBehaviour
{
    public Transform friendsPanel; 
    public GameObject friendTextPrefab;

    private DatabaseReference friendsRef;
    private HashSet<string> existingFriends = new HashSet<string>();

    private void Start()
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("No hay usuario autenticado.");
            return;
        }

        string userId = user.UserId;
        friendsRef = FirebaseDatabase.DefaultInstance.GetReference("users").Child(userId).Child("friends");

        // Escuchar amigos nuevos
        friendsRef.ChildAdded += HandleFriendAdded;

        // Inicial: cargar los amigos que ya existen
        LoadExistingFriends(userId);
    }

    private async void LoadExistingFriends(string userId)
    {
        var snapshot = await FirebaseDatabase.DefaultInstance.GetReference("users").Child(userId).Child("friends").GetValueAsync();
        if (!snapshot.Exists) return;

        foreach (var child in snapshot.Children)
        {
            string friendUid = child.Key;
            if (!existingFriends.Contains(friendUid))
            {
                existingFriends.Add(friendUid);
                AddFriendToUI(friendUid);
            }
        }
    }

    private async void HandleFriendAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Database error en HandleFriendAdded: " + args.DatabaseError.Message);
            return;
        }

        string friendUid = args.Snapshot.Key;
        Debug.Log("Amigo activo en Firebase: " + friendUid);

        if (existingFriends.Contains(friendUid)) return; // evitar duplicados
        existingFriends.Add(friendUid);

        AddFriendToUI(friendUid);
    }

    private async void AddFriendToUI(string friendUid)
    {
        Debug.Log("Intentando obtener username del UID: " + friendUid);

        var usernameSnapshot = await FirebaseDatabase.DefaultInstance
            .GetReference("users").Child(friendUid).Child("username").GetValueAsync();

        if (!usernameSnapshot.Exists)
        {
            Debug.LogWarning("No se encontró el username para: " + friendUid);
            return;
        }

        string username = usernameSnapshot.Value.ToString();
        Debug.Log("Username obtenido: " + username);

        GameObject friendText = Instantiate(friendTextPrefab, friendsPanel);
        friendText.GetComponent<TMPro.TMP_Text>().text = username;
        friendText.SetActive(true);

        Debug.Log("Amigo instanciado en el panel: " + username);
    }
}
