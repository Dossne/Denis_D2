using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class TargetObjectController : MonoBehaviour
    {
        private const string enterFallbackLayerName = "Ignore Raycast";
        private const string exitFallbackLayerName = "Default";

        [Header("Target Object Configuration")]
        [SerializeField][Range(1, 50)] private int minCashRewardAmount = 1;
        [SerializeField][Range(1, 50)] private int maxCashRewardAmount = 1;
        [SerializeField][Range(0f, 1f)] private float cashRewardFrequency = 0.5f;

        [Header("Target Object References")]
        [SerializeField] private string objectName = string.Empty;
        [SerializeField] private Rigidbody rigidbody3D = null;
        [SerializeField] private MeshRenderer meshRenderer = null;

        public string ObjectName => objectName;
        public float ObjectSize
        {
            get
            {
                if (meshRenderer.bounds.size.x > meshRenderer.bounds.size.z)
                    return meshRenderer.bounds.size.x;
                else return meshRenderer.bounds.size.z;
            }
        }
        private Coroutine cRCheckFall = null;
        private int physicsPullCount = 0;
        private bool isBeingConsumed = false;
        private Vector3 basePrefabScale = Vector3.one;
        private bool isBasePrefabScaleCached = false;

        private void Awake()
        {
            CacheBasePrefabScale();
        }

        private void OnEnable()
        {
            cRCheckFall = null;
            physicsPullCount = 0;
            isBeingConsumed = false;

            if (rigidbody3D != null)
            {
                rigidbody3D.detectCollisions = true;
            }

            SetLayerSafe(exitFallbackLayerName, exitFallbackLayerName);
        }

        /// <summary>
        /// Apply spawn scale as prefab base scale multiplied by level data scale.
        /// </summary>
        /// <param name="levelScale"></param>
        public void ApplySpawnScale(Vector3 levelScale)
        {
            CacheBasePrefabScale();
            transform.localScale = Vector3.Scale(basePrefabScale, levelScale);
        }

        private void CacheBasePrefabScale()
        {
            if (isBasePrefabScaleCached)
            {
                return;
            }

            basePrefabScale = transform.localScale;
            isBasePrefabScaleCached = true;
        }


        /// <summary>
        /// Handle actions of this target object when its collide with the player.
        /// </summary>
        /// <param name="objectLayer"></param>
        public void OnEnterPlayer(string objectLayer)
        {
            isBeingConsumed = true;
            SetLayerSafe(objectLayer, enterFallbackLayerName);
            rigidbody3D.detectCollisions = false;
            rigidbody3D.WakeUp();

            if (physicsPullCount < 15 || transform.position.y > -0.1)
            {
                physicsPullCount++;
                if (physicsPullCount < 2)
                {
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.TargetObjectCollected);
                }

                //Pull this target object toward the center of the player
                Vector3 pullDir = (PlayerController.Instance.transform.position - transform.position).normalized;
                rigidbody3D.AddForce(pullDir * 10f);
            }

            //Check falldown
            if (cRCheckFall == null)
            {
                cRCheckFall = StartCoroutine(CRCheckFalldown());
                StartCoroutine(CRUpdatePositionOnUIMap());
            }
        }


        /// <summary>
        /// Handle actions of this target object exit collision with the player.
        /// </summary>
        /// <param name="defaultLayer"></param>
        public void OnExitPlayer(string defaultLayer)
        {
            if (isBeingConsumed)
            {
                return;
            }

            cRCheckFall = null;
            physicsPullCount = 0;
            rigidbody3D.detectCollisions = true;
            SetLayerSafe(defaultLayer, exitFallbackLayerName);
        }

        /// <summary>
        /// Safely set layer by name and fallback to Default if needed.
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="fallbackLayerName"></param>
        private void SetLayerSafe(string layerName, string fallbackLayerName)
        {
            int layerIndex = LayerMask.NameToLayer(layerName);
            if (layerIndex >= 0)
            {
                gameObject.layer = layerIndex;
                return;
            }

            int fallbackLayerIndex = LayerMask.NameToLayer(fallbackLayerName);
            if (fallbackLayerIndex >= 0)
            {
                gameObject.layer = fallbackLayerIndex;
            }
        }



        /// <summary>
        /// Coroutine check this target object fall down and disable.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRCheckFalldown()
        {
            while (!rigidbody3D.isKinematic)
            {
                if (transform.position.y <= -3f) 
                {
                    cRCheckFall = null;
                    isBeingConsumed = false;

                    //Update the player
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.TargetObjectDestroyed);
                    PlayerController.Instance.RegisterCollectedTarget(ObjectSize);
                    ViewManager.Instance.IngameViewController.RemoveTargetObjectDot(this);
                    IngameManager.Instance.OnPlayerAteTargetObject();

                    //Create cash effect
                    if (Random.value <= cashRewardFrequency)
                    {
                        int cashAmount = Random.Range(minCashRewardAmount, maxCashRewardAmount);
                        PlayerController.Instance.CreateCashEffect(cashAmount);
                    }

                    physicsPullCount = 0;
                    rigidbody3D.detectCollisions = true;
                    SetLayerSafe(exitFallbackLayerName, exitFallbackLayerName);
                    gameObject.SetActive(false); 
                }
                yield return null;
            }
        }


        /// <summary>
        /// Update the position of this target object on UI map.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRUpdatePositionOnUIMap()
        {
            while (cRCheckFall != null)
            {
                //Update position on UI map
                ViewManager.Instance.IngameViewController.UpdateTargetObjectPos(this);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
