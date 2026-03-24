using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ClawbearGames
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { private set; get; }

        [SerializeField] private Texture2D[] groundTextures = null;
        [SerializeField] private TargetObjectController[] targetObjectControllerPrefabs = null;
        [SerializeField] private DeadlyObjectController[] deadlyObjectControllerPrefabs = null;

        private List<TargetObjectController> listTargetObjectController = new List<TargetObjectController>();
        private List<DeadlyObjectController> listDeadlyObjectController = new List<DeadlyObjectController>();

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



        /// <summary>
        /// Find the TargetObjectController object with given transform.
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public TargetObjectController FindTargetObject(Transform trans)
        {
            TargetObjectController objectResult = null;
            foreach(TargetObjectController targetObject in listTargetObjectController)
            {
                if (targetObject.transform.Equals(trans))
                {
                    objectResult = targetObject;
                    break;
                }
            }
            return objectResult;
        }


        /// <summary>
        /// Find the DeadlyObjectController object with given transform.
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public DeadlyObjectController FindDeadlyObject(Transform trans)
        {
            DeadlyObjectController objectResult = null;
            foreach (DeadlyObjectController deadlyObject in listDeadlyObjectController)
            {
                if (deadlyObject.transform.Equals(trans))
                {
                    objectResult = deadlyObject;
                    break;
                }
            }
            return objectResult;
        }



        /// <summary>
        /// Get the ground texture with name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Texture2D GetGroundTexture(string name)
        {
            foreach (Texture2D texture in groundTextures)
            {
                if (texture.name.Equals(name)) { return texture; }
            }
            return null;
        }



        /// <summary>
        /// Get an inactive TargetObjectController object.
        /// </summary>
        /// <returns></returns>
        public TargetObjectController GetTargetObjectController(string objectName)
        {
            //Find in the list
            TargetObjectController targetObject = listTargetObjectController.Where(a => !a.gameObject.activeSelf && a.ObjectName.Equals(objectName)).FirstOrDefault();

            if (targetObject == null)
            {
                //Didn't find one -> create new one
                TargetObjectController prefab = targetObjectControllerPrefabs.Where(a => a.ObjectName.Equals(objectName)).FirstOrDefault();
                targetObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                targetObject.gameObject.SetActive(false);
                listTargetObjectController.Add(targetObject);
            }

            return targetObject;
        }


        /// <summary>
        /// Get an inactive DeadlyObjectController object.
        /// </summary>
        /// <returns></returns>
        public DeadlyObjectController GetDeadlyObjectController(string objectName)
        {
            //Find in the list
            DeadlyObjectController deadlyObject = listDeadlyObjectController.Where(a => !a.gameObject.activeSelf && a.ObjectName.Equals(objectName)).FirstOrDefault();

            if (deadlyObject == null)
            {
                //Didn't find one -> create new one
                DeadlyObjectController prefab = deadlyObjectControllerPrefabs.Where(a => a.ObjectName.Equals(objectName)).FirstOrDefault();
                deadlyObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                deadlyObject.gameObject.SetActive(false);
                listDeadlyObjectController.Add(deadlyObject);
            }

            return deadlyObject;
        }

    }
}
