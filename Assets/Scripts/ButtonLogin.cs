using UnityEngine;
using Firebase.Auth;
using System.Collections;

public class ButtonLogin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

    }

    private void HandleRegistration()
    {
        string email = "a";
        string password = "a";
        StartCoroutine(RegisterUser(email, password));
    }

    private IEnumerator RegisterUser(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registrationTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registrationTask.IsCompleted);
    }

}
