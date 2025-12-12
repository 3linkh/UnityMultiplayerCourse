using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInputField;
    public async void StartHost()
    {
        await HostSingleton.Instance.HostGameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        string joinCode = joinCodeInputField.text;
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeInputField.text);
    }

}
