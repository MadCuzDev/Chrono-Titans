using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Search;

namespace Multiplayer_Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [SerializeField] private GameObject Panel_menu, Panel_lobby;
        [SerializeField] private GameObject chatPanel, textObject;
        [SerializeField] private TMP_InputField inputField;

        [SerializeField] private GameObject PlayerFeildBox, playerCardPrefab;
        [SerializeField] private GameObject readyButton, NotreadyButton, startButton;

        public Dictionary<ulong, GameObject> playerInfo = new Dictionary<ulong, GameObject>();

        [SerializeField] private int maxMessages = 20;

        private List<Message> messageList = new List<Message>();
        
        public bool connceted;
        public bool inGame;
        public bool isHost;
        public ulong myClientId;
        
        private void Awake()
        {
            if (instance != null)
                Destroy(this);
            else
                instance = this;
        }

        private void Update()
        {
            
            if(inputField.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (inputField.text == " ")
                    {
                        inputField.text = "";
                        inputField.DeactivateInputField();
                        return;
                    }
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    NetworkTransmission.instance.WishToSendAChatServerRPC(inputField.text, myClientId);
                    inputField.text = "";
                }
            }
            else
            {
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    inputField.ActivateInputField();
                    inputField.text = " ";
                }
            }
        }

        public class Message
        {
            public string text;
            public TMP_Text textObject;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SendMessageToChat(string _text, ulong _from, bool _server)
        {
            if (messageList.Count >= maxMessages)
            {
                Destroy(messageList[0].textObject.gameObject);
                messageList.Remove(messageList[0]);
            }

            Message newMessage = new Message();
            string _name = "Server";
            
            if (!_server)
            {
                if (playerInfo.ContainsKey(_from))
                {
                    _name = playerInfo[_from].GetComponent<PlayerInfo>().steamName;
                }
            }

            newMessage.text = _name + ":" + _text;

            GameObject newText = Instantiate(textObject, chatPanel.transform);
            newMessage.textObject = newText.GetComponent<TMP_Text>();
            newMessage.textObject.text = newMessage.text;

            messageList.Add(newMessage);
        }

        public void ClearChat()
        {
            messageList.Clear();
            GameObject[] chat = GameObject.FindGameObjectsWithTag("ChatMessage");
            foreach (GameObject chit in chat)
            {
                Destroy(chit);
            }
            Debug.Log("Clearing Chat");
        }
        
        public void HostCreated()
        {
            Panel_menu.SetActive(false);
            Panel_lobby.SetActive(true);
            isHost = true;
            connceted = true;
        }

        public void ConnectedAsClient()
        {
            Panel_menu.SetActive(false);
            Panel_lobby.SetActive(true);
            isHost = false;
            connceted = true;
        }

        public void Disconnected()
        {
            playerInfo.Clear();
            GameObject[] playercards = GameObject.FindGameObjectsWithTag("PlayerCard");
            foreach (GameObject card in playercards)
            {
                Destroy(card);
            }
            
            Panel_menu.SetActive(true);
            Panel_lobby.SetActive(false);
            isHost = false;
            connceted = false;
        }

        public void AddPlayerToDictionary(ulong clientId, string steamName, ulong steamId)
        {
            if (!playerInfo.ContainsKey(clientId))
            {
                PlayerInfo pInfo = Instantiate(playerCardPrefab,PlayerFeildBox.transform).GetComponent<PlayerInfo>();
                pInfo.steamId = steamId;
                pInfo.steamName = steamName;
                playerInfo.Add(clientId, pInfo.gameObject);
            }
        }

        public void UpdateClients()
        {
            foreach (KeyValuePair<ulong, GameObject> player in playerInfo)
            {
                ulong steamId = player.Value.GetComponent<PlayerInfo>().steamId;
                string steamName = player.Value.GetComponent<PlayerInfo>().steamName;
                ulong clientId = player.Key;
                
                NetworkTransmission.instance.UpdateClientsPlayerInfoClientRPC(steamId, steamName, clientId);
            }
        }

        public void RemovePlayerFromDictionary(ulong steamId)
        {
            GameObject value = null;
            ulong key = 100;
            foreach(KeyValuePair<ulong,GameObject> player in playerInfo)
            {
                if (player.Value.GetComponent<PlayerInfo>().steamId == steamId)
                {
                    value = player.Value;
                    key = player.Key;
                }
            }

            if (key != 100)
            {
                playerInfo.Remove(key);
            }

            if (value != null)
            {
                Destroy(value);
            }
        }

        public void ReadyButton(bool ready)
        {
            NetworkTransmission.instance.IsTheClientReadyServerRPC(ready, myClientId);
        }
        
        public bool CheckIfPlayersAreReady()
        {
            bool ready = false;

            foreach(KeyValuePair<ulong,GameObject> player in playerInfo)
            {
                if (!player.Value.GetComponent<PlayerInfo>().isReady)
                {
                    startButton.SetActive(false);
                    return false;
                }
                else
                {
                    startButton.SetActive(true);
                    ready = true;
                }
            }

            return ready;
        }
        
        public void Quit()
        {
            Application.Quit();
        }
    }
}



























