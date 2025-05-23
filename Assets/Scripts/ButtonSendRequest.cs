using Firebase.Auth;
using Firebase.Database;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public class ButtonSendRequest : MonoBehaviour
{
    [SerializeField] private Button _sendResquestButton;
    [SerializeField] private TMP_InputField _frienUsernameInputField;

    private void Reset()
    {
        _sendResquestButton = GetComponent<Button>();
        _frienUsernameInputField = GameObject.Find("InputFieldFriendUsername").GetComponent<TMP_InputField>();
    }
    void Start()
    {
        _sendResquestButton.onClick.AddListener(HandleSendRequestButtonClicked);
    }

    private void HandleSendRequestButtonClicked()
    {
        string friendUserId = _frienUsernameInputField.text;
        var mDatabaseRe = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var username = PlayerPrefs.GetString("username");

        mDatabaseRe.Child("users")
        .Child(friendUserId)
        .Child("friendRequests")
        .Child(userId)
        .SetValueAsync(username).ContinueWith(t =>
        {
            mDatabaseRe.Child("users")
            .Child(userId)
            .Child("SendRequests")
            .Child(friendUserId)
            .SetValueAsync(0); 
        }); 
    }
}
