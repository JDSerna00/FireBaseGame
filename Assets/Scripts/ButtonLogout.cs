using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonLogout : MonoBehaviour
{
    public void Logout()
    {
        var auth = FirebaseAuth.DefaultInstance;
        var user = auth.CurrentUser;

        if (user != null)
        {
            var userId = user.UserId;
            var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

            // Remove from users-online before signing out
            mDatabaseRef.Child("users-online").Child(userId).SetValueAsync(null);

            auth.SignOut();
            Debug.Log("User logged out: " + userId);
        }
        else
        {
            Debug.LogWarning("No user currently logged in.");
        }
    }

  
}
