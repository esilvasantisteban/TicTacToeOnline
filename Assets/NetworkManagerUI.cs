using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private GameObject clientOverlay;
    [SerializeField] private UnityTransport unityTransport;

    public void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            HideNetworkUI();
        });
        clientBtn.onClick.AddListener(() =>
        {
            clientOverlay.SetActive(true);
            HideNetworkUI();
        });
        hostBtn.onClick.AddListener(() =>
        {
            HideNetworkUI();
            unityTransport.ConnectionData.Address = GetLocalIpAdress();
            NetworkManager.Singleton.StartHost();
            message.text = "You Local IP is: " + GetLocalIpAdress();
            message.gameObject.SetActive(true);
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        });
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        if (obj == NetworkManager.Singleton.LocalClientId) return;
        message.text = "Opponent connected succesfully!";
    }

    private void HideNetworkUI ()
    {
        gameObject.SetActive(false);
    }

    private string GetLocalIpAdress ()
    {
        IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        foreach(IPAddress adress in addressList)
        {
            if (adress.AddressFamily == AddressFamily.InterNetwork)
                return adress.ToString();
        }
        return null;
    }
}
