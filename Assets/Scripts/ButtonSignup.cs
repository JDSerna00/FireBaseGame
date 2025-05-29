using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSignup : MonoBehaviour
{
    FirebaseAuth auth;
    [SerializeField] private Button _registrationButton;
    private Coroutine _registrationCoroutine;
    [SerializeField] GameObject alertPanel;

    private void Reset()
    {
        _registrationButton = GetComponent<Button>();
    }

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        _registrationButton.onClick.AddListener(HandleRegistrationButtonClick);
        alertPanel = GameObject.Find("AlertPanel");
        alertPanel.SetActive(false); // Ensure the alert panel is hidden at start
    }

    private void HandleRegistrationButtonClick()
    {
        string email = GameObject.Find("InputFieldEmail").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;

        _registrationCoroutine = StartCoroutine(RegisterUser(email, password));
    }

    IEnumerator RegisterUser(string email, string password)
    {
        string username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text; 
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.IsCanceled)
        {
            Debug.LogError("CreateruserWithEmailAndPasswordAsync was canceled");
        }
        else if (registerTask.IsFaulted)
        {
            foreach (var innerException in registerTask.Exception.Flatten().InnerExceptions)
            {
                if (innerException is FirebaseException firebaseEx)
                {
                    if (firebaseEx.ErrorCode == (int)AuthError.EmailAlreadyInUse)
                    {
                        ShowAlert("This email is already registered");
                        yield break; // Exit the coroutine
                    }
                }
            }

            // For other errors
            ShowAlert("Registration failed. Please try again.");
        }
        else
        {
            AuthResult result = registerTask.Result;
            FirebaseDatabase.DefaultInstance.RootReference
                .Child("users")
                .Child(result.User.UserId)
                .Child("username")
                .SetValueAsync(username);
            Debug.LogFormat("Firebase user created sucessfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
        }
    }
    void ShowAlert(string message)
    {
        TextMeshProUGUI alertText = alertPanel.transform.Find("AlertText").GetComponent<TextMeshProUGUI>();
        Button okButton = alertPanel.transform.Find("OKButton").GetComponent<Button>();

        alertText.text = message;
        alertPanel.SetActive(true);

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() => {
            alertPanel.SetActive(false);
        });

        StartCoroutine(HideAlertAfterDelay(2f));
    }

    private IEnumerator HideAlertAfterDelay(float v)
    {
        yield return new WaitForSeconds(v);
        alertPanel.SetActive(false);
    }
}