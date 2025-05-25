using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using System;

public enum FriendResponse
{
    Accept = 1,
    Reject = 2
}

public class ButtonSendFriendResponse : MonoBehaviour
{
    [SerializeField] private Button _sendResponseButton;
    public TMP_InputField _friendUserIdInputField;
    [SerializeField] private FriendResponse _responseType = FriendResponse.Accept;

    public void SetResponseType(FriendResponse responseType)
    {
        _responseType = responseType;
    }

    private void Reset()
    {
        _sendResponseButton = GetComponent<Button>();
        _friendUserIdInputField = GameObject.Find("InputFieldFriendUserId").GetComponent<TMP_InputField>();
    }

    void Start()
    {
        _sendResponseButton.onClick.AddListener(HandleSendResponseButtonClicked);
    }

    async private void HandleSendResponseButtonClicked()
    {
        string friendUserId = _friendUserIdInputField.text;

        if (string.IsNullOrEmpty(friendUserId))
        {
            Debug.LogWarning("Friend User ID is empty.");
            return;
        }

        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        // Guardamos la respuesta (1 = aceptar, 2 = rechazar)
        await mDatabaseRef
            .Child("users")
            .Child(userId)
            .Child("friendResponse")
            .Child(friendUserId)
            .SetValueAsync((int)_responseType);

        if (_responseType == FriendResponse.Accept)
        {
            // Si se acepta, también lo agregamos a la lista de amigos
            var friendUsername = (await FirebaseDatabase.DefaultInstance
                .GetReference("users/" + friendUserId + "/username")
                .GetValueAsync()).Value?.ToString();

            await mDatabaseRef
                .Child("users")
                .Child(userId)
                .Child("friends")
                .Child(friendUserId)
                .SetValueAsync(friendUsername);
        }
        else if (_responseType == FriendResponse.Reject)
        {
            // Si se rechaza, eliminamos la solicitud entrante
            await mDatabaseRef
                .Child("users")
                .Child(userId)
                .Child("friendRequests")
                .Child(friendUserId)
                .RemoveValueAsync();
        }

        Destroy(transform.parent.gameObject); 
        Debug.Log($"Respuesta de amistad enviada: {_responseType} para el usuario {friendUserId}");
        
    }
}
