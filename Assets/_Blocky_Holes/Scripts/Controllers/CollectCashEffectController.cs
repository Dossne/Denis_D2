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
        private GameObject cashIconObject = null;
        private Coroutine moveCoroutine = null;

        private void Awake()
        {
            CacheCashIcon();
        }

        private void OnEnable()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        private void CacheCashIcon()
        {
            if (cashIconObject != null || cashText == null)
            {
                return;
            }

            Transform iconTransform = cashText.transform.Find("Image");
            if (iconTransform != null)
            {
                cashIconObject = iconTransform.gameObject;
            }
        }

        private void BeginEffect(int amount, bool showCashIcon)
        {
            cashAmount = amount;
            if (cashText != null)
            {
                cashText.text = "+" + amount.ToString();
            }

            CacheCashIcon();
            if (cashIconObject != null)
            {
                cashIconObject.SetActive(showCashIcon);
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(CRMoveUp());
        }



        /// <summary>
        /// Init this collect cash effect with amount.
        /// </summary>
        /// <param name="amount"></param>
        public void OnInit(int amount)
        {
            BeginEffect(amount, true);
            ServicesManager.Instance.CoinManager.AddCollectedCoins(amount, 0f);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.CashCollected);
        }

        /// <summary>
        /// Init this floating effect in score mode (no coin side effects).
        /// </summary>
        /// <param name="amount"></param>
        public void OnInitScore(int amount)
        {
            BeginEffect(amount, false);
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

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
            moveCoroutine = null;
            gameObject.SetActive(false);
        }
    }
}
