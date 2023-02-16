using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

namespace Multiplayer_Scripts
{
    public class NetworkTransmission : NetworkBehaviour
    {
        public static NetworkTransmission instance;

        private void Awake()
        {
            if (instance != null)
                Destroy(this);
            else
                instance = this;
        }

        [ServerRpc(RequireOwnership = false)]
        public void WishToSendAChatServerRPC(string _message, ulong _from)
        {
            ChatFromServerClientRPC(_message, _from);
        }

        [ClientRpc]
        private void ChatFromServerClientRPC(string _message, ulong _from)
        {
            GameManager.instance.SendMessageToChat(_message, _from, false);
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddMeToDictionaryServerRPC(ulong steamId, string steamName, ulong clientId)
        {
            GameManager.instance.SendMessageToChat($"{steamName} has joined", clientId, true);
            GameManager.instance.AddPlayerToDictionary(clientId, steamName, steamId);
            GameManager.instance.UpdateClients();
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemoveMeFromDictionaryServerRPC(ulong steamId)
        {
            RemovePlayerFromDictionaryClientRPC(steamId);
        }

        [ClientRpc]
        private void RemovePlayerFromDictionaryClientRPC(ulong steamId)
        {
            Debug.Log("Removing Client");
            GameManager.instance.RemovePlayerFromDictionary(steamId);
        }

        [ClientRpc]
        public void UpdateClientsPlayerInfoClientRPC(ulong steamId, string steamName, ulong clientId)
        {
             GameManager.instance.AddPlayerToDictionary(clientId, steamName, steamId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void IsTheClientReadyServerRPC(bool ready, ulong clientId)
        {
            AClientMightBeReadyClientRPC(ready, clientId);
        }

        [ClientRpc]
        private void AClientMightBeReadyClientRPC(bool ready, ulong clientId)
        {
            foreach (KeyValuePair<ulong, GameObject> player in GameManager.instance.playerInfo)
            {
                if (player.Key == clientId)
                {
                    player.Value.GetComponent<PlayerInfo>().isReady = ready;
                    player.Value.GetComponent<PlayerInfo>().readyImage.SetActive(ready);
                    if (NetworkManager.Singleton.IsHost)
                    {
                        Debug.Log(GameManager.instance.CheckIfPlayersAreReady()); 
                    }
                }
            }
        }
    }
}





























