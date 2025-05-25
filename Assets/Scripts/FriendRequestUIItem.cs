using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FriendRequestUIItem : MonoBehaviour
{
    public TMP_Text textUserId;
    public TMP_InputField inputUserId;
    public Button buttonAccept;
    public Button buttonReject;

    public void Initialize(string userId, Transform container, GameObject prefab)
    {
        textUserId.text = $"Solicitud de: {userId}";
        inputUserId.text = userId;

        // Conecta el input al botón de respuesta
        var acceptScript = buttonAccept.GetComponent<ButtonSendFriendResponse>();
        var rejectScript = buttonReject.GetComponent<ButtonSendFriendResponse>();

        acceptScript._friendUserIdInputField = inputUserId;
        acceptScript.SetResponseType(FriendResponse.Accept);

        rejectScript._friendUserIdInputField = inputUserId;
        rejectScript.SetResponseType(FriendResponse.Reject);
    }
}