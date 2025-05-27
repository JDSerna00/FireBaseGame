using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;

public class PasswordValidator : MonoBehaviour
{
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_Text errorText;
    public Toggle showPasswordToggle;
    public Button registerButton;

    [Header("Settings")]
    public int minUsernameLength = 3;
    public int maxUsernameLength = 20;
    public int minPasswordLength = 8;
    public string allowedUsernameChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
    public Color errorColor = Color.red;
    public Color warningColor = Color.yellow;
    public Color validColor = Color.green;

    void Start()
    {
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        confirmPasswordInput.contentType = TMP_InputField.ContentType.Password;

        usernameInput.onValueChanged.AddListener(ValidateAll);
        emailInput.onValueChanged.AddListener(ValidateAll);
        passwordInput.onValueChanged.AddListener(ValidateAll);
        confirmPasswordInput.onValueChanged.AddListener(ValidateAll);
        showPasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility);

        registerButton.interactable = false;  
    }

    private void TogglePasswordVisibility(bool show)
    {
        passwordInput.contentType = show ?
           TMP_InputField.ContentType.Standard :
           TMP_InputField.ContentType.Password;

        confirmPasswordInput.contentType = show ?
            TMP_InputField.ContentType.Standard :
            TMP_InputField.ContentType.Password;

        // Refresh the input fields to apply changes
        passwordInput.ForceLabelUpdate();
        confirmPasswordInput.ForceLabelUpdate();
    }

    private void ValidateAll(string input)
    {
        string usernameError = ValidateUsername(usernameInput.text);
        string emailError = ValidateEmail(emailInput.text);
        string passwordError = ValidatePassword();

        // Combine all errors
        if (!string.IsNullOrEmpty(usernameError))
        {
            errorText.text = usernameError;
            errorText.color = errorColor;
            registerButton.interactable = false;
        }
        else if (!string.IsNullOrEmpty(emailError))
        {
            errorText.text = emailError;
            errorText.color = errorColor;
            registerButton.interactable = false;
        }
        else if (!string.IsNullOrEmpty(passwordError))
        {
            errorText.text = passwordError;
            errorText.color = passwordInput.text.Length >= minPasswordLength ? warningColor : errorColor;
            registerButton.interactable = false;
        }
        else
        {
            errorText.text = "All fields are valid!";
            errorText.color = validColor;
            registerButton.interactable = true;
        }
    }

    private string ValidateUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
            return "Username cannot be empty";

        if (username.Length < minUsernameLength)
            return $"Username must be at least {minUsernameLength} characters";

        if (username.Length > maxUsernameLength)
            return $"Username cannot exceed {maxUsernameLength} characters";

        if (username.Any(c => !allowedUsernameChars.Contains(c.ToString())))
            return "Username contains invalid characters";

        return string.Empty;
    }

    private string ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return "Email cannot be empty";

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address != email)
                return "Invalid email format";
        }
        catch
        {
            return "Invalid email format";
        }

        return string.Empty;
    }

    private string ValidatePassword()
    {
        if (string.IsNullOrEmpty(passwordInput.text))
            return "Password cannot be empty";

        if (passwordInput.text.Length < minPasswordLength)
            return $"Password must be at least {minPasswordLength} characters";

        if (passwordInput.text != confirmPasswordInput.text)
            return "Passwords do not match";

        return string.Empty;
    }

    public bool ArePasswordsValid()
    {
        return passwordInput.text == confirmPasswordInput.text &&
               !string.IsNullOrEmpty(passwordInput.text) &&
               passwordInput.text.Length >= minPasswordLength;
    }
}
