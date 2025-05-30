using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class FriendRequestMailbox : MonoBehaviour
{
    [SerializeField] private GameObject friendRequestItemPrefab;
    [SerializeField] private Transform friendRequestPanelContent;

    private DatabaseReference _requestsRef;
    private Dictionary<string, GameObject> requestItems = new Dictionary<string, GameObject>();

    private string currentUserId;
    private bool isSubscribed = false;

    void Start()
    {
        TrySubscribeToUserRequests();
    }

    void Update()
    {
        // Si el usuario cambió (logout/login), resuscribirse
        string newUserId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

        if (newUserId != currentUserId)
        {
            Debug.Log("Cambio de usuario detectado.");
            UnsubscribeFromRequests();
            currentUserId = newUserId;
            TrySubscribeToUserRequests();
        }
    }

    private void TrySubscribeToUserRequests()
    {
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogWarning("No hay un usuario logueado.");
            return;
        }

        _requestsRef = FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .Child(currentUserId)
            .Child("friendRequests");

        _requestsRef.ValueChanged += OnFriendRequestsChanged;
        isSubscribed = true;

        Debug.Log("Mailbox suscrito a: " + currentUserId);
    }

    private void UnsubscribeFromRequests()
    {
        if (_requestsRef != null && isSubscribed)
        {
            _requestsRef.ValueChanged -= OnFriendRequestsChanged;
            Debug.Log("Mailbox desuscrito de: " + currentUserId);
        }

        ClearRequestItems();
        isSubscribed = false;
    }

    private void OnFriendRequestsChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Error al leer solicitudes: " + args.DatabaseError.Message);
            return;
        }

        ClearRequestItems();

        if (args.Snapshot.Exists && args.Snapshot.HasChildren)
        {
            foreach (var child in args.Snapshot.Children)
            {
                string senderId = child.Key;
                GameObject requestGO = Instantiate(friendRequestItemPrefab, friendRequestPanelContent);
                requestGO.GetComponent<FriendRequestUIItem>().Initialize(senderId, friendRequestPanelContent, friendRequestItemPrefab);
                requestItems[senderId] = requestGO;
            }
        }
    }

    private void ClearRequestItems()
    {
        foreach (var item in requestItems.Values)
        {
            Destroy(item);
        }

        requestItems.Clear();
    }

    private void OnDisable()
    {
        UnsubscribeFromRequests();
    }
}