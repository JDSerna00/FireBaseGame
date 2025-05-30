using Firebase.Auth;
using Firebase.Database;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public class ButtonSendFriendRequest : MonoBehaviour
{
    [SerializeField] private Button _sendResquestButton;
    [SerializeField] private TMP_InputField _frienUserIdInputField;

    private void Reset()
    {
        _sendResquestButton = GetComponent<Button>();
        _frienUserIdInputField = GameObject.Find("InputFieldFriendUserId").GetComponent<TMP_InputField>();
    }
    void Start()
    {
        _sendResquestButton.onClick.AddListener(HandleSendRequestButtonClicked);
    }

    private void HandleSendRequestButtonClicked()
    {
        string friendUserId = _frienUserIdInputField.text;
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var username = PlayerPrefs.GetString("username");

        mDatabaseRef.Child("users")
        .Child(friendUserId)
        .Child("friendRequests")
        .Child(userId)
        .SetValueAsync(username).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                
            }
            else if (t.IsCompleted)
            {
                //Manejar el error
                mDatabaseRef.Child("users")
                    .Child(userId)
                    .Child("SendRequests")
                    .Child(friendUserId)
                    .SetValueAsync(0); 
                //Establece estado 0 para solicitudes pendientes
            }

        }); 
    }
}
