using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

namespace ClawbearGames
{
    public class LevelManager : MonoBehaviour
    {

        [Header("Levelmanager References")]
        [SerializeField] private Material groundMaterial = null;
        [SerializeField] private Texture2D[] groundTextures = null;
        [SerializeField] private TargetObjectController[] targetObjectPrefabs = null;
        [SerializeField] private DeadlyObjectController[] deadlyObjectPrefabs = null;


        [HideInInspector] public int targetObjectAmount = 1;
        [HideInInspector] public int timeToCompleteLevel = 30;
        [HideInInspector] public float playerMovementSpeed = 5f;

        public Texture2D[] GroundTextures => groundTextures;
        public Material GroundMaterial => groundMaterial;

        public int TotalLevel => Resources.LoadAll("Levels/").Length;

        public int TotalObjectAmount => FindObjectsByType<TargetObjectController>(FindObjectsSortMode.None).Length;

        public static string JsonPath(int levelNumber)
        {
            return "Assets/_Blocky_Holes/Resources/Levels/" + levelNumber.ToString() + ".json";
        }

        public static string ScreenshotPath(int levelNumber)
        {
            string path = "Assets/_Blocky_Holes/Sprites/Levels/" + levelNumber.ToString() + ".png";
            return path;
        }


        /// <summary>
        /// Create the level.
        /// </summary>
        public void CreateLevel()
        {
            FileStream fs = new FileStream(JsonPath(TotalLevel + 1), FileMode.Create);
            fs.Close();
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }



        /// <summary>
        /// Save the given level.
        /// </summary>
        /// <param name="levelNumber"></param>
        public void SaveLevel(int levelNumber)
        {
            File.WriteAllText(JsonPath(levelNumber), GetLevelData(levelNumber));
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }



        /// <summary>
        /// Load the given level.
        /// </summary>
        /// <param name="levelNumber"></param>
        public void LoadLevel(int levelNumber)
        {
            ClearScene();

            TextAsset tex = Resources.Load("Levels/" + levelNumber.ToString()) as TextAsset;
            LevelData levelData = JsonUtility.FromJson<LevelData>(tex.ToString().Trim());

            //Setup params
            targetObjectAmount = levelData.TargetObjectAmount;
            timeToCompleteLevel = levelData.TimeToCompleteLevel;
            playerMovementSpeed = levelData.PlayerMovementSpeed;
            for (int i = 0; i < groundTextures.Length; i++)
            {
                if (groundTextures[i].name.Equals(levelData.GroundTexture))
                {
                    groundMaterial.SetTexture("_Main_Texture", groundTextures[i]);
                    break;
                }
            }

            //Load target objects
            foreach(TargetObjectData objectData in levelData.ListTargetObjectData)
            {
                TargetObjectController prefab = targetObjectPrefabs.Where(a => a.ObjectName.Equals(objectData.ObjectName)).FirstOrDefault();
                TargetObjectController targetObject = Instantiate(prefab, objectData.Position, Quaternion.Euler(objectData.Angles));
                targetObject.transform.localScale = objectData.Scale;
                targetObject.gameObject.name = prefab.name;
            }

            //Load deadly objects
            foreach (DeadlyObjectData objectData in levelData.ListDeadlyObjectData)
            {
                DeadlyObjectController prefab = deadlyObjectPrefabs.Where(a => a.ObjectName.Equals(objectData.ObjectName)).FirstOrDefault();
                DeadlyObjectController deadlyObject = Instantiate(prefab, objectData.Position, Quaternion.Euler(objectData.Angles));
                deadlyObject.transform.localScale = objectData.Scale;
                deadlyObject.gameObject.name = prefab.name;
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }


        /// <summary>
        /// Clear the whole scene.
        /// </summary>
        public void ClearScene()
        {
            TargetObjectController[] targetObjects = FindObjectsByType<TargetObjectController>(FindObjectsSortMode.None);
            foreach(TargetObjectController targetObject in targetObjects)
            {
                DestroyImmediate(targetObject.gameObject);
            }

            DeadlyObjectController[] deadlyObjects = FindObjectsByType<DeadlyObjectController>(FindObjectsSortMode.None);
            foreach(DeadlyObjectController deadlyObject in deadlyObjects)
            {
                DestroyImmediate(deadlyObject.gameObject);
            }
        }


        /// <summary>
        /// Determine whether the current level data is null.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        public bool IsLevelNullData(int levelNumber)
        {
            if (TotalLevel == 0 || levelNumber <= 0 || levelNumber > TotalLevel) return true;
            string data = File.ReadAllText(JsonPath(levelNumber)).Trim();
            return string.IsNullOrEmpty(data);
        }




        /// <summary>
        /// Get the LevelData object based on current level.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        private string GetLevelData(int levelNumber)
        {
            LevelData levelData = new LevelData();
            levelData.LevelNumber = levelNumber;
            levelData.TargetObjectAmount = targetObjectAmount;
            levelData.TimeToCompleteLevel = timeToCompleteLevel;
            levelData.PlayerMovementSpeed = playerMovementSpeed;
            levelData.GroundTexture = groundMaterial.GetTexture("_Main_Texture").name;

            levelData.ListTargetObjectData = new List<TargetObjectData>();
            TargetObjectController[] targetObjects = FindObjectsByType<TargetObjectController>(FindObjectsSortMode.None);
            foreach (TargetObjectController targetObject in targetObjects)
            {
                TargetObjectData objectData = new TargetObjectData();
                objectData.ObjectName = targetObject.ObjectName;
                objectData.Position = targetObject.transform.position;
                objectData.Angles = targetObject.transform.eulerAngles;
                objectData.Scale = targetObject.transform.localScale;
                levelData.ListTargetObjectData.Add(objectData);
            }

            levelData.ListDeadlyObjectData = new List<DeadlyObjectData>();
            DeadlyObjectController[] deadlyObjects = FindObjectsByType<DeadlyObjectController>(FindObjectsSortMode.None);
            foreach (DeadlyObjectController deadlyObject in deadlyObjects)
            {
                DeadlyObjectData objectData = new DeadlyObjectData();
                objectData.ObjectName = deadlyObject.ObjectName;
                objectData.Position = deadlyObject.transform.position;
                objectData.Angles = deadlyObject.transform.eulerAngles;
                objectData.Scale = deadlyObject.transform.localScale;
                levelData.ListDeadlyObjectData.Add(objectData);
            }

            return JsonUtility.ToJson(levelData);
        }
    }
}
