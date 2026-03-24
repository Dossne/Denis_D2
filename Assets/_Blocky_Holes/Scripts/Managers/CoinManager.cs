using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class CoinManager : MonoBehaviour
    {
        [Header("Coin Manager Configuration")]
        [SerializeField] private int initialCoins = 0;
        [SerializeField] private int minRewardedCoins = 100;
        [SerializeField] private int maxRewardedCoins = 150;

        public int InitialCoins => initialCoins;
        public int CollectedCoins { private set; get; }


        public int TotalCoins
        {
            private set { PlayerDataHandler.UpdateTotalCoin(value); }
            get { return PlayerDataHandler.GetTotalCoin(); }
        }


        /// <summary>
        /// Set the CollectedCoins by the given amount.
        /// </summary>
        /// <param name="amount"></param>
        public void SetCollectedCoins(int amount)
        {
            CollectedCoins = amount;
        }


        /// <summary>
        /// Get an amount of coins to reward to user.
        /// </summary>
        /// <returns></returns>
        public int GetRewardedCoins()
        {
            return Random.Range(minRewardedCoins, maxRewardedCoins) / 5 * 5;
        }




        /// <summary>
        /// Add an amount of coins to TotalCoins with given delay time.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="delay"></param>
        public void AddTotalCoins(int amount, float delay)
        {
            int startCoins = TotalCoins;
            int endCoins = TotalCoins + amount;
            StartCoroutine(CRUpdateCoins(0, startCoins, endCoins, delay));
        }


        /// <summary>
        /// Add an amount of coins to CollectedCoins with given delay time.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="delay"></param>
        public void AddCollectedCoins(int amount, float delay)
        {
            int startCoins = CollectedCoins;
            int endCoins = CollectedCoins + amount;
            StartCoroutine(CRUpdateCoins(1, startCoins, endCoins, delay));
        }




        /// <summary>
        /// Remove an amount of total coins.
        /// </summary>
        /// <param name="delay"></param>
        public void RemoveTotalCoins(int amount, float delay)
        {
            int startCoins = TotalCoins;
            int endCoins = TotalCoins - amount;
            StartCoroutine(CRUpdateCoins(0, startCoins, endCoins, delay));
        }



        /// <summary>
        /// Coroutine updating the coins with start coins, end coins and delay time.
        /// index == 1: updating TotalCoins.
        /// index == 1: updating CollectedCoins.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="startCoins"></param>
        /// <param name="endCoins"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator CRUpdateCoins(int index, int startCoins, int endCoins, float delay)
        {
            //Delay == 0 and this is the update of CollectedCoins -> set directly
            if (delay == 0 && index == 1)
            {
                CollectedCoins = endCoins;
                yield break;
            }

            yield return new WaitForSeconds(delay);
            if (endCoins > startCoins) { ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Rewarded); }
            float t = 0;
            float runTime = 0.25f;
            while (t < runTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.EaseOutQuad, t / runTime);
                int newCoins = Mathf.RoundToInt(Mathf.Lerp(startCoins, endCoins, factor));
                if (index == 0) { TotalCoins = newCoins; }
                else { SetCollectedCoins(newCoins); }
                yield return null;
            }
            if (index == 0) { TotalCoins = endCoins; ; }
            else { SetCollectedCoins(endCoins); }
        }
    }
}
