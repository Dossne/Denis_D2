using ClawbearGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class PlayerDataHandler
    {
        public const string PLAYER_DATA = "CBGAMES_PLAYER_DATA";
        private static int CurrentTotalCoin = -1;
        private static int CurrentRewardIndex = -1;


        /// <summary>
        /// Check if the player data is initialized/saved.
        /// </summary>
        /// <returns></returns>
        public static bool HasData()
        {
            return PlayerPrefs.HasKey(PLAYER_DATA);
        }

        /// <summary>
        /// Convert the given player data object then save to the PlayerPrefs.
        /// </summary>
        /// <param name="playerData"></param>
        public static void SaveData(PlayerData playerData)
        {
            string json = JsonUtility.ToJson(playerData);
            PlayerPrefs.SetString(PLAYER_DATA, json);
            CurrentTotalCoin = playerData.TotalCoins;
            CurrentRewardIndex = playerData.CurrentRewardIndex;
        }





        /// <summary>
        /// Get the current level of the player.
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentLevel()
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            return playerData.CurrentLevel;
        }

        /// <summary>
        /// Update the current level of the player data.
        /// </summary>
        /// <param name="newLevel"></param>
        public static void UpdateCurrentLevel(int newLevel)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            playerData.CurrentLevel = newLevel;
            SaveData(playerData);
        }






        /// <summary>
        /// Get the user name of the player.
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            return playerData.UserName;
        }

        /// <summary>
        /// Update the user name of the player.
        /// </summary>
        /// <param name="userName"></param>
        public static void UpdateUserName(string userName)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            playerData.UserName = userName;
            SaveData(playerData);
        }






        /// <summary>
        /// Get the total coins of the player data.
        /// </summary>
        /// <returns></returns>
        public static int GetTotalCoin()
        {
            if (CurrentTotalCoin < 0)
            {
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
                CurrentTotalCoin = playerData.TotalCoins;
            }
            return CurrentTotalCoin;
        }

        /// <summary>
        /// Update the total coins of the player data.
        /// </summary>
        /// <param name="newCoin"></param>
        public static void UpdateTotalCoin(int newCoin)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            playerData.TotalCoins = newCoin;
            SaveData(playerData);
        }



        /// <summary>
        /// Check if the state of sound is off.
        /// </summary>
        /// <returns></returns>
        public static bool IsSoundOff()
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            return playerData.SoundState == 0 ? true : false;
        }

        /// <summary>
        /// Update the sound state of the player data.
        /// </summary>
        /// <param name="muted"></param>
        public static void UpdateSoundState(bool muted)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            playerData.SoundState = muted ? 0 : 1;
            SaveData(playerData);
        }



        /// <summary>
        /// Check if the state of music is off.
        /// </summary>
        /// <returns></returns>
        public static bool IsMusicOff()
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            return playerData.MusicState == 0 ? true : false;
        }

        /// <summary>
        /// Update the music state of the player data.
        /// </summary>
        /// <param name="muted"></param>
        public static void UpdateMusicState(bool muted)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            playerData.MusicState = muted ? 0 : 1;
            SaveData(playerData);
        }




        /// <summary>
        /// Get the selected character index.
        /// </summary>
        /// <returns></returns>
        public static int GetSelectedCharacterIndex()
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            return playerData.SelectedCharacterIndex;
        }



        /// <summary>
        /// Update the selected character index.
        /// </summary>
        /// <param name="newIndex"></param>
        public static void UpdateSelectedCharacterIndex(int newIndex)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            playerData.SelectedCharacterIndex = newIndex;
            SaveData(playerData);
        }




        /// <summary>
        /// Get the fighterIndex of current daily reward.
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentRewardIndex()
        {
            if (CurrentRewardIndex < 0)
            {
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
                CurrentRewardIndex = playerData.CurrentRewardIndex;
            }
            return CurrentRewardIndex;
        }

        /// <summary>
        /// Update the current reward fighterIndex of the player data.
        /// </summary>
        /// <param name="newIndex"></param>
        public static void UpdateCurrentRewardIndex(int newIndex)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            playerData.CurrentRewardIndex = newIndex;
            CurrentRewardIndex = newIndex;
            SaveData(playerData);
        }






        /// <summary>
        /// Get the time of last reward.
        /// </summary>
        /// <returns></returns>
        public static string GetTimeOfLastReward()
        {
            if (!HasData()) { return string.Empty; }
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            return playerData.TimeOfLastReward;
        }

        /// <summary>
        /// Update time of last reward the player data.
        /// </summary>
        /// <param name="timeOfLastReward"></param>
        public static void UpdateTimeOfLastReward(string timeOfLastReward)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            playerData.TimeOfLastReward = timeOfLastReward;
            SaveData(playerData);
        }




        /// <summary>
        /// Check if the tutorial is watched.
        /// </summary>
        /// <returns></returns>
        public static bool IsWatchedTutorial()
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            return playerData.WatchedTutorial == 1 ? true : false;
        }

        /// <summary>
        /// Update watched tutorial.
        /// </summary>
        public static void UpdateWatchedTutorial()
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            playerData.WatchedTutorial = 1;
            SaveData(playerData);
        }



        /// <summary>
        /// Check if the given character name is unlocked.
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public static bool IsUnlockedCharacter(string characterName)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            return playerData.ListUnlockedCharacter.Contains(characterName);
        }


        /// <summary>
        /// Update the unlocked character of the player data.
        /// </summary>
        /// <param name="characterName"></param>
        public static void UpdateUnlockedCharacter(string characterName)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA));
            if (!IsUnlockedCharacter(characterName)) { playerData.ListUnlockedCharacter.Add(characterName); }
            SaveData(playerData);
        }
    }
}