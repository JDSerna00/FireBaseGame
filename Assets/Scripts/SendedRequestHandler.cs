using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class SendedRequestHandler : MonoBehaviour
{

    void Start()
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        var reference = mDatabaseRef.Child("users").Child("users-online").Child("SendRequest");
        reference.ChildChanged += HandleChildChanged;
        reference.ChildAdded += HandleChildAdded;
        // reference.ChildAdded += HandleChildRemoved;
    }


    private async void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        Debug.Log(snapshot.Key + ":Solicitud pendiente"); 

    }
    
    private async void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;

        string friendId = snapshot.Key; 
        int estado = (int)snapshot.Value;
        
        string friendUsername = (await FirebaseDatabase.DefaultInstance.GetReference("users/" + friendId + "/username").GetValueAsync()).Value.ToString();

        if (estado == 1)
        {
            Debug.Log(friendId + " ha aceptado tu solicitud");
            eliminarSolicitud(snapshot.Key);
            var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
            var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId; 
            mDatabaseRef.Child("users").Child(userId).Child("friends").SetValueAsync(friendId);
        }
        if (estado == 2)
        {
            Debug.Log(friendId + " ha rechazado tu solicitud");
            eliminarSolicitud(snapshot.Key); 
        }

    }

    /* private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        Debug.Log(snapshot.Value + "se ha desconectado"); 

    } */

    private void eliminarSolicitud(string requestUserId)
    {
        var mDatabaseRe = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        mDatabaseRe.Child("users")
            .Child(userId)
            .Child("SendRequests")
            .Child(requestUserId)
            .SetValueAsync(null);
    }

}
