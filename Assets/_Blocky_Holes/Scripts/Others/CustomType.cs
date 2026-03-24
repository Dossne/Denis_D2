using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{

    #region --------------------Ingame Enums
    public enum IngameState
    {
        Ingame_Playing = 0,
        Ingame_Revive = 1,
        Ingame_GameOver = 2,
        Ingame_CompleteLevel = 3,
    }

    public enum PlayerState
    {
        Player_Prepare = 0,
        Player_Living = 1,
        Player_Died = 2,
        Player_CompletedLevel = 3,
    }


    public enum ItemType
    {
        Coin = 0,
        Magnet = 1,
        shield = 2,
    }


    public enum CornerType
    {
        Left_Corner = 0,
        Right_Corner = 1,
    }

    public enum ObstacleType
    {
        Spike_Obstacle = 0,
        Saw_Obstacle = 1,
        Spinner_Obstacle = 2,
        Swinger_Obstacle = 3,
    }

    public enum BulletType
    {
        Enemy_Bullet = 0,
        Player_Bullet = 1,
    }

    public enum BoosterType
    {
        Damage_Booster = 0,
        Health_Booster = 1,
        Range_Booster = 2,
        Accuracy_Booster = 3,
        Firerate_Booster = 4,
    }


    public enum PlayerTowerType
    {
        Player_Tower_0 = 0,
        Player_Tower_1 = 1,
        Player_Tower_2 = 2,
        Player_Tower_3 = 3,
        Player_Tower_4 = 4,
        Player_Tower_5 = 5,
        Player_Tower_6 = 6,
        Player_Tower_7 = 7,
        Player_Tower_8 = 8,
        Player_Tower_9 = 9,
        Player_Tower_10 = 10,
        Player_Tower_11 = 11,
        Player_Tower_12 = 12,
        Player_Tower_13 = 13,
        Player_Tower_14 = 14,
        Player_Tower_15 = 15,
        Player_Tower_16 = 16,
        Player_Tower_17 = 17,
        Player_Tower_18 = 18,
        Player_Tower_19 = 19,
    }

    public enum EnemyTowerType
    {
        Enemy_Tower_0 = 0,
        Enemy_Tower_1 = 1,
        Enemy_Tower_2 = 2,
        Enemy_Tower_3 = 3,
        Enemy_Tower_4 = 4,
        Enemy_Tower_5 = 5,
        Enemy_Tower_6 = 6,
        Enemy_Tower_7 = 7,
        Enemy_Tower_8 = 8,
        Enemy_Tower_9 = 9,
        Enemy_Tower_10 = 10,
        Enemy_Tower_11 = 11,
        Enemy_Tower_12 = 12,
        Enemy_Tower_13 = 13,
        Enemy_Tower_14 = 14,
        Enemy_Tower_15 = 15,
        Enemy_Tower_16 = 16,
        Enemy_Tower_17 = 17,
        Enemy_Tower_18 = 18,
        Enemy_Tower_19 = 19,
    }


    public enum PlayerTankType
    {
        Player_Tank_0 = 0,
        Player_Tank_1 = 1,
        Player_Tank_2 = 2,
        Player_Tank_3 = 3,
        Player_Tank_4 = 4,
        Player_Tank_5 = 5,
        Player_Tank_6 = 6,
        Player_Tank_7 = 7,
        Player_Tank_8 = 8,
        Player_Tank_9 = 9,
        Player_Tank_10 = 10,
        Player_Tank_11 = 11,
        Player_Tank_12 = 12,
        Player_Tank_13 = 13,
        Player_Tank_14 = 14,
        Player_Tank_15 = 15,
        Player_Tank_16 = 16,
        Player_Tank_17 = 17,
        Player_Tank_18 = 18,
        Player_Tank_19 = 19,
    }

    public enum EnemyTankType
    {
        Enemy_Tank_0 = 0,
        Enemy_Tank_1 = 1,
        Enemy_Tank_2 = 2,
        Enemy_Tank_3 = 3,
        Enemy_Tank_4 = 4,
        Enemy_Tank_5 = 5,
        Enemy_Tank_6 = 6,
        Enemy_Tank_7 = 7,
        Enemy_Tank_8 = 8,
        Enemy_Tank_9 = 9,
        Enemy_Tank_10 = 10,
        Enemy_Tank_11 = 11,
        Enemy_Tank_12 = 12,
        Enemy_Tank_13 = 13,
        Enemy_Tank_14 = 14,
        Enemy_Tank_15 = 15,
        Enemy_Tank_16 = 16,
        Enemy_Tank_17 = 17,
        Enemy_Tank_18 = 18,
        Enemy_Tank_19 = 19,
    }


    public enum BossType
    {
        Boss_0 = 0,
        Boss_1 = 1,
        Boss_2 = 2,
        Boss_3 = 3,
        Boss_4 = 4,
        Boss_5 = 5,
        Boss_6 = 6,
        Boss_7 = 7,
        Boss_8 = 8,
        Boss_9 = 9,
        Boss_10 = 10,
        Boss_11 = 11,
        Boss_12 = 12,
        Boss_13 = 13,
        Boss_14 = 14,
        Boss_15 = 15,
    }


    public enum DayType
    {
        DAY_1 = 0,
        DAY_2 = 1,
        DAY_3 = 2,
        DAY_4 = 3,
        DAY_5 = 4,
        DAY_6 = 5,
        DAY_7 = 6,
        DAY_8 = 7,
        DAY_9 = 8,
    }

    #endregion



    #region --------------------Ads Enums
    public enum BannerAdType
    {
        NONE = 0,
        ADMOB = 1,
        UNITY = 2,
    }

    public enum InterstitialAdType
    {
        UNITY = 0,
        ADMOB = 1,
    }


    public enum RewardedAdType
    {
        UNITY = 0,
        ADMOB = 1,
    }


    public enum RewardedAdTarget
    {
        GET_FREE_COINS = 0,
        GET_DOUBLE_COIN = 1,
        REVIVE_PLAYER = 2,
        ADD_MORE_SIZE = 3,
    }

    #endregion



    #region --------------------View Enums
    public enum ViewType
    {
        HOME_VIEW = 0,
        LEADERBOARD_VIEW = 1,
        DAILY_REWARD_VIEW = 2,
        LOADING_VIEW = 3,
        INGAME_VIEW = 4,
        REVIVE_VIEW = 5,
        ENDGAME_VIEW = 6,
        CHARACTER_VIEW = 7,
    }

    #endregion



    #region --------------------Classes


    [System.Serializable]
    public class DailyRewardConfiguration
    {
        [SerializeField] private DayType dayType = DayType.DAY_1;

        /// <summary>
        /// the day type of this DailyRewardItem.
        /// </summary>
        public DayType DayType => dayType;


        [SerializeField] private int coinAmount = 0;


        /// <summary>
        /// The amount of coins reward to player.
        /// </summary>
        public int CoinAmount => coinAmount;
    }


    [System.Serializable]
    public class InterstitialAdConfiguration
    {
        [SerializeField] private IngameState ingameStateWhenShowingAd = IngameState.Ingame_CompleteLevel;
        public IngameState IngameStateWhenShowingAd => ingameStateWhenShowingAd;

        [SerializeField] private int ingameStateAmountWhenShowingAd = 3;
        public int IngameStateAmountWhenShowingAd => ingameStateAmountWhenShowingAd;


        [SerializeField] private float delayTimeWhenShowingAd = 2f;
        public float DelayTimeWhenShowingAd => delayTimeWhenShowingAd;

        [SerializeField] private List<InterstitialAdType> listInterstitialAdType = new List<InterstitialAdType>();
        public List<InterstitialAdType> ListInterstitialAdType => listInterstitialAdType;
    }


    public class LeaderboardParams
    {
        public string Username { private set; get; }
        public void SetUsername(string username)
        {
            Username = username;
        }

        public int Level { private set; get; }
        public void SetLevel(int level)
        {
            Level = level;
        }
    }

    public class LeaderboardComparer : IComparer<LeaderboardParams>
    {
        public int Compare(LeaderboardParams dataX, LeaderboardParams dataY)
        {
            if (dataX.Level < dataY.Level)
                return 1;
            if (dataX.Level > dataY.Level)
                return -1;
            else
                return 0;
        }
    }

    #endregion
}
