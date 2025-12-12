using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthenticationState authState { get; private set; } = AuthenticationState.NotAuthenticated;

    public static async Task<AuthenticationState> DoAuth(int maxRetries = 5)
    {
        if (authState == AuthenticationState.Authenticated)
        {
            return authState;
        }

        if (authState == AuthenticationState.Authenticating)
        {
            Debug.LogWarning("Authentication is already in progress, waiting for result...");
            await Authenticating();
            return authState;
        }


        await SignInAnonymously(maxRetries);
        return authState;
    }
    
    private static async Task<AuthenticationState> Authenticating()
    {
        while (authState == AuthenticationState.Authenticating || authState == AuthenticationState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return authState;

    }

    private static async Task SignInAnonymously(int maxRetries)
    {
        authState = AuthenticationState.Authenticating;

        int retries = 0;
        while (authState == AuthenticationState.Authenticating && retries < maxRetries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    authState = AuthenticationState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException e)
            {
                Debug.LogError($"Authentication failed with exception: {e.Message}");
                authState = AuthenticationState.Error;

            }
            catch (RequestFailedException e)
            {
                Debug.LogError($"Request failed with exception: {e.Message}");
                authState = AuthenticationState.Error;

            }

            retries++;
            await Task.Delay(1000); // is milliseconds aka 1 second
        }
        
        if (authState != AuthenticationState.Authenticated)
        {
            Debug.LogError($"player was not signed in after {retries} attempts, timing out");
            authState = AuthenticationState.Timeout;
        }

    }   

}


public enum AuthenticationState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    Timeout
}
