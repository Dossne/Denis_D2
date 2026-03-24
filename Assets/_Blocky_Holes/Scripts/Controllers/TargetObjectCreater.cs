using System.Collections.Generic;
using UnityEngine;


namespace ClawbearGames
{
    public class TargetObjectCreater : MonoBehaviour
    {
        [SerializeField][Range(2, 50)] public int verticalObjectAmount = 1;
        [SerializeField][Range(2, 50)] private int horizontalObjectAmount = 2;
        [SerializeField][Range(1, 10)] private int verticalObjectSpacing = 1;
        [SerializeField][Range(1, 10)] private int horizontalObjectSpacing = 1;
        [SerializeField] private TargetObjectController[] ballObjectPrefabs = null;
        [SerializeField] private TargetObjectController[] hatObjectPrefabs = null;
        [SerializeField] private TargetObjectController[] graveObjectPrefabs = null;
        [SerializeField] private TargetObjectController[] giftBoxObjectPrefabs = null;
        [SerializeField] private TargetObjectController[] pineObjectPrefabs = null;
        [SerializeField] private TargetObjectController[] carObjectPrefabs = null;
        [SerializeField] private TargetObjectController[] houseObjectPrefabs = null;
        [SerializeField] private TargetObjectController[] buildingObjectPrefabs = null;

        [SerializeField] private bool drawBallPrefab = false;
        [SerializeField] private bool drawHatPrefab = false;
        [SerializeField] private bool drawGravePrefab = false;
        [SerializeField] private bool drawGiftBoxPrefab = false;
        [SerializeField] private bool drawPinePrefab = false;
        [SerializeField] private bool drawCarPrefab = false;
        [SerializeField] private bool drawHousePrefab = false;
        [SerializeField] private bool drawBuildingPrefab = false;

        public bool DrawGizmos { set { drawGizmos = value; } }
        private bool drawGizmos = true;
        private void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                TargetObjectController firstTargetObjectPrefab = null;
                if (drawBallPrefab) { firstTargetObjectPrefab = ballObjectPrefabs[0]; }
                else if (drawHatPrefab) { firstTargetObjectPrefab = hatObjectPrefabs[0]; }
                else if (drawGravePrefab) { firstTargetObjectPrefab = graveObjectPrefabs[0]; }
                else if (drawGiftBoxPrefab) { firstTargetObjectPrefab = giftBoxObjectPrefabs[0]; }
                else if (drawPinePrefab) { firstTargetObjectPrefab = pineObjectPrefabs[0]; }
                else if (drawCarPrefab) { firstTargetObjectPrefab = carObjectPrefabs[0]; }
                else if (drawHousePrefab) { firstTargetObjectPrefab = houseObjectPrefabs[0]; }
                else if (drawBuildingPrefab) { firstTargetObjectPrefab = buildingObjectPrefabs[0]; }

                if (firstTargetObjectPrefab != null)
                {
                    Vector3 objectExtents = firstTargetObjectPrefab.GetComponent<MeshRenderer>().bounds.extents;
                    List<Vector3> listObjectPos = CalculateObjectPositions(firstTargetObjectPrefab);

                    //Draw the wire cube for positions
                    foreach (Vector3 pos in listObjectPos)
                    {
                        Gizmos.DrawWireCube(new Vector3(pos.x, 2f, pos.z), new Vector3(objectExtents.x * 2f, 4f, objectExtents.z * 2f));
                    }
                }
            }
        }



        /// <summary>
        /// Calculate the positions for the target object based on configed parameters.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <returns></returns>
        private List<Vector3> CalculateObjectPositions(TargetObjectController targetObject)
        {
            Vector3 objectExtents = targetObject.GetComponent<MeshRenderer>().bounds.extents;
            List<Vector3> listObjectPos = new List<Vector3>();
            if (horizontalObjectAmount % 2 == 0)
            {
                Vector3 leftPos = transform.position + Vector3.left * (objectExtents.x + (horizontalObjectSpacing / 2f));
                Vector3 rightPos = transform.position + Vector3.right * (objectExtents.x + (horizontalObjectSpacing / 2f));
                listObjectPos.Add(leftPos);
                listObjectPos.Add(rightPos);

                int horizontalCount = (horizontalObjectAmount - 1) / 2;
                for (int i = 1; i <= horizontalCount; i++)
                {
                    listObjectPos.Add(leftPos + Vector3.left * (objectExtents.x * 2f + horizontalObjectSpacing) * i);
                    listObjectPos.Add(rightPos + Vector3.right * (objectExtents.x * 2f + horizontalObjectSpacing) * i);
                }
            }
            else
            {
                listObjectPos.Add(transform.position);
                int horizontalCount = (horizontalObjectAmount - 1) / 2;
                for (int i = 1; i <= horizontalCount; i++)
                {
                    listObjectPos.Add(transform.position + Vector3.left * (objectExtents.x * 2f + (horizontalObjectSpacing / 2f)) * i);
                    listObjectPos.Add(transform.position + Vector3.right * (objectExtents.x * 2f + (horizontalObjectSpacing / 2f)) * i);
                }
            }

            if (verticalObjectAmount % 2 == 0)
            {
                int forwardCount = (verticalObjectAmount - 1) / 2;
                List<Vector3> listForwardPos = new List<Vector3>();
                List<Vector3> listBackPos = new List<Vector3>();
                foreach (Vector3 pos in listObjectPos)
                {
                    listForwardPos.Add(pos + Vector3.forward * (objectExtents.z + (verticalObjectSpacing / 2f)));
                    listBackPos.Add(pos + Vector3.back * (objectExtents.z + (verticalObjectSpacing / 2f)));
                }

                List<Vector3> listAddedForward = new List<Vector3>();
                List<Vector3> listAddedBack = new List<Vector3>();
                for (int i = 1; i <= forwardCount; i++)
                {
                    foreach (Vector3 pos in listForwardPos)
                    {
                        listAddedForward.Add(pos + Vector3.forward * (objectExtents.z * 2f + verticalObjectSpacing) * i);
                    }
                    foreach (Vector3 pos in listBackPos)
                    {
                        listAddedBack.Add(pos + Vector3.back * (objectExtents.z * 2f + verticalObjectSpacing) * i);
                    }
                }

                listObjectPos.Clear();
                listObjectPos.AddRange(listForwardPos);
                listObjectPos.AddRange(listBackPos);
                listObjectPos.AddRange(listAddedForward);
                listObjectPos.AddRange(listAddedBack);
            }
            else
            {
                int forwardCount = (verticalObjectAmount - 1) / 2;
                List<Vector3> listForwardPos = new List<Vector3>();
                for (int i = 1; i <= forwardCount; i++)
                {
                    foreach (Vector3 pos in listObjectPos)
                    {
                        listForwardPos.Add(pos + Vector3.forward * (objectExtents.z * 2f + (verticalObjectSpacing / 2f)) * i);
                        listForwardPos.Add(pos + Vector3.back * (objectExtents.z * 2f + (verticalObjectSpacing / 2f)) * i);
                    }
                }
                listObjectPos.AddRange(listForwardPos);
            }
            return listObjectPos;
        }



        /// <summary>
        /// Create the ball objects using the configed parameters.
        /// </summary>
        public void CreateBallObjects()
        {
            //Creat the objects
            List<Vector3> listObjectPos = CalculateObjectPositions(ballObjectPrefabs[0]);
            foreach (Vector3 pos in listObjectPos)
            {
                TargetObjectController ballObject = GetBallObjectController();
                ballObject.transform.position = pos;
            }
        }


        /// <summary>
        /// Create the hat objects using the configed parameters.
        /// </summary>
        public void CreateHatObjects()
        {
            //Creat the objects
            List<Vector3> listObjectPos = CalculateObjectPositions(hatObjectPrefabs[0]);
            foreach (Vector3 pos in listObjectPos)
            {
                TargetObjectController targetObject = GetHatObjectController();
                targetObject.transform.position = pos;
            }
        }


        /// <summary>
        /// Create the grave objects using the configed parameters.
        /// </summary>
        public void CreateGraveObjects()
        {
            //Creat the objects
            List<Vector3> listObjectPos = CalculateObjectPositions(graveObjectPrefabs[0]);
            foreach (Vector3 pos in listObjectPos)
            {
                TargetObjectController targetObject = GetGraveObjectController();
                targetObject.transform.position = pos;
            }
        }


        /// <summary>
        /// Create the gift box objects using the configed parameters.
        /// </summary>
        public void CreateGiftBoxObjects()
        {
            //Creat the objects
            List<Vector3> listObjectPos = CalculateObjectPositions(giftBoxObjectPrefabs[0]);
            foreach (Vector3 pos in listObjectPos)
            {
                TargetObjectController targetObject = GetGiftBoxObjectController();
                targetObject.transform.position = pos;
            }
        }



        /// <summary>
        /// Create the pine objects using the configed parameters.
        /// </summary>
        public void CreatePineObjects()
        {
            //Creat the objects
            List<Vector3> listObjectPos = CalculateObjectPositions(pineObjectPrefabs[0]);
            foreach (Vector3 pos in listObjectPos)
            {
                TargetObjectController targetObject = GetPineObjectController();
                targetObject.transform.position = pos;
            }
        }


        /// <summary>
        /// Create the car objects using the configed parameters.
        /// </summary>
        public void CreateCarObjects()
        {
            //Creat the objects
            List<Vector3> listObjectPos = CalculateObjectPositions(carObjectPrefabs[0]);
            foreach (Vector3 pos in listObjectPos)
            {
                TargetObjectController targetObject = GetCarObjectController();
                targetObject.transform.position = pos;
            }
        }


        /// <summary>
        /// Create the house objects using the configed parameters.
        /// </summary>
        public void CreateHouseObjects()
        {
            //Creat the objects
            List<Vector3> listObjectPos = CalculateObjectPositions(houseObjectPrefabs[0]);
            foreach (Vector3 pos in listObjectPos)
            {
                TargetObjectController targetObject = GetHouseObjectController();
                targetObject.transform.position = pos;
            }
        }


        /// <summary>
        /// Create the building objects using the configed parameters.
        /// </summary>
        public void CreateBuildingObjects()
        {
            //Creat the objects
            List<Vector3> listObjectPos = CalculateObjectPositions(buildingObjectPrefabs[0]);
            foreach (Vector3 pos in listObjectPos)
            {
                TargetObjectController targetObject = GetBuildingObjectController();
                targetObject.transform.position = pos;
            }
        }


        /// <summary>
        /// Get a random TargetObjectController from the ball prefab array.
        /// </summary>
        /// <returns></returns>
        private TargetObjectController GetBallObjectController()
        {
            TargetObjectController prefab = ballObjectPrefabs[Random.Range(0, ballObjectPrefabs.Length)];
            TargetObjectController targetObject = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0f));
            targetObject.gameObject.name = prefab.name;
            return targetObject;
        }



        /// <summary>
        /// Get a random TargetObjectController from the hat prefab array.
        /// </summary>
        /// <returns></returns>
        private TargetObjectController GetHatObjectController()
        {
            TargetObjectController prefab = hatObjectPrefabs[Random.Range(0, hatObjectPrefabs.Length)];
            TargetObjectController targetObject = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0f));
            targetObject.gameObject.name = prefab.name;
            return targetObject;
        }


        /// <summary>
        /// Get a random TargetObjectController from the grave prefab array.
        /// </summary>
        /// <returns></returns>
        private TargetObjectController GetGraveObjectController()
        {
            TargetObjectController prefab = graveObjectPrefabs[Random.Range(0, graveObjectPrefabs.Length)];
            TargetObjectController targetObject = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0f));
            targetObject.gameObject.name = prefab.name;
            return targetObject;
        }


        /// <summary>
        /// Get a random TargetObjectController from the gift box prefab array.
        /// </summary>
        /// <returns></returns>
        private TargetObjectController GetGiftBoxObjectController()
        {
            TargetObjectController prefab = giftBoxObjectPrefabs[Random.Range(0, giftBoxObjectPrefabs.Length)];
            TargetObjectController targetObject = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0f));
            targetObject.gameObject.name = prefab.name;
            return targetObject;
        }



        /// <summary>
        /// Get a random TargetObjectController from the pine prefab array.
        /// </summary>
        /// <returns></returns>
        private TargetObjectController GetPineObjectController()
        {
            TargetObjectController prefab = pineObjectPrefabs[Random.Range(0, pineObjectPrefabs.Length)];
            TargetObjectController targetObject = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0f));
            targetObject.gameObject.name = prefab.name;
            return targetObject;
        }



        /// <summary>
        /// Get a random TargetObjectController from the car prefab array.
        /// </summary>
        /// <returns></returns>
        private TargetObjectController GetCarObjectController()
        {
            TargetObjectController prefab = carObjectPrefabs[Random.Range(0, carObjectPrefabs.Length)];
            TargetObjectController targetObject = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0f));
            targetObject.gameObject.name = prefab.name;
            return targetObject;
        }



        /// <summary>
        /// Get a random TargetObjectController from the house prefab array.
        /// </summary>
        /// <returns></returns>
        private TargetObjectController GetHouseObjectController()
        {
            TargetObjectController prefab = houseObjectPrefabs[Random.Range(0, houseObjectPrefabs.Length)];
            TargetObjectController targetObject = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0f));
            targetObject.gameObject.name = prefab.name;
            return targetObject;
        }


        /// <summary>
        /// Get a random TargetObjectController from the building prefab array.
        /// </summary>
        /// <returns></returns>
        private TargetObjectController GetBuildingObjectController()
        {
            TargetObjectController prefab = buildingObjectPrefabs[Random.Range(0, buildingObjectPrefabs.Length)];
            TargetObjectController targetObject = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0f));
            targetObject.gameObject.name = prefab.name;
            return targetObject;
        }
    }
}
