using System.Collections.Generic;
using UnityEngine;


namespace ClawbearGames
{
    public class DeadlyObjectCreater : MonoBehaviour
    {
        [SerializeField][Range(2, 50)] public int verticalObjectAmount = 1;
        [SerializeField][Range(2, 50)] private int horizontalObjectAmount = 2;
        [SerializeField][Range(1, 10)] private int verticalObjectSpacing = 1;
        [SerializeField][Range(1, 10)] private int horizontalObjectSpacing = 1;
        [SerializeField][Range(1, 10)] private int objectScale = 1;
        [SerializeField] private DeadlyObjectController[] deadlyObjectPrefabs = null;

        public bool DrawGizmos { set { drawGizmos = value; } }
        private bool drawGizmos = true;

        private void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                if (deadlyObjectPrefabs != null)
                {
                    if (deadlyObjectPrefabs.Length > 0)
                    {
                        Vector3 objectExtents = deadlyObjectPrefabs[0].GetComponent<MeshRenderer>().bounds.extents;
                        List<Vector3> listObjectPos = CalculateObjectPositions();

                        //Draw the wire cube for positions
                        foreach (Vector3 pos in listObjectPos)
                        {
                            Gizmos.DrawWireCube(new Vector3(pos.x, 2f, pos.z), new Vector3(objectExtents.x * 2f * objectScale, 4f, objectExtents.z * 2f * objectScale));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Calculate the positions for the deadly object based on configed parameters.
        /// </summary>
        /// <returns></returns>
        private List<Vector3> CalculateObjectPositions()
        {
            Vector3 objectExtents = deadlyObjectPrefabs[0].GetComponent<MeshRenderer>().bounds.extents;
            List<Vector3> listObjectPos = new List<Vector3>();
            if (horizontalObjectAmount % 2 == 0)
            {
                Vector3 leftPos = transform.position + Vector3.left * (objectExtents.x * objectScale + (horizontalObjectSpacing / 2f));
                Vector3 rightPos = transform.position + Vector3.right * (objectExtents.x * objectScale + (horizontalObjectSpacing / 2f));
                listObjectPos.Add(leftPos);
                listObjectPos.Add(rightPos);

                int horizontalCount = (horizontalObjectAmount - 1) / 2;
                for (int i = 1; i <= horizontalCount; i++)
                {
                    listObjectPos.Add(leftPos + Vector3.left * (objectExtents.x * 2f * objectScale + horizontalObjectSpacing) * i);
                    listObjectPos.Add(rightPos + Vector3.right * (objectExtents.x * 2f * objectScale + horizontalObjectSpacing) * i);
                }
            }
            else
            {
                listObjectPos.Add(transform.position);
                int horizontalCount = (horizontalObjectAmount - 1) / 2;
                for (int i = 1; i <= horizontalCount; i++)
                {
                    listObjectPos.Add(transform.position + Vector3.left * (objectExtents.x * 2f * objectScale + (horizontalObjectSpacing / 2f)) * i);
                    listObjectPos.Add(transform.position + Vector3.right * (objectExtents.x * 2f * objectScale + (horizontalObjectSpacing / 2f)) * i);
                }
            }

            if (verticalObjectAmount % 2 == 0)
            {
                int forwardCount = (verticalObjectAmount - 1) / 2;
                List<Vector3> listForwardPos = new List<Vector3>();
                List<Vector3> listBackPos = new List<Vector3>();
                foreach (Vector3 pos in listObjectPos)
                {
                    listForwardPos.Add(pos + Vector3.forward * (objectExtents.z * objectScale + (verticalObjectSpacing / 2f)));
                    listBackPos.Add(pos + Vector3.back * (objectExtents.z * objectScale + (verticalObjectSpacing / 2f)));
                }

                List<Vector3> listAddedForward = new List<Vector3>();
                List<Vector3> listAddedBack = new List<Vector3>();
                for (int i = 1; i <= forwardCount; i++)
                {
                    foreach (Vector3 pos in listForwardPos)
                    {
                        listAddedForward.Add(pos + Vector3.forward * (objectExtents.z * 2f * objectScale + verticalObjectSpacing) * i);
                    }
                    foreach (Vector3 pos in listBackPos)
                    {
                        listAddedBack.Add(pos + Vector3.back * (objectExtents.z * 2f * objectScale + verticalObjectSpacing) * i);
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
                        listForwardPos.Add(pos + Vector3.forward * (objectExtents.z * 2f * objectScale + (verticalObjectSpacing / 2f)) * i);
                        listForwardPos.Add(pos + Vector3.back * (objectExtents.z * 2f * objectScale + (verticalObjectSpacing / 2f)) * i);
                    }
                }
                listObjectPos.AddRange(listForwardPos);
            }
            return listObjectPos;
        }




        /// <summary>
        /// Create the deadly objects using the configed parameters.
        /// </summary>
        public void CreateDeadlyObjects()
        {
            //Creat the objects
            List<Vector3> listObjectPos = CalculateObjectPositions();
            foreach(Vector3 pos in listObjectPos)
            {
                DeadlyObjectController deadlyObject = GetDeadlyObjectController();
                deadlyObject.transform.localScale = Vector3.one * objectScale;
                deadlyObject.transform.position = pos;
            }
        }





        /// <summary>
        /// Get a random DeadlyObjectController from the prefab array.
        /// </summary>
        /// <returns></returns>
        private DeadlyObjectController GetDeadlyObjectController()
        {
            DeadlyObjectController prefab = deadlyObjectPrefabs[Random.Range(0, deadlyObjectPrefabs.Length)];
            DeadlyObjectController deadlyObject = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0f));
            deadlyObject.gameObject.name = prefab.name;
            return deadlyObject;
        }
    }
}
