using System;
using Firebase.Auth;
using Firebase.Database;
using Unity.VisualScripting;
using UnityEngine;

public class FriendRequestMailbox : MonoBehaviour
{
    [SerializeField] private GameObject friendRequestItemPrefab;
    [SerializeField] private Transform friendRequestPanelContent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        Debug.Log("OnEnable");
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
            var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            var reference = mDatabaseRef.Child("users").Child(userId).Child("friendRequests");
            reference.ChildAdded += HandleChildAdded;
        }
    }
    
    private void OnDisable()
    {
        Debug.Log("OnDisable");
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
            var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            var reference = mDatabaseRef.Child("users").Child(userId).Child("friendRequests");
            reference.ChildAdded -= HandleChildAdded;
        }
    }


    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string friendUserId = args.Snapshot.Key;
        Debug.Log("Solicitud de amistad recibida de: " + friendUserId);

        GameObject newRequest = Instantiate(friendRequestItemPrefab, friendRequestPanelContent);
        newRequest.transform.localScale = Vector3.one;
        Debug.Log("Instanciado"); 
        var requestUI = newRequest.GetComponent<FriendRequestUIItem>();
        requestUI.Initialize(friendUserId, friendRequestPanelContent, friendRequestItemPrefab);

    }

}
