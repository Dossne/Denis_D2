using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClawbearGames
{
    public class CollectCashEffectController : MonoBehaviour
    {
        [SerializeField] private Text cashText = null;
        [SerializeField] private CanvasGroup canvasGroup = null;
        private int cashAmount = 0;



        /// <summary>
        /// Init this collect cash effect with amount.
        /// </summary>
        /// <param name="amount"></param>
        public void OnInit(int amount)
        {
            cashAmount = amount;
            cashText.text = "+" + amount.ToString();
            StartCoroutine(CRMoveUp());
            ServicesManager.Instance.CoinManager.AddCollectedCoins(amount, 0f);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.CashCollected);
        }




        /// <summary>
        /// Coroutine move up and fade out effect.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRMoveUp()
        {
            float t = 0;
            float effectTime = 2f;
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.up * 8f;
            while (t < effectTime)
            {
                t += Time.deltaTime;
                float factor = t / effectTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, factor);
                transform.position=Vector3.Lerp(startPos, endPos, factor);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            gameObject.SetActive(false);
        }
    }
}
