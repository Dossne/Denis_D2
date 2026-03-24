using UnityEngine;
using UnityEngine.UI;
using System;

namespace ClawbearGames
{
    public class DailyRewardItemController : MonoBehaviour
    {
        [Header("Daily Reward Item References")]
        [SerializeField] private Text rewardDayTxt = null;
        [SerializeField] private Text rewardedCoinTxt = null;
        [SerializeField] private GameObject claimedPanel = null;

        /// <summary>
        /// Setup this daily reward item.
        /// </summary>
        /// <param name="dayType"></param>
        /// <param name="rewardedCoins"></param>
        public void OnSetup(DayType dayType, int rewardedCoins)
        {
            rewardDayTxt.text = ("DAY " + dayType.ToString().Split('_')[1]).ToUpper();
            rewardedCoinTxt.text = rewardedCoins.ToString();
        }


        /// <summary>
        /// Active or deactive claimed panel of this item.
        /// </summary>
        public void HandleClaimedPanel()
        {
            int itemIndex = int.Parse(rewardDayTxt.text[rewardDayTxt.text.Length - 1].ToString()) - 1;
            int savedDailyRewardIndex = PlayerDataHandler.GetCurrentRewardIndex();
            claimedPanel.SetActive((itemIndex < savedDailyRewardIndex) ? true : false);
        }
    }
}
