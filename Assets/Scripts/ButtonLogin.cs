using Firebase;
using Firebase.Auth;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLogin : MonoBehaviour
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private GameObject _alertPanel; // Reference to your alert UI panel
    [SerializeField] private TMP_Text _alertText; // Reference to the alert text component
    [SerializeField] private Button _alertOkButton; // Reference to the OK button in the alert

    private Coroutine _loginRoutine;

    private void Reset()
    {
        _loginButton = GetComponent<Button>();
        _emailInputField = GameObject.Find("InputFieldEmail").GetComponent<TMP_InputField>();
        _passwordInputField = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>();
    }

    void Start()
    {
        _loginButton.onClick.AddListener(HandleLogin);
        _alertOkButton.onClick.AddListener(() => _alertPanel.SetActive(false));
        _alertPanel.SetActive(false);
    }

    void HandleLogin()
    {
        if (_loginRoutine != null)
        {
            StopCoroutine(_loginRoutine);
        }
        _loginRoutine = StartCoroutine(LoginUserCoroutine());
    }

    IEnumerator LoginUserCoroutine()
    {
        string email = _emailInputField.text;
        string password = _passwordInputField.text;

        // Basic validation
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowAlert("Please enter both email and password");
            yield break;
        }

        var auth = FirebaseAuth.DefaultInstance;
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.IsCanceled)
        {
            ShowAlert("Login was canceled");
            yield break;
        }

        if (loginTask.IsFaulted)
        {
            string errorMessage = "Login failed";

            foreach (var innerException in loginTask.Exception.Flatten().InnerExceptions)
            {
                if (innerException is FirebaseException firebaseEx)
                {
                    switch ((AuthError)firebaseEx.ErrorCode)
                    {
                        case AuthError.InvalidEmail:
                            errorMessage = "Invalid email address";
                            break;
                        case AuthError.WrongPassword:
                            errorMessage = "Incorrect password";
                            break;
                        case AuthError.UserNotFound:
                            errorMessage = "Account not found";
                            break;
                        case AuthError.TooManyRequests:
                            errorMessage = "Too many attempts. Try again later";
                            break;
                        case AuthError.UserDisabled:
                            errorMessage = "Account disabled";
                            break;
                        default:
                            errorMessage = "Login error occurred";
                            break;
                    }
                    break; // Show the first relevant error
                }
            }

            ShowAlert(errorMessage);
            yield break;
        }

        // Login successful
        AuthResult result = loginTask.Result;
        Debug.LogFormat("User signed in successfully: {0} ({1})",
            result.User.DisplayName, result.User.UserId);

        ShowAlert("Login successful!", isSuccess: true);
        _alertPanel.GetComponent<Image>().color = Color.green; // Change alert panel color to green for success
        // Here you would typically load the next scene or do post-login actions
    }

    void ShowAlert(string message, bool isSuccess = false)
    {
        _alertText.text = message;
        _alertText.color = isSuccess ? Color.green : Color.red;
        _alertPanel.SetActive(true);
        StartCoroutine(HideAlertAfterDelay(3f));
    }

    private IEnumerator HideAlertAfterDelay(float v)
    {
        yield return new WaitForSeconds(v);
        _alertPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (_loginRoutine != null)
        {
            StopCoroutine(_loginRoutine);
        }
    }
}