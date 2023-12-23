using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ClientOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private UnityTransport transport;
    [SerializeField] private TextMeshProUGUI message;

    private void Awake()
    {
        submitButton.onClick.AddListener(() =>
        {
            string address = inputField.text;
            address = address.Substring(0, 12);
            transport.ConnectionData.Address = address;
            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        });
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        message.text = "Connected to host!";
        message.gameObject.SetActive(true);
    }
}
