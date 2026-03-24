using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ClawbearGames
{
    public class HomeViewController : BaseViewController
    {
        private const string runtimeTitleObjectName = "RuntimeTitleText";
        private const string runtimeTitleTextValue = "Bomb Jumping";
        private const float runtimeTitleScaleMultiplier = 8f;
        private const float runtimeTitleTopOffset = -90f;

        [SerializeField] private RectTransform topPanelTrans = null;
        [SerializeField] private RectTransform bottomPanelTrans = null;
        [SerializeField] private RectTransform gameNameTrans = null;
        [SerializeField] private RectTransform playButtonTrans = null;
        [SerializeField] private RectTransform shareButtonTrans = null;
        [SerializeField] private RectTransform soundButtonsTrans = null;
        [SerializeField] private RectTransform musicButtonsTrans = null;
        [SerializeField] private RectTransform leaderboardButtonTrans = null;
        [SerializeField] private RectTransform rateAppButtonTrans = null;
        [SerializeField] private RectTransform removeAdsButtonTrans = null;
        [SerializeField] private RectTransform soundOnButtonTrans = null;
        [SerializeField] private RectTransform soundOffButtonTrans = null;
        [SerializeField] private RectTransform musicOnButtonTrans = null;
        [SerializeField] private RectTransform musicOffButtonTrans = null;
        [SerializeField] private RectTransform warningSignTrans = null;
        [SerializeField] private Text currentLevelText = null;
        [SerializeField] private Text totalCoinsText = null;

        private Text runtimeTitleText = null;
        private int settingButtonTurn = 1;

        private void Update()
        {
            totalCoinsText.text = ServicesManager.Instance.CoinManager.TotalCoins.ToString();
        }


        /// <summary>
        /// Coroutine on click setting button.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CROnClickSettingButton()
        {
            if (settingButtonTurn == -1)
            {
                MoveRectTransform(shareButtonTrans, shareButtonTrans.anchoredPosition, new Vector2(0, shareButtonTrans.anchoredPosition.y), 0.5f);
                MoveRectTransform(leaderboardButtonTrans, leaderboardButtonTrans.anchoredPosition, new Vector2(0, leaderboardButtonTrans.anchoredPosition.y), 0.5f);

                yield return new WaitForSeconds(0.08f);

                MoveRectTransform(soundButtonsTrans, soundButtonsTrans.anchoredPosition, new Vector2(0, soundButtonsTrans.anchoredPosition.y), 0.5f);
                MoveRectTransform(rateAppButtonTrans, rateAppButtonTrans.anchoredPosition, new Vector2(0, rateAppButtonTrans.anchoredPosition.y), 0.5f);

                yield return new WaitForSeconds(0.08f);

                MoveRectTransform(musicButtonsTrans, musicButtonsTrans.anchoredPosition, new Vector2(0, musicButtonsTrans.anchoredPosition.y), 0.5f);
                MoveRectTransform(removeAdsButtonTrans, removeAdsButtonTrans.anchoredPosition, new Vector2(0, removeAdsButtonTrans.anchoredPosition.y), 0.5f);
            }
            else
            {
                MoveRectTransform(shareButtonTrans, shareButtonTrans.anchoredPosition, new Vector2(-200, shareButtonTrans.anchoredPosition.y), 0.5f);
                MoveRectTransform(leaderboardButtonTrans, leaderboardButtonTrans.anchoredPosition, new Vector2(200, leaderboardButtonTrans.anchoredPosition.y), 0.5f);

                yield return new WaitForSeconds(0.08f);

                MoveRectTransform(soundButtonsTrans, soundButtonsTrans.anchoredPosition, new Vector2(-200, soundButtonsTrans.anchoredPosition.y), 0.5f);
                MoveRectTransform(rateAppButtonTrans, rateAppButtonTrans.anchoredPosition, new Vector2(200, rateAppButtonTrans.anchoredPosition.y), 0.5f);

                yield return new WaitForSeconds(0.08f);

                MoveRectTransform(musicButtonsTrans, musicButtonsTrans.anchoredPosition, new Vector2(-200, musicButtonsTrans.anchoredPosition.y), 0.5f);
                MoveRectTransform(removeAdsButtonTrans, removeAdsButtonTrans.anchoredPosition, new Vector2(200, removeAdsButtonTrans.anchoredPosition.y), 0.5f);
            }
        }

        private void EnsureRuntimeTitle()
        {
            if (gameNameTrans == null)
            {
                return;
            }

            Image gameNameImage = gameNameTrans.GetComponent<Image>();
            if (gameNameImage != null)
            {
                gameNameImage.enabled = false;
            }

            if (runtimeTitleText == null)
            {
                Transform existingTitleTransform = gameNameTrans.Find(runtimeTitleObjectName);
                if (existingTitleTransform != null)
                {
                    runtimeTitleText = existingTitleTransform.GetComponent<Text>();
                }

                if (runtimeTitleText == null)
                {
                    GameObject runtimeTitleObject = new GameObject(runtimeTitleObjectName, typeof(RectTransform));
                    runtimeTitleObject.transform.SetParent(gameNameTrans, false);

                    RectTransform runtimeTitleRect = runtimeTitleObject.GetComponent<RectTransform>();
                    runtimeTitleRect.anchorMin = Vector2.zero;
                    runtimeTitleRect.anchorMax = Vector2.one;
                    runtimeTitleRect.offsetMin = Vector2.zero;
                    runtimeTitleRect.offsetMax = Vector2.zero;
                    runtimeTitleRect.localScale = Vector3.one;
                    runtimeTitleRect.localPosition = Vector3.zero;

                    runtimeTitleText = runtimeTitleObject.AddComponent<Text>();
                }
            }

            if (runtimeTitleText == null)
            {
                return;
            }

            ApplyRuntimeTitleStyle(runtimeTitleText);
            runtimeTitleText.text = runtimeTitleTextValue;
            RectTransform runtimeTitleRect = runtimeTitleText.rectTransform;
            float rectWidth = gameNameTrans.rect.width > 0f ? gameNameTrans.rect.width : 1024f;
            float rectHeight = gameNameTrans.rect.height > 0f ? gameNameTrans.rect.height : 256f;
            runtimeTitleRect.anchorMin = new Vector2(0.5f, 1f);
            runtimeTitleRect.anchorMax = new Vector2(0.5f, 1f);
            runtimeTitleRect.pivot = new Vector2(0.5f, 1f);
            runtimeTitleRect.sizeDelta = new Vector2(rectWidth, rectHeight);
            runtimeTitleRect.anchoredPosition = new Vector2(0f, runtimeTitleTopOffset);
            runtimeTitleRect.localScale = new Vector3(runtimeTitleScaleMultiplier, runtimeTitleScaleMultiplier, 1f);

            Outline runtimeTitleOutline = runtimeTitleText.GetComponent<Outline>();
            if (runtimeTitleOutline == null)
            {
                runtimeTitleOutline = runtimeTitleText.gameObject.AddComponent<Outline>();
            }
            runtimeTitleOutline.effectColor = Color.black;
            runtimeTitleOutline.effectDistance = new Vector2(3f, 3f);
            runtimeTitleOutline.useGraphicAlpha = true;
        }

        private void ApplyRuntimeTitleStyle(Text targetText)
        {
            Text styleSource = currentLevelText != null ? currentLevelText : totalCoinsText;

            if (styleSource != null)
            {
                targetText.font = styleSource.font;
                targetText.fontStyle = styleSource.fontStyle;
                targetText.color = styleSource.color;
                targetText.lineSpacing = styleSource.lineSpacing;
                targetText.supportRichText = styleSource.supportRichText;
            }
            else
            {
                targetText.color = Color.white;
            }

            targetText.alignment = TextAnchor.MiddleCenter;
            targetText.horizontalOverflow = HorizontalWrapMode.Overflow;
            targetText.verticalOverflow = VerticalWrapMode.Overflow;
            targetText.resizeTextForBestFit = true;
            targetText.resizeTextMinSize = 24;
            targetText.resizeTextMaxSize = 130;
            targetText.raycastTarget = false;
        }


        /// <summary>
        ////////////////////////////////////////////////// Public Functions
        /// </summary>


        public override void OnShow()
        {
            EnsureRuntimeTitle();

            MoveRectTransform(topPanelTrans, topPanelTrans.anchoredPosition, new Vector2(topPanelTrans.anchoredPosition.x, 0f), 0.5f);
            MoveRectTransform(bottomPanelTrans, bottomPanelTrans.anchoredPosition, new Vector2(bottomPanelTrans.anchoredPosition.x, 0f), 0.75f);
            ScaleRectTransform(gameNameTrans, Vector2.zero, Vector2.one, 1f);
            ScaleRectTransform(playButtonTrans, Vector2.zero, Vector2.one, 0.5f);

            settingButtonTurn = 1;
            currentLevelText.text = "LEVEL: " + PlayerDataHandler.GetCurrentLevel().ToString();

            // MVP: keep Home focused on Play only.
            shareButtonTrans.gameObject.SetActive(false);
            leaderboardButtonTrans.gameObject.SetActive(false);
            rateAppButtonTrans.gameObject.SetActive(false);
            removeAdsButtonTrans.gameObject.SetActive(false);
            soundButtonsTrans.gameObject.SetActive(false);
            musicButtonsTrans.gameObject.SetActive(false);
            warningSignTrans.gameObject.SetActive(false);

            //Update sound buttons
            if (PlayerDataHandler.IsSoundOff())
            {
                soundOnButtonTrans.gameObject.SetActive(false);
                soundOffButtonTrans.gameObject.SetActive(true);
            }
            else
            {
                soundOnButtonTrans.gameObject.SetActive(true);
                soundOffButtonTrans.gameObject.SetActive(false);
            }

            //Update music buttons
            if (PlayerDataHandler.IsMusicOff())
            {
                musicOffButtonTrans.gameObject.SetActive(true);
                musicOnButtonTrans.gameObject.SetActive(false);
            }
            else
            {
                musicOffButtonTrans.gameObject.SetActive(false);
                musicOnButtonTrans.gameObject.SetActive(true);
            }


            //Handle warning sign
            warningSignTrans.gameObject.SetActive(false);
        }


        public override void OnClose()
        {
            topPanelTrans.anchoredPosition = new Vector2(topPanelTrans.anchoredPosition.x, 200f);
            bottomPanelTrans.anchoredPosition = new Vector2(bottomPanelTrans.anchoredPosition.x, -500f);
            gameNameTrans.localScale = Vector2.zero;
            playButtonTrans.localScale = Vector3.zero;

            shareButtonTrans.anchoredPosition = new Vector2(-200, shareButtonTrans.anchoredPosition.y);
            soundButtonsTrans.anchoredPosition = new Vector2(-200, soundButtonsTrans.anchoredPosition.y);
            musicButtonsTrans.anchoredPosition = new Vector2(-200, musicButtonsTrans.anchoredPosition.y);
            leaderboardButtonTrans.anchoredPosition = new Vector2(200, leaderboardButtonTrans.anchoredPosition.y);
            rateAppButtonTrans.anchoredPosition = new Vector2(200, rateAppButtonTrans.anchoredPosition.y);
            removeAdsButtonTrans.anchoredPosition = new Vector2(200, removeAdsButtonTrans.anchoredPosition.y);
            gameObject.SetActive(false);
        }


        /// <summary>
        ////////////////////////////////////////////////// UI Buttons
        /// </summary>


        public void OnClickPlayButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            LoadScene("Ingame", 0.25f);
        }


        public void OnClickRewardButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }


        public void OnClickCharacterButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }

        public void OnClickSettingButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }

        public void OnClickShareButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }

        public void OnClickSoundButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ServicesManager.Instance.SoundManager.ToggleSound();
            if (PlayerDataHandler.IsSoundOff())
            {
                soundOnButtonTrans.gameObject.SetActive(false);
                soundOffButtonTrans.gameObject.SetActive(true);
            }
            else
            {
                soundOnButtonTrans.gameObject.SetActive(true);
                soundOffButtonTrans.gameObject.SetActive(false);
            }
        }

        public void OnClickMusicButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ServicesManager.Instance.SoundManager.ToggleMusic();
            if (PlayerDataHandler.IsMusicOff())
            {
                musicOffButtonTrans.gameObject.SetActive(true);
                musicOnButtonTrans.gameObject.SetActive(false);
            }
            else
            {
                musicOffButtonTrans.gameObject.SetActive(false);
                musicOnButtonTrans.gameObject.SetActive(true);
            }
        }



        public void OnClickLeaderboardButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }

        public void OnClickRateAppButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }
        public void OnClickRemoveAdsButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }
    }
}
