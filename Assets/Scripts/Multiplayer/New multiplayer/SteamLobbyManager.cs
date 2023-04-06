using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Multiplayer_Scripts.New_multiplayer;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Steamworks;
using Steamworks.Data;
using TMPro;
using UnityEngine.SceneManagement;

public class SteamLobbyManager : MonoBehaviour
{

    #region Declared variables and Feilds 

        public static Lobby currentLobby;
        public static bool UserInLobby;
    
        public UnityEvent OnLobbyCreated;
        public UnityEvent OnLobbyJoined;
        public UnityEvent OnLobbyLeave;

        public GameObject InLobbyFriend;
        public Transform content;

        public Dictionary<SteamId, GameObject> inLobby = new Dictionary<SteamId, GameObject>();

    #endregion

    private void Start()
    {
        DontDestroyOnLoad(this);
        
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallBack;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnChatMessage += OnChatMessage;
        SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnected;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberDisconnected;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequest;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
    }
    
    
    
    
    void OnLobbyInvite(Friend friend, Lobby lobby)
    {
        Debug.Log($"{friend.Name} invited you to his lobby.");
    }

    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId id)
    {
        
    }

    private async void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        Debug.Log($"{friend.Name} joined the lobby");

        GameObject obj = Instantiate(InLobbyFriend, content);
        obj.GetComponentInChildren<TMP_Text>().text = friend.Name;
        obj.GetComponentInChildren<RawImage>().texture = await SteamFriendsManager.GetTextureFromSteamIdAsync(friend.Id);
        inLobby.Add(friend.Id, obj);
    }

    void OnLobbyMemberDisconnected(Lobby lobby, Friend friend)
    {
        Debug.Log($"{friend.Name} left the Lobby");
        Debug.Log($"New lobby owner is {currentLobby.Owner}");

        if (inLobby.ContainsKey(friend.Id))
        {
            Destroy(inLobby[friend.Id]);
            inLobby.Remove(friend.Id);
        }
    }

    void OnChatMessage(Lobby lobby, Friend friend, string message)
    {
        Debug.Log($"Incoming chat message From Friend {friend.Name} : {message}");
    }

    async void OnGameLobbyJoinRequest(Lobby joinedLobby, SteamId id)
    {
        RoomEnter joinedLobbySuccess = await joinedLobby.Join();

        if (joinedLobbySuccess != RoomEnter.Success)
            Debug.Log($"failed to join : {joinedLobbySuccess}");

        else
            currentLobby = joinedLobby;
    }

    void OnLobbyCreatedCallBack(Result result, Lobby lobby)
    {
        if(result != Result.OK)
            Debug.Log($"lobby creation result failed : {result}");
        else
        {
            OnLobbyCreated.Invoke();
            Debug.Log("lobby created successfully");
        }
    }
    
    async void OnLobbyEntered(Lobby lobby)
    {
        Debug.Log("Client joined the lobby");
        UserInLobby = true;
        foreach (var user in inLobby.Values)
        {
            Destroy(user);
        }
        inLobby.Clear();

        GameObject obj = Instantiate(InLobbyFriend, content);
        obj.GetComponentInChildren<TMP_Text>().text = SteamClient.Name;
        obj.GetComponentInChildren<RawImage>().texture = await SteamFriendsManager.GetTextureFromSteamIdAsync(SteamClient.SteamId);

        inLobby.Add(SteamClient.SteamId, obj);

        foreach (var friend in currentLobby.Members)
        {
            if (friend.Id != SteamClient.SteamId)
            {
                GameObject obj2 = Instantiate(InLobbyFriend, content);
                obj2.GetComponentInChildren<TMP_Text>().text = friend.Name;
                obj2.GetComponentInChildren<RawImage>().texture = await SteamFriendsManager.GetTextureFromSteamIdAsync(friend.Id);

                inLobby.Add(friend.Id, obj2);
            }
        }
        OnLobbyJoined.Invoke();
    }
    
    public async void CreateLobbyAsync()
    {
        bool result = await CreateLobby();
        if (!result)
        {
            //Invoke and error message
            Debug.Log("Error lobby not created successfully");
        }
    }
    
    public static async Task<bool> CreateLobby()
    {
        try
        {
            var createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync();
            if (!createLobbyOutput.HasValue)
            {
                Debug.Log("Lobby created but not correctly instantiated.");
                return false;
            }
            currentLobby = createLobbyOutput.Value;

            currentLobby.SetPublic();
            //currentLobby.SetPrivate();
            currentLobby.SetJoinable(true);

            return true;
        }
        catch(System.Exception exception)
        {
            Debug.Log("Failed to create multiplayer lobby : " + exception);
            return false;
        }
    }

    public static async Task JoinLobby(SteamId hostId)
    {
        var lobbyJoined = await SteamMatchmaking.JoinLobbyAsync(hostId);

        currentLobby = lobbyJoined.Value;
    }

    public void LeaveLobby()
    {
        try
        {
            UserInLobby = false;
            currentLobby.Leave();
            OnLobbyLeave.Invoke();
            foreach (var user in inLobby.Values)
            {
                Destroy(user);
            }
            inLobby.Clear();
        }
        catch
        {

        }
    }

    public void ReadyButton()
    {
        currentLobby.SetMemberData("Ready", "true");
    }

    public void NotReadyButton()
    {
        currentLobby.SetMemberData("Ready", "true");
    }

     public void StartGameFromLobby()
    {
        bool allready = true;
        if (SteamClient.SteamId == currentLobby.Owner.Id)
        {
            Debug.Log("host attempted to start game");
            currentLobby.SetGameServer(currentLobby.Owner.Id);

            foreach (Friend friend in currentLobby.Members)
            {
                string status = currentLobby.GetMemberData(friend, "Ready");
                Debug.Log($"{friend.Name} is: {status}");

                if (status != "true")
                {
                    allready = false;
                }
            }

            if (allready == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
    
   
}
