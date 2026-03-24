using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace ClawbearGames
{
    public class IngameViewController : BaseViewController
    {
        [SerializeField] private RectTransform lelfPanelTrans = null;
        [SerializeField] private RectTransform tutorialPanelTrans = null;
        [SerializeField] private RectTransform playerDotTrans = null;
        [SerializeField] private RectTransform mapPanelTrans = null;
        [SerializeField] private RectTransform watchAdButton = null;
        [SerializeField] private Image timeSliderImage = null;
        [SerializeField] private Image targetSliderImage = null;
        [SerializeField] private Text currentObjectText = null;
        [SerializeField] private Text targetObjectText = null;
        [SerializeField] private Text levelText = null;
        [SerializeField] private Text timeText = null;
        [SerializeField] private RectTransform targetObjectDotPrefab = null;
        [SerializeField] private RectTransform deadlyObjectDotPrefab = null;

        private Dictionary<TargetObjectController, RectTransform> dicTargetObjectDot = new Dictionary<TargetObjectController, RectTransform>();
        private Dictionary<DeadlyObjectController, RectTransform> dicDeadlyObjectDot = new Dictionary<DeadlyObjectController, RectTransform>();
        private List<RectTransform> listTargetObjectDot = new List<RectTransform>();
        private List<RectTransform> listDeadlyObjectDot = new List<RectTransform>();
        private float posScaleFactor = 1.5f;



        /// <summary>
        /// Get an inactive target object dot.
        /// </summary>
        /// <returns></returns>
        private RectTransform GetTargetObjectDot()
        {
            //Find in the list
            RectTransform targetObjectDot = listTargetObjectDot.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if(targetObjectDot == null)
            {
                //Didn't find one -> create new one
                targetObjectDot = Instantiate(targetObjectDotPrefab);
                targetObjectDot.transform.SetParent(mapPanelTrans);
                targetObjectDot.transform.localScale = Vector3.one;
                targetObjectDot.transform.SetAsLastSibling();
                targetObjectDot.gameObject.SetActive(false);
                listTargetObjectDot.Add(targetObjectDot);
            }

            return targetObjectDot;
        }



        /// <summary>
        /// Get an inactive deadly object dot.
        /// </summary>
        /// <returns></returns>
        private RectTransform GetDeadlyObjectDot()
        {
            //Find in the list
            RectTransform deadlyObjectDot = listDeadlyObjectDot.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (deadlyObjectDot == null)
            {
                //Didn't find one -> create new one
                deadlyObjectDot = Instantiate(deadlyObjectDotPrefab);
                deadlyObjectDot.transform.SetParent(mapPanelTrans);
                deadlyObjectDot.transform.localScale = Vector3.one;
                deadlyObjectDot.transform.SetAsLastSibling();
                deadlyObjectDot.gameObject.SetActive(false);
                listDeadlyObjectDot.Add(deadlyObjectDot);
            }

            return deadlyObjectDot;
        }


        private void Update()
        {
            Vector3 playerPos = PlayerController.Instance.transform.position;
            playerDotTrans.anchoredPosition = new Vector2(playerPos.x * posScaleFactor, playerPos.z * posScaleFactor);
            playerDotTrans.SetAsLastSibling();
        }


        /// <summary>
        ////////////////////////////////////////////////// Public Functions
        /// </summary>


        public override void OnShow()
        {
            MoveRectTransform(lelfPanelTrans, lelfPanelTrans.anchoredPosition, new Vector2(lelfPanelTrans.anchoredPosition.x, -10f), 0.5f);

            //Update texts and other fields, parameters
            levelText.text = "Level: " + IngameManager.Instance.CurrentLevel.ToString();
            if (!IngameManager.Instance.IsRevived)
            {
                foreach(RectTransform rect in listTargetObjectDot)
                {
                    rect.gameObject.SetActive(false);
                }

                foreach (RectTransform rect in listDeadlyObjectDot)
                {
                    rect.gameObject.SetActive(false);
                }

                dicTargetObjectDot.Clear();
                dicDeadlyObjectDot.Clear();

                targetSliderImage.fillAmount = 0f;
            }
            tutorialPanelTrans.gameObject.SetActive(!PlayerDataHandler.IsWatchedTutorial());
            watchAdButton.gameObject.SetActive(false);
            StartCoroutine(CRCheckAndShowWatchAdButton());
        }

        public override void OnClose()
        {
            lelfPanelTrans.anchoredPosition = new Vector2(lelfPanelTrans.anchoredPosition.x, 150f);
            gameObject.SetActive(false);
        }




        /// <summary>
        /// Update the object texts on UI.
        /// </summary>
        /// <param name="currentObject"></param>
        /// <param name="targetObject"></param>
        public void UpdateObjectTexts(int currentObject, int targetObject)
        {
            currentObjectText.text = currentObject.ToString();
            targetObjectText.text = targetObject.ToString();
            targetSliderImage.fillAmount = currentObject / (float)targetObject;
        }



        /// <summary>
        /// Update the time text and the time slider.
        /// </summary>
        /// <param name="fillAmount"></param>
        /// <param name="currentTime"></param>
        public void UpdateTimePanel(float fillAmount, int currentTime)
        {
            timeText.text = currentTime.ToString();
            timeSliderImage.fillAmount = fillAmount;
        }


        /// <summary>
        /// Create the target object dot with given TargetObjectController.
        /// </summary>
        /// <param name="targetObject"></param>
        public void CreateTargetObjectDot(TargetObjectController targetObject)
        {
            RectTransform targetObjectDot = GetTargetObjectDot();
            Vector2 dotPos = new Vector2(targetObject.transform.position.x * posScaleFactor, targetObject.transform.position.z * posScaleFactor);
            targetObjectDot.anchoredPosition = dotPos;
            targetObjectDot.gameObject.SetActive(true);
            dicTargetObjectDot.Add(targetObject, targetObjectDot);
        }



        /// <summary>
        /// Remove the target object dot corresponding with the given target object.
        /// </summary>
        /// <param name="targetObject"></param>
        public void RemoveTargetObjectDot(TargetObjectController targetObject)
        {
            RectTransform targetObjectDot = null;
            dicTargetObjectDot.TryGetValue(targetObject, out targetObjectDot);
            if (targetObjectDot != null)
            {
                dicTargetObjectDot.Remove(targetObject);
                targetObjectDot.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// Update the position of target object dot corresponding with the given target object.
        /// </summary>
        /// <param name="targetObject"></param>
        public void UpdateTargetObjectPos(TargetObjectController targetObject)
        {
            RectTransform targetObjectDot = null;
            dicTargetObjectDot.TryGetValue(targetObject, out targetObjectDot);
            if (targetObjectDot != null)
            {
                Vector2 dotPos = new Vector2(targetObject.transform.position.x * posScaleFactor, targetObject.transform.position.z * posScaleFactor);
                targetObjectDot.anchoredPosition = dotPos;
            }
        }


        /// <summary>
        /// Create the deadly object dot with given DeadlyObjectController.
        /// </summary>
        /// <param name="deadlyObject"></param>
        public void CreateDeadlyObjectDot(DeadlyObjectController deadlyObject)
        {
            RectTransform deadlyObjectDot = GetDeadlyObjectDot();
            Vector2 dotPos = new Vector2(deadlyObject.transform.position.x * posScaleFactor, deadlyObject.transform.position.z * posScaleFactor);
            deadlyObjectDot.anchoredPosition = dotPos;
            deadlyObjectDot.gameObject.SetActive(true);
            dicDeadlyObjectDot.Add(deadlyObject, deadlyObjectDot);
        }



        /// <summary>
        /// Remove the deadly object dot corresponding with the given deadly object.
        /// </summary>
        /// <param name="deadlyObject"></param>
        public void RemoveDeadlyObjectDot(DeadlyObjectController deadlyObject)
        {
            RectTransform deadlyObjectDot = null;
            dicDeadlyObjectDot.TryGetValue(deadlyObject, out deadlyObjectDot);
            if (deadlyObjectDot != null)
            {
                dicDeadlyObjectDot.Remove(deadlyObject);
                deadlyObjectDot.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// Update the position of deadly object dot corresponding with the given target object.
        /// </summary>
        /// <param name="deadlyObject"></param>
        public void UpdateDeadlyObjectPos(DeadlyObjectController deadlyObject)
        {
            RectTransform deadlyObjectDot = null;
            dicDeadlyObjectDot.TryGetValue(deadlyObject, out deadlyObjectDot);
            if (deadlyObjectDot != null)
            {
                Vector2 dotPos = new Vector2(deadlyObject.transform.position.x * posScaleFactor, deadlyObject.transform.position.z * posScaleFactor);
                deadlyObjectDot.anchoredPosition = dotPos;
            }
        }


        /// <summary>
        /// Coroutine check and show the watch ad button.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRCheckAndShowWatchAdButton()
        {
            while (gameObject.activeSelf)
            {
                yield return new WaitForSeconds(1f);
                if (!watchAdButton.gameObject.activeSelf)
                {
                    watchAdButton.gameObject.SetActive(ServicesManager.Instance.AdManager.IsRewardedAdReady());
                }
            }
        }


        /// <summary>
        ////////////////////////////////////////////////// UI Buttons
        /// </summary>



        public void OnClickCloseTutorialButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            PlayerDataHandler.UpdateWatchedTutorial();
            tutorialPanelTrans.gameObject.SetActive(false);
            IngameManager.Instance.PlayingGame();
        }


        public void OnClickWatchAdButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            watchAdButton.gameObject.SetActive(false);
            ServicesManager.Instance.AdManager.ShowRewardedAd(RewardedAdTarget.ADD_MORE_SIZE);
        }

    }
}
