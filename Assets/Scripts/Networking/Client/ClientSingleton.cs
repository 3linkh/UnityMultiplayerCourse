using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{

    private static ClientSingleton instance;

    public ClientGameManager GameManager { get;  private set; } = new ClientGameManager();

    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindObjectOfType<ClientSingleton>();

            if (instance == null)
            {
                Debug.LogError("No ClientSingleton instance found in the scene.");
                return null;
            }
            return instance;
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();

        return await GameManager.InitAsync();
    }
    
    private void OnDestroy()
    {
        GameManager?.Dispose();
    }

}
