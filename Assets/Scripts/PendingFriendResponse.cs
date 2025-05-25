using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class PendingFriendResponse : MonoBehaviour
{

    void Start()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {

        }
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var reference = mDatabaseRef.Child("users").Child(userId).Child("friendResponse");
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
        string friendId = snapshot.Key;
        Debug.Log("Respuesta de" + friendId + "estado:" + snapshot.Value);
        int estado = int.Parse(snapshot.Value.ToString());
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        string friendUsername = (await FirebaseDatabase.DefaultInstance.GetReference("users/" + friendId + "/username").GetValueAsync()).Value.ToString();


        var cheskRequestId = await (FirebaseDatabase.DefaultInstance.GetReference("users/" + userId + "/SendRequests/" + friendId).GetValueAsync());
        //Validar si la respuesta es una solicitud pendiente
        if (cheskRequestId.Value == null)
        {
            Debug.Log("Se elimino la solicitud de amistad con id" + friendId);
            eliminarSolicitud(friendId, "FriendResponse");
            return;
        }

        // // Estado 1 para solicitud aceptada
        if (estado == 1)
        {
            Debug.Log(friendId + " ha aceptado tu solicitud");
            mDatabaseRef.Child("users").Child(userId).Child("friends").Child(friendId).SetValueAsync(friendUsername);
        }

        // Estado 2 para solicitud rechazada
        if (estado == 2)
        {
            Debug.Log(friendId + " ha rechazado tu solicitud");
        }
        eliminarSolicitud(friendId, "SendRequests");
        eliminarSolicitud(friendId, "friendResponse");

    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        Debug.Log(snapshot.Value + "se ha desconectado"); 

    }

    private void eliminarSolicitud(string requestUserId, string requestMailbox)
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
