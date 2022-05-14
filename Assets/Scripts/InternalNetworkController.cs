using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FishNet;
using FishNet.Discovery;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

namespace FarmGame
{
    public class InternalNetworkController : MonoBehaviour
    {
        [SerializeField] private bool AutoStartClientOnServer = false;
        
        private NetworkDiscovery networkDiscovery;
        private List<IPEndPoint> endPoints = new();
        private bool serverFound;
        private int maxAttempts = 100;

        private void Start()
        {
            networkDiscovery = FindObjectOfType<NetworkDiscovery>();
            if (networkDiscovery == null)
            {
                Debug.LogWarning("Network Discovery not found, aborting...");
                return;
            }

            InstanceFinder.ServerManager.OnServerConnectionState += OnServerConnectionStateChange;
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionStateChange;
            networkDiscovery.ServerFoundCallback += ServerFoundCallback;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
                StartServerRequest();
            if (Input.GetKeyDown(KeyCode.O))
                StartClientRequest();
        }

        private void OnDestroy()
        {
            if (FindObjectOfType<NetworkManager>() == null)
                return;
            
            InstanceFinder.ServerManager.OnServerConnectionState -= OnServerConnectionStateChange;
            InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionStateChange;
            networkDiscovery.ServerFoundCallback -= ServerFoundCallback;
        }

        private void StartServerRequest()
        {
            InstanceFinder.ServerManager.StartConnection();
        }

        public void StartServerClient()
        {
            if (AutoStartClientOnServer)
                InstanceFinder.ClientManager.StartConnection();
        }

        private void StartClientRequest()
        {
            Task.Run(ServerSearch);
        }

        private async void ServerSearch()
        {
            serverFound = false;
            int currentAttempts = 0;
            
            while (!serverFound && currentAttempts <= maxAttempts)
            {
                await Task.Delay(250);
                networkDiscovery.StartSearchingForServers();
                await Task.Delay(1000);
                networkDiscovery.StopSearchingForServers();
                currentAttempts++;
            }

            if (currentAttempts > maxAttempts)
            {
                Debug.LogWarning("No server found.");
            }
        }

        private void OnServerConnectionStateChange(ServerConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionStates.Started)
            {
                networkDiscovery.StartAdvertisingServer();
                StartServerClient();
            }
        }

        private void OnClientConnectionStateChange(ClientConnectionStateArgs obj)
        {
            
        }

        private void ServerFoundCallback(IPEndPoint ip)
        {
            serverFound = true;
            string serverAddress = ip.Address.ToString();
            InstanceFinder.ClientManager.StartConnection(serverAddress);
        }
    }
}