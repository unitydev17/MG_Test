using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    public static event Action OnStart; 
    
    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _clientBtn;
    [SerializeField] private Button _serverBtn;

    private void Awake()
    {
        _hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            OnStart?.Invoke();
            Close();
        });
        _clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            Close();
        });
        _serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            OnStart?.Invoke();
            Close();
        });
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}