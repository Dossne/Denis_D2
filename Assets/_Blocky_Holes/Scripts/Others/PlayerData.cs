using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    [System.Serializable]
    public class PlayerData
    {
        public int SoundState = 1;
        public int MusicState = 1;
        public int CurrentLevel = 1;
        public int TotalCoins = 500;
        public int WatchedTutorial = 0;
        public int CurrentRewardIndex = 0;
        public int SelectedCharacterIndex = 0;
        public string UserName = string.Empty;
        public string TimeOfLastReward = string.Empty;
        public List<string> ListUnlockedCharacter = new List<string>();
    }
}