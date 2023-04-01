using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button hostBtn;

    public void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            HideNetworkUI();
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            HideNetworkUI();
        });
        hostBtn.onClick.AddListener(() =>
        {
            HideNetworkUI();
            NetworkManager.Singleton.StartHost();
        });
    }

    private void HideNetworkUI ()
    {
        serverBtn.gameObject.SetActive(false);
        clientBtn.gameObject.SetActive(false);
        hostBtn.gameObject.SetActive(false);
    }
}
