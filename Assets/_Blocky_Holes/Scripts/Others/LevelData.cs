using System.Collections.Generic;

namespace ClawbearGames
{
    [System.Serializable]
    public class LevelData
    {
        public int LevelNumber = 1;
        public int TargetObjectAmount = 1;
        public int TimeToCompleteLevel = 30;
        public float PlayerMovementSpeed = 5f;
        public string GroundTexture = string.Empty;
        public List<TargetObjectData> ListTargetObjectData = new List<TargetObjectData>();
        public List<DeadlyObjectData> ListDeadlyObjectData = new List<DeadlyObjectData>();
    }
}
