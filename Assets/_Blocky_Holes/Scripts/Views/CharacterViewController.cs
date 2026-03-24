using UnityEngine;
using UnityEngine.UI;

namespace ClawbearGames
{
    public class CharacterViewController : BaseViewController
    {
        [SerializeField] private CanvasGroup canvasGroup = null;
        [SerializeField] private RectTransform topPanelTrans = null;
        [SerializeField] private RectTransform bottomPanelTrans = null;
        [SerializeField] private Button selectButton = null;
        [SerializeField] private Button unlockButton = null;
        [SerializeField] private Text totalCoinsText = null;
        [SerializeField] private Text charPriceText = null;


        private CharacterManager characterManager = null;
        private CharacterInforController currentCharacterInforController = null;


        private void Update()
        {
            totalCoinsText.text = ServicesManager.Instance.CoinManager.TotalCoins.ToString();
        }



        /// <summary>
        ////////////////////////////////////////////////// Public Functions
        /// </summary>


        public override void OnShow()
        {
            FadeInCanvasGroup(canvasGroup, 0.75f);
            MoveRectTransform(topPanelTrans, topPanelTrans.anchoredPosition, new Vector2(topPanelTrans.anchoredPosition.x, 0), 0.5f);
            MoveRectTransform(bottomPanelTrans, bottomPanelTrans.anchoredPosition, new Vector2(bottomPanelTrans.anchoredPosition.x, 0), 0.5f);
            if (characterManager == null) { characterManager = FindFirstObjectByType<CharacterManager>(); }
        }

        public override void OnClose()
        {
            canvasGroup.alpha = 0f;
            topPanelTrans.anchoredPosition = new Vector2(topPanelTrans.anchoredPosition.x, 150f);
            bottomPanelTrans.anchoredPosition = new Vector2(topPanelTrans.anchoredPosition.x, -500f);
            gameObject.SetActive(false);
        }




        /// <summary>
        /// Update the UI base on given CharacterInforController.
        /// </summary>
        /// <param name="characterInfor"></param>
        public void UpdateCharacterInfor(CharacterInforController characterInfor)
        {
            currentCharacterInforController = characterInfor;
            if (!characterInfor.IsUnlocked) //The skin is not unlocked yet
            {
                selectButton.gameObject.SetActive(false);
                unlockButton.gameObject.SetActive(true);
                charPriceText.text = characterInfor.CharacterPrice.ToString();
                if (ServicesManager.Instance.CoinManager.TotalCoins >= characterInfor.CharacterPrice)
                {
                    //Enough coins -> allow user buy this skin
                    unlockButton.interactable = true;
                }
                else
                {
                    //Not enough coins -> dont allow user buy this skin
                    unlockButton.interactable = false;
                }
            }
            else//The skin is already unlocked
            {
                unlockButton.gameObject.SetActive(false);
                selectButton.gameObject.SetActive(true);
            }
        }


        /// <summary>
        ////////////////////////////////////////////////// UI Buttons
        /// </summary>


        public void OnClickUnlockButton()
        {
            characterManager.PlayEffects();
            currentCharacterInforController.Unlock();
            UpdateCharacterInfor(currentCharacterInforController);
        }

        public void OnClickSelectButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            PlayerDataHandler.UpdateSelectedCharacterIndex(currentCharacterInforController.SequenceNumber);
            LoadScene("Home", 0.25f);
        }

        public void OnClickCloseButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            LoadScene("Home", 0.25f);
        }
    }
}
