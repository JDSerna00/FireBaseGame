using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class PendingFriendResponse : MonoBehaviour
{
    private DatabaseReference mDatabaseRef;
    private string userId;

    void Start()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            Debug.LogWarning("No hay usuario autenticado");
            return;
        }

        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // Escuchar respuestas a las solicitudes del usuario actual
        mDatabaseRef.Child("users").Child(userId).Child("friendResponse")
            .ChildAdded += HandleChildAdded;
    }

    private async void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            Debug.LogWarning("Usuario no autenticado, cancelando manejo de respuesta");
            return;
        }

        string friendId = args.Snapshot.Key;
        string estadoStr = args.Snapshot.Value?.ToString();

        if (string.IsNullOrEmpty(estadoStr))
        {
            Debug.LogWarning("Estado inválido en respuesta");
            return;
        }

        int estado = int.Parse(estadoStr);

        // Verificar si todavía existe la solicitud
        var checkRequest = await mDatabaseRef.Child("users")
            .Child(userId)
            .Child("SendRequests")
            .Child(friendId)
            .GetValueAsync();

        if (!checkRequest.Exists)
        {
            Debug.Log("Solicitud ya eliminada, limpiando respuesta");
            eliminarSolicitud(friendId, "friendResponse");
            return;
        }

        if (estado == 1) // Aceptada
        {
            Debug.Log("El usuario " + friendId + " aceptó tu solicitud");

            // Agregar a ambos usuarios como amigos usando solo UID
            mDatabaseRef.Child("users").Child(userId).Child("friends").Child(friendId).SetValueAsync(true);
            mDatabaseRef.Child("users").Child(friendId).Child("friends").Child(userId).SetValueAsync(true);
        }
        else if (estado == 2) // Rechazada
        {
            Debug.Log("El usuario " + friendId + " rechazó tu solicitud");
        }

        // Eliminar nodos
        eliminarSolicitud(friendId, "SendRequests");
        eliminarSolicitud(friendId, "friendResponse");
    }

    private void eliminarSolicitud(string requestUserId, string requestMailbox)
    {
        if (string.IsNullOrEmpty(userId)) return;

        mDatabaseRef.Child("users")
            .Child(userId)
            .Child(requestMailbox)
            .Child(requestUserId)
            .RemoveValueAsync();
    }
}
