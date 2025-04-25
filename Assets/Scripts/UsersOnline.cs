using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class UsersOnline : MonoBehaviour
{
    void Start()
    {
        
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
        Debug.Log(snapshot.Value + " se ha conectado");     
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


    private void OnApplicationQuit()
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        mDatabaseRef.Child("users-online").Child(userId).SetValueAsync(null);
    }
    
}
