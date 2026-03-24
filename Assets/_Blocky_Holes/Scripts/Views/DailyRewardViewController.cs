using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ClawbearGames
{
    public class DailyRewardViewController : BaseViewController
    {
        [SerializeField] private CanvasGroup canvasGroup = null;
        [SerializeField] private RectTransform dailyrewardItemsPanelTrans = null;
        [SerializeField] private RectTransform countingPanelTrans = null;
        [SerializeField] private RectTransform claimButtonTrans = null;
        [SerializeField] private Button freeCoinsButton = null;
        [SerializeField] private Text totalCoinsText = null;
        [SerializeField] private Text rewardCoinText = null;
        [SerializeField] private Text nextRewardTimeText = null;
        [SerializeField] private DailyRewardItemController dailyRewardItemControllerPrefab = null;

        private List<DailyRewardItemController> listDailyRewardItemController = new List<DailyRewardItemController>();

        private void Update()
        {
            //Get time rewmains till next reward
            double timeRemains = ServicesManager.Instance.DailyRewardManager.TimeRemainsTillNextReward();

            //Update the texts and buttons
            if (timeRemains > 0)
            {
                claimButtonTrans.gameObject.SetActive(false);
                countingPanelTrans.gameObject.SetActive(true);
                nextRewardTimeText.text = Utilities.SecondsToTimeFormat(timeRemains);
            }
            else
            {
                countingPanelTrans.gameObject.SetActive(false);
                claimButtonTrans.gameObject.SetActive(true);
            }

            //Update texts
            totalCoinsText.text = ServicesManager.Instance.CoinManager.TotalCoins.ToString();
        }


        /// <summary>
        ////////////////////////////////////////////////// Public Functions
        /// </summary>

        public override void OnShow()
        {
            FadeInCanvasGroup(canvasGroup, 0.75f);
            if (listDailyRewardItemController.Count == 0)
            {
                //Fill the value for the items 
                for (int i = 0; i < ServicesManager.Instance.DailyRewardManager.DailyRewardConfigurations.Length; i++)
                {
                    DailyRewardConfiguration itemConfig = ServicesManager.Instance.DailyRewardManager.DailyRewardConfigurations[i];

                    DailyRewardItemController dailyRewardItemController = Instantiate(dailyRewardItemControllerPrefab, Vector3.zero, Quaternion.identity);
                    dailyRewardItemController.transform.SetParent(dailyrewardItemsPanelTrans);
                    dailyRewardItemController.gameObject.transform.localScale = Vector3.one;
                    dailyRewardItemController.OnSetup(itemConfig.DayType, itemConfig.CoinAmount);
                    listDailyRewardItemController.Add(dailyRewardItemController);
                }
            }


            //Update claimed panel of all items.
            UpdateClaimedPanelOfAllItems();


            //Update texts and buttons
            rewardCoinText.text = ServicesManager.Instance.DailyRewardManager.GetCoinAmountOfCurrentDailyReward().ToString();
            freeCoinsButton.interactable = ServicesManager.Instance.AdManager.IsRewardedAdReady();
        }


        public override void OnClose()
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }



        /// <summary>
        /// Reset the claimed panel of all items.
        /// </summary>
        public void UpdateClaimedPanelOfAllItems()
        {
            foreach (DailyRewardItemController o in listDailyRewardItemController)
            {
                o.HandleClaimedPanel();
            }
        }




        /// <summary>
        ////////////////////////////////////////////////// UI Buttons
        /// </summary>

        public void OnClickClaimButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);

            //Claim the reward of the current daily reward item.
            int rewardedCoins = ServicesManager.Instance.DailyRewardManager.GetCoinAmountOfCurrentDailyReward();
            ServicesManager.Instance.CoinManager.AddTotalCoins(rewardedCoins, 0.25f);
            ServicesManager.Instance.DailyRewardManager.HandleClaimedCurrentDailyReward();
            rewardCoinText.text = ServicesManager.Instance.DailyRewardManager.GetCoinAmountOfCurrentDailyReward().ToString();

            //Update claimed panel of all items
            UpdateClaimedPanelOfAllItems();
        }
        public void OnClickFreeCoinsButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            freeCoinsButton.interactable = false;
            ServicesManager.Instance.AdManager.ShowRewardedAd(RewardedAdTarget.GET_FREE_COINS);
        }

        public void OnClickCloseButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ViewManager.Instance.OnShowView(ViewType.HOME_VIEW);
        }
    }
}
