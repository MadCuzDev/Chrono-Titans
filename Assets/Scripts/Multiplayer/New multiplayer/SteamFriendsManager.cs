using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Multiplayer_Scripts.New_multiplayer
{
    public class SteamFriendsManager : MonoBehaviour
    {
        #region declaring variables and properties

            public RawImage profilePic;
            public TMP_Text playername;

            public Transform friendsContent;
            public GameObject friendObj;

        #endregion

        async void Start()
        {
            if(!SteamClient.IsValid) return;

            playername.text = SteamClient.Name;
            InitFriendsAsync();
            var img = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
            profilePic.texture = GetTextureFromImage(img.Value);
            
        }
        
        public static Texture2D GetTextureFromImage(Steamworks.Data.Image image)
        {
            Texture2D texture = new Texture2D((int)image.Width, (int)image.Height);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var p = image.GetPixel(x, y);
                    texture.SetPixel(x, (int)image.Height - y, new Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
                }
            }
            texture.Apply();
            return texture;
        }


        public void InitFriendsAsync()
        {
            foreach (var friend in SteamFriends.GetFriends())
            {
                GameObject f = Instantiate(friendObj, friendsContent);
                f.GetComponentInChildren<TMP_Text>().text = friend.Name;
                f.GetComponent<FriendObject>().steamid = friend.Id;
                AssingFriendImage(f, friend.Id);
            }
        }
        
        public async void AssingFriendImage(GameObject f, SteamId id)
        {
            var img = await SteamFriends.GetLargeAvatarAsync(id);
            f.GetComponentInChildren<RawImage>().texture = GetTextureFromImage(img.Value);
        }
        
        public static async System.Threading.Tasks.Task<Texture2D> GetTextureFromSteamIdAsync(SteamId id)
        {
            var img = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
            Steamworks.Data.Image image = img.Value;
            Texture2D texture = new Texture2D((int)image.Width, (int)image.Height);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var p = image.GetPixel(x, y);
                    texture.SetPixel(x, (int)image.Height - y, new Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
                }
            }
            texture.Apply();
            return texture;
        }
    }
}