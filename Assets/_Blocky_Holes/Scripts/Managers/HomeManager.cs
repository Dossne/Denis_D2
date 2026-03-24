using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class HomeManager : MonoBehaviour
    {
        [Header("HomeManager References")]
        [SerializeField] private SpriteRenderer holeSpriteRenderer = null;


        private void Start()
        {
            //Init player data
            if (!PlayerDataHandler.HasData())
            {
                PlayerData playerData = new PlayerData();
                playerData.SoundState = 1;
                playerData.MusicState = 1;
                playerData.CurrentLevel = 1;
                playerData.WatchedTutorial = 0;
                playerData.CurrentRewardIndex = 0;
                playerData.UserName = string.Empty;
                playerData.TimeOfLastReward = string.Empty;
                playerData.TotalCoins = ServicesManager.Instance.CoinManager.InitialCoins;
                playerData.ListUnlockedCharacter = new List<string>();

                //Load free character
                foreach (CharacterInforController characterInfor in ServicesManager.Instance.CharacterContainer.CharacterInforControllers)
                {
                    if (characterInfor.CharacterPrice == 0)
                    {
                        playerData.SelectedCharacterIndex = characterInfor.SequenceNumber;
                        playerData.ListUnlockedCharacter.Add(characterInfor.CharacterName);
                    }
                }

                PlayerDataHandler.SaveData(playerData);
            }

            Application.targetFrameRate = 60;
            ViewManager.Instance.OnShowView(ViewType.HOME_VIEW);

            //Setup character
            CharacterInforController charControl = ServicesManager.Instance.CharacterContainer.CharacterInforControllers[ServicesManager.Instance.CharacterContainer.SelectedCharacterIndex];
            holeSpriteRenderer.sprite = charControl.HoleSprite;
        }
    }
}
