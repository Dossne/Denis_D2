using System.Collections;
using UnityEngine;


namespace ClawbearGames
{
    public class DeadlyObjectController : MonoBehaviour
    {
        [Header("Deadly Object References")]
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


        /// <summary>
        /// Handle actions of this deadly object when its collide with the player.
        /// </summary>
        /// <param name="objectLayer"></param>
        public void OnEnterPlayer(string objectLayer)
        {
            gameObject.layer = LayerMask.NameToLayer(objectLayer);
            rigidbody3D.WakeUp();

            if (physicsPullCount < 5)
            {
                physicsPullCount++;

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
        /// Handle actions of this deadly object exit collision with the player.
        /// </summary>
        /// <param name="defaultLayer"></param>
        public void OnExitPlayer(string defaultLayer)
        {
            cRCheckFall = null;
            physicsPullCount = 0;
            gameObject.layer = LayerMask.NameToLayer(defaultLayer);
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
                    physicsPullCount = 0;
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
