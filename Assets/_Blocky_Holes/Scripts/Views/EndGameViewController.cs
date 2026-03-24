using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ClawbearGames
{
    public class EndGameViewController : BaseViewController
    {
        [SerializeField] private CanvasGroup mainCanvasGroup = null;
        [SerializeField] private CanvasGroup collectedCoinsCanvasGroup = null;
        [SerializeField] private RectTransform topPanelTrans = null;
        [SerializeField] private RectTransform resultPanelTrans = null;
        [SerializeField] private RectTransform mainButtonsPanelTrans = null;
        [SerializeField] private RectTransform bottonPanelTrans = null;
        [SerializeField] private RectTransform levelCompletedTextTrans = null;
        [SerializeField] private RectTransform levelFailedTextTrans = null;
        [SerializeField] private RectTransform nextLevelButtonTrans = null;
        [SerializeField] private RectTransform replayLevelButtonTrans = null;
        [SerializeField] private RectTransform doubleCoinButtonTrans = null;
        [SerializeField] private RectTransform sunbrustImageTrans = null;
        [SerializeField] private Text currentLevelText = null;
        [SerializeField] private Text totalCoinsText = null;
        [SerializeField] private Text collectedCoinsText = null;

        private void Update()
        {
            totalCoinsText.text = ServicesManager.Instance.CoinManager.TotalCoins.ToString();
            collectedCoinsText.text = ServicesManager.Instance.CoinManager.CollectedCoins.ToString();
        }


        /// <summary>
        /// Coroutine rotate the sunbrust image.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRRotateSunbrustImage()
        {
            while (gameObject.activeInHierarchy)
            {
                sunbrustImageTrans.localEulerAngles += Vector3.forward * 75f * Time.deltaTime;
                yield return null;
            }
        }


        /// <summary>
        ////////////////////////////////////////////////// Public Functions
        /// </summary>


        public override void OnShow()
        {
            FadeInCanvasGroup(mainCanvasGroup, 0.75f);
            MoveRectTransform(topPanelTrans, topPanelTrans.anchoredPosition, new Vector2(topPanelTrans.anchoredPosition.x, 0f), 0.5f);
            MoveRectTransform(resultPanelTrans, resultPanelTrans.anchoredPosition, new Vector2(0f, resultPanelTrans.anchoredPosition.y), 0.5f);
            MoveRectTransform(bottonPanelTrans, bottonPanelTrans.anchoredPosition, new Vector2(bottonPanelTrans.anchoredPosition.x, 0f), 0.5f);
            ScaleRectTransform(mainButtonsPanelTrans, Vector3.zero, Vector3.one, 0.5f);
            StartCoroutine(CRRotateSunbrustImage());

            if (IngameManager.Instance.IngameState == IngameState.Ingame_CompleteLevel)
            {
                levelCompletedTextTrans.gameObject.SetActive(true);
                levelFailedTextTrans.gameObject.SetActive(false);

                nextLevelButtonTrans.gameObject.SetActive(true);
                replayLevelButtonTrans.gameObject.SetActive(false);
            }
            else
            {
                levelFailedTextTrans.gameObject.SetActive(true);
                levelCompletedTextTrans.gameObject.SetActive(false);

                replayLevelButtonTrans.gameObject.SetActive(true);
                nextLevelButtonTrans.gameObject.SetActive(false);
            }


            // MVP: no ad-driven reward branch on endgame screen.
            collectedCoinsCanvasGroup.gameObject.SetActive(false);
            doubleCoinButtonTrans.gameObject.SetActive(false);



            //Update texts
            currentLevelText.text = "Level: " +  PlayerDataHandler.GetCurrentLevel().ToString();
        }

        public override void OnClose()
        {
            mainCanvasGroup.alpha = 0;
            collectedCoinsCanvasGroup.alpha = 0;
            topPanelTrans.anchoredPosition = new Vector2(topPanelTrans.anchoredPosition.x, 300f);
            resultPanelTrans.anchoredPosition = new Vector2(-1100f, resultPanelTrans.anchoredPosition.y);
            bottonPanelTrans.anchoredPosition = new Vector2(bottonPanelTrans.anchoredPosition.x, -500f);
            mainButtonsPanelTrans.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }

        /// <summary>
        ////////////////////////////////////////////////// UI Buttons
        /// </summary>  


        public void OnClickClaimButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }

        public void OnClickDoubleCoinButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }

        public void OnClickPlayButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            LoadScene("Ingame", 0.25f);
        }

        public void OnClickShareButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }

        public void OnClickCharacterButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }

        public void OnClickHomeButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }
    }
}
