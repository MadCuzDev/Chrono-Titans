using System.Net.Mime;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;

namespace Multiplayer_Scripts.New_multiplayer
{
    public class FriendObject : MonoBehaviour
    {
        public SteamId steamid;
        public bool ready = false;

        public async void Invite()
        {
            if (SteamLobbyManager.UserInLobby)
            {
                SteamLobbyManager.currentLobby.InviteFriend(steamid);
                Debug.Log("Invited " + steamid);
            }
            else
            {
                bool result = await SteamLobbyManager.CreateLobby();
                if (result)
                {
                    SteamLobbyManager.currentLobby.InviteFriend(steamid);
                    Debug.Log("Invited " + steamid + " Created a new lobby");
                }
            }
        }
        
    }
}