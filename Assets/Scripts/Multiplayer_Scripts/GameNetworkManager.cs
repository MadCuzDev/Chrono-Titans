using UnityEngine;
using Unity.Netcode;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;

namespace Multiplayer_Scripts
{
    public class GameNetworkManager : MonoBehaviour
    {
        public static GameNetworkManager instance { get; private set; } = null;

        private FacepunchTransport transport = null;

        public Lobby? currentLobby { get; private set; } = null;

        public ulong hostId;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else { Destroy(gameObject); return; }

        }

        #region start and destroy
        private void Start()
        {
            transport = GetComponent<FacepunchTransport>();

            SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
            SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;
        }

        private void OnDestroy()
        {
            SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
            SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

            if (NetworkManager.Singleton == null)
                return;

            NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectedCallback;

        }
        #endregion

        private void OnApplicationQuit()
        {
            Disconnected();
        }

    //when an invite is accepted 
        private async void SteamFriends_OnGameLobbyJoinRequested(Lobby lobby, SteamId _steamId)
        {
            RoomEnter joined = await lobby.Join();
            if (joined != RoomEnter.Success)
                Debug.Log("failed to create lobby");
            else
            {
                currentLobby = lobby;
                GameManager.instance.ConnectedAsClient();

                Debug.Log("lobby Joined");
            }


        }

        private void SteamMatchmaking_OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId)
        {
            Debug.Log("Lobby Created");
            GameManager.instance.SendMessageToChat($"Lobby was created", NetworkManager.Singleton.LocalClientId, true);
            
        }

    //receive Invite
        private void SteamMatchmaking_OnLobbyInvite(Friend steamId, Lobby lobby)
        {
            Debug.Log($"Invite to join from{steamId.Name}");
        }

        private void SteamMatchmaking_OnLobbyMemberLeave(Lobby lobby, Friend steamId)
        {
            Debug.Log("member leave");
            GameManager.instance.SendMessageToChat($"{steamId.Name} has left", steamId.Id, true);
            NetworkTransmission.instance.RemoveMeFromDictionaryServerRPC(steamId.Id);
        }

        private void SteamMatchmaking_OnLobbyMemberJoined(Lobby lobby, Friend steamId)
        {
            Debug.Log("member join");
        }

        private void SteamMatchmaking_OnLobbyEntered(Lobby lobby)
        {
            if (NetworkManager.Singleton.IsHost)
                return;

            StartClient(currentLobby.Value.Owner.Id);
        }

        private void SteamMatchmaking_OnLobbyCreated(Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.Log("lobby was not created");
                return;
            }

            lobby.SetPublic();
            lobby.SetJoinable(true);
            lobby.SetGameServer(lobby.Owner.Id);
            Debug.Log($"lobby created{lobby.Owner.Name}");
            NetworkTransmission.instance.AddMeToDictionaryServerRPC(SteamClient.SteamId, lobby.Owner.Name, NetworkManager.Singleton.LocalClientId);
        }

        public void Disconnected()
        {
            currentLobby?.Leave();
            if (NetworkManager.Singleton == null)
                return;

            if (NetworkManager.Singleton.IsHost)
                NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;

            else
                NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;

            NetworkManager.Singleton.Shutdown(true);
            
            GameManager.instance.ClearChat();
            GameManager.instance.Disconnected();
            
            Debug.Log("disconnected");
        }

        public async void StartHost(int maxPlayers)
        {
            NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
            NetworkManager.Singleton.StartHost();

            GameManager.instance.myClientId = NetworkManager.Singleton.LocalClientId;
            
            currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);
        }

        private void Singleton_OnServerStarted()
        {
            Debug.Log("Host started");
            
            GameManager.instance.HostCreated();
        }

        public void StartClient(SteamId steamId)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectedCallback;

            transport.targetSteamId = steamId;

            GameManager.instance.myClientId = NetworkManager.Singleton.LocalClientId;
            
            if (NetworkManager.Singleton.StartClient())
                Debug.Log("Client has started");
        }

        
        
        private void Singleton_OnClientDisconnectedCallback(ulong clientId)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectedCallback;
            if(clientId == 0)
            {
                Disconnected();
            }
        }

        private void Singleton_OnClientConnectedCallback(ulong clientId)
        {
            NetworkTransmission.instance.AddMeToDictionaryServerRPC(SteamClient.SteamId, SteamClient.Name, clientId);
            GameManager.instance.myClientId = clientId;
            NetworkTransmission.instance.IsTheClientReadyServerRPC(false, clientId);
            Debug.Log($"Client has connected : AnotherFakeSteamName");
        }
    }
}