using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class PlayerController : MonoBehaviour
    {

        public static PlayerController Instance { private set; get; }
        public static event System.Action<PlayerState> PlayerStateChanged = delegate { };


        //[Header("Player Configuration")]

        [Header("Player References")]
        [SerializeField] private string objectLayer = "Object";
        [SerializeField] private string defaultLayer = "Default";
        [SerializeField] private Transform holeParentTrans = null;
        [SerializeField] private Transform fireEffectTrans = null;
        [SerializeField] private SpriteRenderer holeSpriteRenderer = null;
        [SerializeField] private ParticleSystem[] holeFireEffects = null;


        public PlayerState PlayerState
        {
            get { return playerState; }
            private set
            {
                if (value != playerState)
                {
                    value = playerState;
                    PlayerStateChanged(playerState);
                }
            }
        }

        private PlayerState playerState = PlayerState.Player_Prepare;
        private List<TargetObjectController> listCurrentTarget = new List<TargetObjectController>();
        private List<DeadlyObjectController> listCurrentDeadly = new List<DeadlyObjectController>();
        private List<TargetObjectController> listDetectedTarget = new List<TargetObjectController>();
        private List<DeadlyObjectController> listDetectedDeadly = new List<DeadlyObjectController>();
        private Vector3 firstInputPos = Vector3.zero;
        private float baseHoleDiameter = 1f;
        private float currentHoleSize = 1f;
        private float targetHoleSize = 1f;
        private float currentHoleDiameter = 1f;
        private float absorbDiameterPerScale = 1f;
        private int currentHoleScore = 0;
        private int currentHoleLevel = HoleProgressionRules.MinHoleLevel;
        private float movementSpeed = 0f;
        private float currentSpeed = 0f;
        private bool isStopControl = false;

        private const string objectFallbackLayerName = "Ignore Raycast";
        private const string defaultFallbackLayerName = "Default";
        private const float overlapDetectionMargin = 0.05f;

        private void OnEnable()
        {
            IngameManager.IngameStateChanged += IngameManager_IngameStateChanged;
        }
        private void OnDisable()
        {
            IngameManager.IngameStateChanged -= IngameManager_IngameStateChanged;
        }
        private void IngameManager_IngameStateChanged(IngameState obj)
        {
            if (obj == IngameState.Ingame_Playing)
            {
                PlayerLiving();
            }
            else if (obj == IngameState.Ingame_CompleteLevel)
            {
                PlayerCompletedLevel();
            }
        }




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
            //Fire event
            PlayerState = PlayerState.Player_Prepare;
            playerState = PlayerState.Player_Prepare;

            //Normalize configured layer names to prevent invalid runtime layer assignment.
            objectLayer = ResolveLayerName(objectLayer, objectFallbackLayerName);
            defaultLayer = ResolveLayerName(defaultLayer, defaultFallbackLayerName);

            //Make sure pulled objects can pass through the ground and fall into the hole.
            int objectLayerIndex = LayerMask.NameToLayer(objectLayer);
            GameObject groundObject = GameObject.Find("Ground");
            if (groundObject != null && objectLayerIndex >= 0)
            {
                int groundLayerIndex = groundObject.layer;
                Physics.IgnoreLayerCollision(objectLayerIndex, groundLayerIndex, true);
            }

            //Apply unified hole visual and disable decorative hole effects.
            HoleVisualUtility.ApplyReferenceSprite(holeSpriteRenderer);
            InitializeAbsorbDiameterPerScale();
            InitializeHoleProgression();
            HoleVisualUtility.DisableHoleEffects(transform, holeFireEffects, fireEffectTrans);
            HoleVisualUtility.DisableHoleEffectsInScene();

            //Setup parameters and objects
            isStopControl = true;
        }

        /// <summary>
        /// Resolve a layer name and fallback when the layer does not exist.
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="fallbackName"></param>
        /// <returns></returns>
        private string ResolveLayerName(string layerName, string fallbackName)
        {
            if (LayerMask.NameToLayer(layerName) >= 0)
            {
                return layerName;
            }

            if (LayerMask.NameToLayer(fallbackName) >= 0)
            {
                return fallbackName;
            }

            return layerName;
        }

        /// <summary>
        /// Cache conversion ratio between hole scale and black-aperture diameter.
        /// </summary>
        private void InitializeAbsorbDiameterPerScale()
        {
            float currentScale = Mathf.Max(holeParentTrans.localScale.x, HoleProgressionRules.SizeEpsilon);
            float blackApertureDiameter = HoleVisualUtility.GetBlackApertureDiameterWorld(holeSpriteRenderer, currentScale);
            absorbDiameterPerScale = Mathf.Max(blackApertureDiameter / currentScale, HoleProgressionRules.SizeEpsilon);
        }

        private float ScaleToAbsorbDiameter(float holeScale)
        {
            return Mathf.Max(holeScale * absorbDiameterPerScale, HoleProgressionRules.SizeEpsilon);
        }

        private float AbsorbDiameterToScale(float absorbDiameter)
        {
            return Mathf.Max(absorbDiameter / absorbDiameterPerScale, HoleProgressionRules.SizeEpsilon);
        }

        private float GetHoleDetectionRadius()
        {
            if (holeSpriteRenderer != null)
            {
                float spriteOuterRadius = holeSpriteRenderer.bounds.extents.x;
                if (spriteOuterRadius > HoleProgressionRules.SizeEpsilon)
                {
                    return spriteOuterRadius + overlapDetectionMargin;
                }
            }

            return (currentHoleDiameter * 0.5f) + overlapDetectionMargin;
        }

        private float GetHoleRadius()
        {
            return currentHoleDiameter * 0.5f;
        }

        private void InitializeHoleProgression()
        {
            float baseYScale = holeParentTrans.localScale.y;
            float initialHoleScale = Mathf.Max(holeParentTrans.localScale.x, HoleProgressionRules.SizeEpsilon);
            baseHoleDiameter = ScaleToAbsorbDiameter(initialHoleScale);

            currentHoleScore = 0;
            currentHoleLevel = HoleProgressionRules.MinHoleLevel;
            currentHoleDiameter = HoleProgressionRules.GetHoleDiameter(baseHoleDiameter, currentHoleLevel);

            targetHoleSize = AbsorbDiameterToScale(currentHoleDiameter);
            currentHoleSize = targetHoleSize;
            holeParentTrans.localScale = new Vector3(currentHoleSize, baseYScale, currentHoleSize);
        }

        /// <summary>
        /// Get the world-space center of the hole for overlap checks.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetHoleCenterWorldPosition()
        {
            if (holeSpriteRenderer != null)
            {
                return holeSpriteRenderer.transform.position;
            }

            return transform.position;
        }

        private bool IsObjectFullyInsideAperture(Vector3 objectCenter, float objectSize)
        {
            float objectRadius = Mathf.Max(objectSize * 0.5f, HoleProgressionRules.SizeEpsilon);
            float holeRadius = GetHoleRadius();

            Vector3 holeCenter = GetHoleCenterWorldPosition();
            float deltaX = objectCenter.x - holeCenter.x;
            float deltaZ = objectCenter.z - holeCenter.z;
            float distanceXZ = Mathf.Sqrt((deltaX * deltaX) + (deltaZ * deltaZ));

            return distanceXZ + objectRadius <= holeRadius + HoleProgressionRules.SizeEpsilon;
        }

        private bool CanStartAbsorbObject(Vector3 objectCenter, float objectSize)
        {
            return CanAbsorbObject(objectSize) && IsObjectFullyInsideAperture(objectCenter, objectSize);
        }

        /// <summary>
        /// Calibrate the base hole diameter to ensure first-tier objects are absorbable.
        /// This should be called right after level objects are spawned.
        /// </summary>
        /// <param name="calibratedBaseDiameter"></param>
        public void CalibrateBaseHoleDiameter(float calibratedBaseDiameter)
        {
            float safeCalibratedDiameter = Mathf.Max(calibratedBaseDiameter, HoleProgressionRules.SizeEpsilon);
            if (safeCalibratedDiameter <= baseHoleDiameter + HoleProgressionRules.SizeEpsilon)
            {
                return;
            }

            float baseYScale = holeParentTrans.localScale.y;
            baseHoleDiameter = safeCalibratedDiameter;
            currentHoleDiameter = HoleProgressionRules.GetHoleDiameter(baseHoleDiameter, currentHoleLevel);
            float resolvedHoleScale = AbsorbDiameterToScale(currentHoleDiameter);

            if (currentHoleLevel == HoleProgressionRules.MinHoleLevel && currentHoleScore == 0)
            {
                targetHoleSize = resolvedHoleScale;
                currentHoleSize = resolvedHoleScale;
                holeParentTrans.localScale = new Vector3(currentHoleSize, baseYScale, currentHoleSize);
            }
            else
            {
                targetHoleSize = Mathf.Max(targetHoleSize, resolvedHoleScale);
            }
        }

        private void Update()
        {
            if (playerState == PlayerState.Player_Living)
            {
                if (!isStopControl)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        currentSpeed = movementSpeed / 2f;
                        firstInputPos = new Vector3(Input.mousePosition.x, 0f, Input.mousePosition.y);
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        //Update the current speed
                        currentSpeed = Mathf.Clamp(currentSpeed + Time.deltaTime, 0f, movementSpeed);

                        //Calculate the movingDir and move the player 
                        Vector3 currentInputPos = new Vector3(Input.mousePosition.x, 0f, Input.mousePosition.y);
                        Vector3 movingDir = (currentInputPos - firstInputPos).normalized;
                        Vector3 playerPos = transform.position;
                        playerPos += movingDir * currentSpeed * Time.deltaTime;
                        transform.position = playerPos;
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        currentSpeed = movementSpeed / 2f;
                        firstInputPos = Vector3.zero;
                    }


                    //Check for deadly objects and deadly objects
                    listDetectedTarget.Clear();
                    listDetectedDeadly.Clear();
                    Vector3 holeCenterPosition = GetHoleCenterWorldPosition();
                    float detectionRadius = GetHoleDetectionRadius();
                    Collider[] delectedColliders = Physics.OverlapSphere(holeCenterPosition, detectionRadius);
                    foreach (Collider collider in delectedColliders)
                    {
                        if (collider.CompareTag("Object"))
                        {
                            TargetObjectController targetObject = PoolManager.Instance.FindTargetObject(collider.transform);
                            if (targetObject != null
                                && !listDetectedTarget.Contains(targetObject)
                                && CanStartAbsorbObject(targetObject.ObjectCenterPosition, targetObject.ObjectSize))
                            {
                                listDetectedTarget.Add(targetObject);
                                targetObject.OnEnterPlayer(objectLayer);
                                if (!listCurrentTarget.Contains(targetObject)) { listCurrentTarget.Add(targetObject); }
                            }

                            DeadlyObjectController deadlyObject = PoolManager.Instance.FindDeadlyObject(collider.transform);
                            if(deadlyObject != null
                                && !listDetectedDeadly.Contains(deadlyObject)
                                && CanStartAbsorbObject(deadlyObject.ObjectCenterPosition, deadlyObject.ObjectSize))
                            {
                                listDetectedDeadly.Add(deadlyObject);
                                deadlyObject.OnEnterPlayer(objectLayer);
                                if (!listCurrentDeadly.Contains(deadlyObject)) { listCurrentDeadly.Add(deadlyObject); }
                            }
                        }
                    }

                    //Target objects fall out of the hole -> re-set to default layer
                    foreach (TargetObjectController target in listCurrentTarget)
                    {
                        if (!listDetectedTarget.Contains(target))
                        {
                            target.OnExitPlayer(defaultLayer);
                        }
                    }

                    //deadly objects fall out of the hole -> re-set to default layer
                    foreach (DeadlyObjectController deadly in listCurrentDeadly)
                    {
                        if (!listDetectedDeadly.Contains(deadly))
                        {
                            deadly.OnExitPlayer(defaultLayer);
                        }
                    }

                    //Update hole size based on targetHoleSize
                    currentHoleSize = holeParentTrans.localScale.x;
                    if (currentHoleSize != targetHoleSize)
                    {
                        float minimumHoleScale = AbsorbDiameterToScale(baseHoleDiameter);
                        float currentYScale = holeParentTrans.localScale.y;
                        currentHoleSize = Mathf.Clamp(currentHoleSize + Time.deltaTime, minimumHoleScale, targetHoleSize);
                        holeParentTrans.localScale = new Vector3(currentHoleSize, currentYScale, currentHoleSize);
                        CameraParentController.Instance.UpdateDistance(currentHoleSize);
                    }
                }
            }
        }



        /// <summary>
        /// Call PlayerState.Player_Living event and handle other actions.
        /// </summary>
        private void PlayerLiving()
        {
            //Fire event
            PlayerState = PlayerState.Player_Living;
            playerState = PlayerState.Player_Living;

            //Add other actions here
            if (IngameManager.Instance.IsRevived)
            {
                StartCoroutine(CRHandleActionsAfterRevived());
            }
            else
            {
                isStopControl = false;
            }
        }


        /// <summary>
        /// Call PlayerState.Player_Died event and handle other actions.
        /// </summary>
        public void PlayerDied()
        {
            //Fire event
            PlayerState = PlayerState.Player_Died;
            playerState = PlayerState.Player_Died;

            //Add other actions here
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerDied);
            ServicesManager.Instance.ShareManager.CreateScreenshot();
            CameraParentController.Instance.Shake();
            isStopControl = true;
        }



        /// <summary>
        /// Fire Player_CompletedLevel event and handle other actions.
        /// </summary>
        private void PlayerCompletedLevel()
        {
            //Fire event
            PlayerState = PlayerState.Player_CompletedLevel;
            playerState = PlayerState.Player_CompletedLevel;

            //Add others action here
            ServicesManager.Instance.ShareManager.CreateScreenshot();
        }


        /// <summary>
        /// Coroutine handle actions after player revived.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRHandleActionsAfterRevived()
        {
            yield return new WaitForSeconds(0.5f);
            isStopControl = false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////Public functions



        /// <summary>
        /// Determine whether the current hole size can absorb an object by physical size.
        /// </summary>
        /// <param name="objectSize"></param>
        /// <returns></returns>
        public bool CanAbsorbObject(float objectSize)
        {
            return objectSize <= currentHoleDiameter + HoleProgressionRules.SizeEpsilon;
        }


        /// <summary>
        /// Read-only world-space center of the black hole aperture.
        /// </summary>
        public Vector3 HoleCenterWorldPosition => GetHoleCenterWorldPosition();



        /// <summary>
        /// Register one collected target object and update hole growth by score thresholds.
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="objectSize"></param>
        /// <returns>Earned points for this collected object.</returns>
        public int RegisterCollectedTarget(string objectName, float objectSize)
        {
            int itemTier;
            if (!HoleProgressionRules.TryGetItemTierByObjectName(objectName, out itemTier))
            {
                itemTier = HoleProgressionRules.GetItemTierBySize(objectSize, baseHoleDiameter);
            }

            int earnedPoints = HoleProgressionRules.GetPointsForItemTier(itemTier);
            currentHoleScore += earnedPoints;

            int resolvedHoleLevel = HoleProgressionRules.GetHoleLevelByScore(currentHoleScore);
            if (resolvedHoleLevel > currentHoleLevel)
            {
                currentHoleLevel = resolvedHoleLevel;
                currentHoleDiameter = HoleProgressionRules.GetHoleDiameter(baseHoleDiameter, currentHoleLevel);
                targetHoleSize = AbsorbDiameterToScale(currentHoleDiameter);
            }

            return earnedPoints;
        }



        /// <summary>
        /// Set the movement speed for the player.
        /// </summary>
        /// <param name="speed"></param>
        public void SetMovementSpeed(float speed)
        {
            movementSpeed = speed;
        }



        /// <summary>
        /// Update the size of the hole with amount. 
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateHoleSize(float amount)
        {
            float targetDiameter = ScaleToAbsorbDiameter(targetHoleSize);
            targetDiameter = Mathf.Clamp(targetDiameter + amount, baseHoleDiameter, 100f);
            targetHoleSize = AbsorbDiameterToScale(targetDiameter);
            currentHoleDiameter = Mathf.Max(currentHoleDiameter, targetDiameter);

            if (holeFireEffects != null)
            {
                for(int i = 0; i < holeFireEffects.Length; i++)
                {
                    ParticleSystem holeEffect = holeFireEffects[i];
                    if (holeEffect == null || !holeEffect.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    //Adjust lifetime
                    var main = holeEffect.main;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(targetHoleSize / 2f, targetHoleSize);

                    //Adjust redius
                    var shape = holeEffect.shape;
                    shape.radius = targetHoleSize * 0.6f;

                    //Adjust emission count
                    var parEmission = holeEffect.emission;
                    parEmission.rateOverTime = Mathf.RoundToInt(targetHoleSize * 30f);

                    //Adjust size
                    if (i == 0)
                    {
                        main.startSize3D = true;
                        main.startSizeX = targetHoleSize * 3f;
                        main.startSizeY = targetHoleSize * 3f;
                        main.startSizeZ = targetHoleSize * 4f;
                    }
                    else if (i == 1)
                    {
                        main.startSize = targetHoleSize * 0.1f;
                    }
                    else
                    {
                        main.startSize = new ParticleSystem.MinMaxCurve(targetHoleSize, targetHoleSize * 3f);
                    }
                }
            }

            if (fireEffectTrans != null && fireEffectTrans.gameObject.activeInHierarchy)
            {
                float newY = -(targetHoleSize * 1.8f);
                fireEffectTrans.localPosition = new Vector3(fireEffectTrans.localPosition.x, newY, fireEffectTrans.localPosition.z);
            }

            //Create effect
            EffectManager.Instance.CreateTargetObjectExplode(transform.position + Vector3.up * 0.15f, currentHoleSize);
        }



        /// <summary>
        /// Handle actions when the player collected a deadly object.
        /// </summary>
        public void OnCollectedDeadlyObject()
        {
            isStopControl = true;
            PlayerDied();
            IngameManager.Instance.HandlePlayerDied();
        }



        /// <summary>
        /// Create the cash effect with amount.
        /// </summary>
        /// <param name="amount"></param>
        public void CreateCashEffect(int amount)
        {
            EffectManager.Instance.CreateCashEffect(transform.position + Vector3.down * 0.5f, amount);
        }
    }
}
