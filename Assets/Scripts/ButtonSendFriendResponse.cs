using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using Firebase.Auth;
using Firebase.Database;
using System;

public enum FriendResponse
{
    Accept = 1, 
    Reject = 2, 
}
public class ButtonSendFriendResponse : MonoBehaviour
{

    [SerializeField] private Button _sendResponseButton;
    [SerializeField] private TMP_InputField _frienUserIdInputField;

    private void Reset()
    {
        _sendResponseButton = GetComponent<Button>();
        _frienUserIdInputField = GameObject.Find("InputFieldFriendUserId").GetComponent<TMP_InputField>();
    }
    void Start()
    {
        _sendResponseButton.onClick.AddListener(HandleSendResponseButtonClicked);
    }

    async private void HandleSendResponseButtonClicked()
    {
        string friendUserId = _frienUserIdInputField.text;
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var friendUsername = (await FirebaseDatabase.DefaultInstance.GetReference("users/" + friendUserId + "/username").GetValueAsync()).Value?.ToString();

        //Respuesta estática que acepta la solicitud de amistad (estado 1)
        await mDatabaseRef.Child("users")
        .Child(friendUserId)
        .Child("friendResponse")
        .Child(userId)
        .SetValueAsync(1);

        await mDatabaseRef.Child("users").Child(userId).Child("friends").Child(friendUserId).SetValueAsync(friendUsername);
    }

}
