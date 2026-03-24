using System.Collections;
using UnityEngine;


namespace ClawbearGames
{
    public class DeadlyObjectController : MonoBehaviour
    {
        private const string autoJumpObjectName = "Deadly_Object_1";
        private const string enterFallbackLayerName = "Ignore Raycast";
        private const string exitFallbackLayerName = "Default";
        private const float autoJumpInterval = 1f;
        private const float jumpForwardDistanceMultiplier = 1.5f;
        private const float gravityFallback = 9.81f;
        private const float physicsEpsilon = 0.0001f;

        [Header("Deadly Object References")]
        [SerializeField] private string objectName = string.Empty;
        [SerializeField] private Rigidbody rigidbody3D = null;
        [SerializeField] private MeshRenderer meshRenderer = null;


        public string ObjectName => objectName;
        public Vector3 ObjectCenterPosition => meshRenderer != null ? meshRenderer.bounds.center : transform.position;
        public float ObjectSize
        {
            get
            {
                if (meshRenderer == null)
                {
                    return 0f;
                }

                Vector3 localSize = meshRenderer.localBounds.size;
                Vector3 lossyScale = transform.lossyScale;
                float sizeX = localSize.x * Mathf.Abs(lossyScale.x);
                float sizeZ = localSize.z * Mathf.Abs(lossyScale.z);
                return Mathf.Max(sizeX, sizeZ);
            }
        }
        private Coroutine cRCheckFall = null;
        private Coroutine cRAutoJump = null;
        private int physicsPullCount = 0;
        private bool isBeingConsumed = false;

        private void OnEnable()
        {
            cRCheckFall = null;
            cRAutoJump = null;
            physicsPullCount = 0;
            isBeingConsumed = false;

            if (rigidbody3D != null)
            {
                rigidbody3D.detectCollisions = true;
            }

            SetLayerSafe(exitFallbackLayerName, exitFallbackLayerName);

            if (IsAutoJumpEnabled())
            {
                cRAutoJump = StartCoroutine(CRAutoJumpTowardHole());
            }
        }

        private void OnDisable()
        {
            StopAutoJump();
            cRCheckFall = null;
        }


        /// <summary>
        /// Handle actions of this deadly object when its collide with the player.
        /// </summary>
        /// <param name="objectLayer"></param>
        public void OnEnterPlayer(string objectLayer)
        {
            isBeingConsumed = true;
            StopAutoJump();
            SetLayerSafe(objectLayer, enterFallbackLayerName);
            rigidbody3D.detectCollisions = false;
            rigidbody3D.WakeUp();

            if (physicsPullCount < 5)
            {
                physicsPullCount++;

                //Pull this target object toward the center of the player
                Vector3 holeCenter = PlayerController.Instance != null
                    ? PlayerController.Instance.HoleCenterWorldPosition
                    : transform.position;
                Vector3 pullDir = (holeCenter - transform.position).normalized;
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
        /// Handle actions of this deadly object exit collision with the player.
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

        private bool IsAutoJumpEnabled()
        {
            return string.Equals(objectName, autoJumpObjectName, System.StringComparison.Ordinal);
        }

        private bool CanExecuteAutoJump()
        {
            if (isBeingConsumed || rigidbody3D == null || rigidbody3D.isKinematic)
            {
                return false;
            }

            if (!gameObject.activeInHierarchy || PlayerController.Instance == null || IngameManager.Instance == null)
            {
                return false;
            }

            return IngameManager.Instance.IngameState == IngameState.Ingame_Playing;
        }

        private void StopAutoJump()
        {
            if (cRAutoJump != null)
            {
                StopCoroutine(cRAutoJump);
                cRAutoJump = null;
            }
        }

        private IEnumerator CRAutoJumpTowardHole()
        {
            WaitForSeconds wait = new WaitForSeconds(autoJumpInterval);
            while (gameObject.activeInHierarchy)
            {
                yield return wait;

                if (!CanExecuteAutoJump())
                {
                    continue;
                }

                ExecuteJumpTowardHole();
            }
        }

        private void ExecuteJumpTowardHole()
        {
            float diameter = Mathf.Max(ObjectSize, physicsEpsilon);
            float jumpHeight = diameter;
            float forwardDistance = jumpForwardDistanceMultiplier * diameter;

            Vector3 holeCenter = PlayerController.Instance.HoleCenterWorldPosition;
            Vector3 toHole = holeCenter - transform.position;
            Vector3 dirXZ = new Vector3(toHole.x, 0f, toHole.z);
            if (dirXZ.sqrMagnitude > physicsEpsilon)
            {
                dirXZ.Normalize();
            }
            else
            {
                dirXZ = Vector3.zero;
            }

            float g = Mathf.Abs(Physics.gravity.y);
            if (g < physicsEpsilon)
            {
                g = gravityFallback;
            }

            float vy = Mathf.Sqrt(2f * g * jumpHeight);
            float flightTime = Mathf.Max((2f * vy) / g, physicsEpsilon);
            float vxz = forwardDistance / flightTime;

            rigidbody3D.velocity = (dirXZ * vxz) + (Vector3.up * vy);
        }



        /// <summary>
        /// Coroutine check this deadly object fall down and disable.
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
                    physicsPullCount = 0;
                    rigidbody3D.detectCollisions = true;
                    SetLayerSafe(exitFallbackLayerName, exitFallbackLayerName);
                    PlayerController.Instance.OnCollectedDeadlyObject();
                    gameObject.SetActive(false);
                }
                yield return null;
            }
        }



        /// <summary>
        /// Update the position of this deadly object on UI map.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRUpdatePositionOnUIMap()
        {
            while (cRCheckFall != null)
            {
                //Update position on UI map
                ViewManager.Instance.IngameViewController.UpdateDeadlyObjectPos(this);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
