using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    [System.Serializable]
    public class TargetObjectData
    {
        public string ObjectName = string.Empty;
        public Vector3 Position = Vector3.zero;
        public Vector3 Angles = Vector3.zero;
        public Vector3 Scale = Vector3.one;
    }
}
