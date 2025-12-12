using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientSingletonPrefab;
    [SerializeField] private HostSingleton hostSingletonPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            // Launch dedicated server
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostSingletonPrefab);
            hostSingleton.CreateHost();
            
            ClientSingleton clientSingleton = Instantiate(clientSingletonPrefab);
            bool isAuthenticated = await clientSingleton.CreateClient();
          
            if (isAuthenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
            else
            {
                // go to main menu
            }
            
        }
    }

}
