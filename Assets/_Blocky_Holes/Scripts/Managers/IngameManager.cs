using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ClawbearGames
{
    public class IngameManager : MonoBehaviour
    {
        public static IngameManager Instance { private set; get; }
        public static event System.Action<IngameState> IngameStateChanged = delegate { };


        [Header("Enter a number of level to test. Set back to 0 to disable this feature.")]
        [SerializeField] private int testingLevel = 0;



        [Header("Ingame Configuration")]
        [SerializeField] private float reviveWaitTime = 5f;
        [SerializeField] private AudioClip[] backgroundMusicClips = null;

        [Header("Ingame References")]
        [SerializeField] private Material groundMaterial = null;
        [SerializeField] private ParticleSystem[] confettiEffects = null;

        public IngameState IngameState
        {
            get { return ingameState; }
            private set
            {
                if (value != ingameState)
                {
                    ingameState = value;
                    IngameStateChanged(ingameState);
                }
            }
        }
        public float ReviveWaitTime { get { return reviveWaitTime; } }
        public int CurrentLevel { private set; get; }
        public bool IsRevived { private set; get; }



        private IngameState ingameState = IngameState.Ingame_GameOver;
        private AudioClip backgroundMusic = null;
        private LevelData levelData = null;
        private int currentObject = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(Instance.gameObject);
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            ServicesManager.Instance.CoinManager.SetCollectedCoins(0);
            StartCoroutine(CRShowViewWithDelay(ViewType.INGAME_VIEW, 0f));

            //Setup variables
            IsRevived = false;
            confettiEffects[0].transform.parent.gameObject.SetActive(false);

            //Load level parameters
            CurrentLevel = (testingLevel != 0) ? testingLevel : PlayerDataHandler.GetCurrentLevel();

            TextAsset textAsset = Resources.Load<TextAsset>("Levels/" + CurrentLevel.ToString());
            levelData = JsonUtility.FromJson<LevelData>(textAsset.ToString());

            //Load other parameters
            groundMaterial.SetTexture("_Main_Texture", PoolManager.Instance.GetGroundTexture(levelData.GroundTexture));
            backgroundMusic = backgroundMusicClips[Random.Range(0, backgroundMusicClips.Length)];
            PlayerController.Instance.SetMovementSpeed(levelData.PlayerMovementSpeed);

            if (PlayerDataHandler.IsWatchedTutorial()) { Invoke(nameof(PlayingGame), 0.15f); }
        }


        /// <summary>
        /// Call IngameState.Ingame_Playing event and handle other actions.
        /// Actual start the game.
        /// </summary>
        public void PlayingGame()
        {
            //Fire event
            IngameState = IngameState.Ingame_Playing;
            ingameState = IngameState.Ingame_Playing;

            //Other actions
            if (IsRevived)
            {
                StartCoroutine(CRShowViewWithDelay(ViewType.INGAME_VIEW, 0f));
                ServicesManager.Instance.SoundManager.ResumeMusic(0.5f);
                StartCoroutine(CRCountdownTime());
            }
            else
            {
                ServicesManager.Instance.SoundManager.PlayMusic(backgroundMusic, 0.5f);

                //Load target objects
                foreach (TargetObjectData objectData in levelData.ListTargetObjectData)
                {
                    TargetObjectController targetObject = PoolManager.Instance.GetTargetObjectController(objectData.ObjectName);
                    targetObject.transform.position = objectData.Position;
                    targetObject.transform.eulerAngles = objectData.Angles;
                    targetObject.transform.localScale = objectData.Scale;
                    targetObject.gameObject.SetActive(true);
                    ViewManager.Instance.IngameViewController.CreateTargetObjectDot(targetObject);
                }


                //Load deadly objects
                foreach (DeadlyObjectData objectData in levelData.ListDeadlyObjectData)
                {
                    DeadlyObjectController deadlyObject = PoolManager.Instance.GetDeadlyObjectController(objectData.ObjectName);
                    deadlyObject.transform.position = objectData.Position;
                    deadlyObject.transform.eulerAngles = objectData.Angles;
                    deadlyObject.transform.localScale = objectData.Scale;
                    deadlyObject.gameObject.SetActive(true);
                    ViewManager.Instance.IngameViewController.CreateDeadlyObjectDot(deadlyObject);
                }

                ViewManager.Instance.IngameViewController.UpdateObjectTexts(currentObject, levelData.TargetObjectAmount);
                StartCoroutine(CRCountdownTime());
            }
        }


        /// <summary>
        /// Call IngameState.Ingame_Revive event and handle other actions.
        /// </summary>
        public void Revive()
        {
            //Fire event
            IngameState = IngameState.Ingame_Revive;
            ingameState = IngameState.Ingame_Revive;

            //Add another actions here
            StartCoroutine(CRShowViewWithDelay(ViewType.REVIVE_VIEW, 1f));
            ServicesManager.Instance.SoundManager.PauseMusic(0.5f);
        }


        /// <summary>
        /// Call IngameState.Ingame_GameOver event and handle other actions.
        /// </summary>
        public void GameOver()
        {
            //Fire event
            IngameState = IngameState.Ingame_GameOver;
            ingameState = IngameState.Ingame_GameOver;

            //Add another actions here
            StartCoroutine(CRShowViewWithDelay(ViewType.ENDGAME_VIEW, 0.25f));
            ServicesManager.Instance.SoundManager.StopMusic(0.5f);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.LevelFailed);
        }


        /// <summary>
        /// Call IngameState.Ingame_CompleteLevel event and handle other actions.
        /// </summary>
        public void CompletedLevel()
        {
            //Fire event
            IngameState = IngameState.Ingame_CompleteLevel;
            ingameState = IngameState.Ingame_CompleteLevel;

            //Other actions
            StartCoroutine(CRShowViewWithDelay(ViewType.ENDGAME_VIEW, 1f));
            ServicesManager.Instance.SoundManager.StopMusic(0.5f);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.LevelCompleted);

            //Play effects
            confettiEffects[0].transform.parent.position = PlayerController.Instance.transform.position;
            confettiEffects[0].transform.parent.gameObject.SetActive(true);
            foreach(ParticleSystem particle in confettiEffects)
            {
                particle.Play();
            }

            //Save level
            if (testingLevel == 0)
            {
                int totalLevel = Resources.LoadAll("Levels/").Length;
                int nextLevel = ((CurrentLevel + 1) > totalLevel) ? totalLevel : (CurrentLevel + 1);
                PlayerDataHandler.UpdateCurrentLevel(nextLevel);

                //Report level to leaderboard
                if (!string.IsNullOrEmpty(PlayerDataHandler.GetUserName()))
                {
                    ServicesManager.Instance.LeaderboardManager.SetPlayerLeaderboardData();
                }
            }
        }



        /// <summary>
        /// Coroutine count down the time to complete level and update on UI.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRCountdownTime()
        {
            int currentTime = levelData.TimeToCompleteLevel;
            ViewManager.Instance.IngameViewController.UpdateTimePanel(1f, currentTime);
            while(currentTime > 0)
            {
                yield return new WaitForSeconds(1f);
                currentTime--;
                ViewManager.Instance.IngameViewController.UpdateTimePanel(currentTime / (float)levelData.TimeToCompleteLevel, currentTime);
                if (ingameState != IngameState.Ingame_Playing) { yield break; }
            }

            if (ingameState == IngameState.Ingame_Playing)
            {
                PlayerController.Instance.PlayerDied();
                HandlePlayerDied();
            }
        }


        /// <summary>
        /// Coroutine show the view with given viewType and delay time.
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator CRShowViewWithDelay(ViewType viewType, float delay)
        {
            yield return new WaitForSeconds(delay);
            ViewManager.Instance.OnShowView(viewType);
        }



        //////////////////////////////////////Publish functions



        /// <summary>
        /// Continue the game
        /// </summary>
        public void SetContinueGame()
        {
            IsRevived = true;
            Invoke(nameof(PlayingGame), 0.05f);
        }



        /// <summary>
        /// Handle actions when player died.
        /// </summary>
        public void HandlePlayerDied()
        {
            GameOver();
        }



        /// <summary>
        /// Handle when the player ate a target object.
        /// </summary>
        public void OnPlayerAteTargetObject()
        {
            currentObject++;
            ViewManager.Instance.IngameViewController.UpdateObjectTexts(currentObject, levelData.TargetObjectAmount);
            if (currentObject >= levelData.TargetObjectAmount && ingameState != IngameState.Ingame_CompleteLevel)
            {
                CompletedLevel();
            }
        }
    }
}
