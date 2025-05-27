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

    private void Reset()
    {
        _registrationButton = GetComponent<Button>();
    }

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        _registrationButton.onClick.AddListener(HandleRegistrationButtonClick);
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
            Debug.LogError("CreateruserWithEmailAndPasswordAsync encountered an error" + registerTask.Exception);
        }
        else
        {
            AuthResult result = registerTask.Result;
            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(result.User.UserId).Child("username").SetValueAsync(username);
            Debug.LogFormat("Firebase user created sucessfully: {0} ({1})", result.User.DisplayName, result.User.UserId); 
        }
    }

}