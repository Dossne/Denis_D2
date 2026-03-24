using UnityEngine;

namespace ClawbearGames
{
    public static class HoleProgressionRules
    {
        public const int MinItemTier = 1;
        public const int MaxItemTier = 9;
        public const int MinHoleLevel = 1;
        public const int MaxHoleLevel = 13;
        public const float HoleGrowthFactor = 1.25f;
        public const float SizeEpsilon = 0.0001f;

        private static readonly int[] ItemPointsByTier =
        {
            1, 4, 8, 10, 15, 15, 15, 20, 20
        };

        private static readonly int[] ScoreThresholdByHoleLevel =
        {
            0, 10, 20, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000
        };

        public static int GetPointsForItemTier(int itemTier)
        {
            int tierIndex = Mathf.Clamp(itemTier, MinItemTier, MaxItemTier) - 1;
            return ItemPointsByTier[tierIndex];
        }

        public static int GetScoreThresholdForHoleLevel(int holeLevel)
        {
            int levelIndex = Mathf.Clamp(holeLevel, MinHoleLevel, MaxHoleLevel) - 1;
            return ScoreThresholdByHoleLevel[levelIndex];
        }

        public static int GetHoleLevelByScore(int score)
        {
            int clampedScore = Mathf.Max(0, score);
            int resolvedLevel = MinHoleLevel;

            for (int level = MinHoleLevel; level <= MaxHoleLevel; level++)
            {
                if (clampedScore >= GetScoreThresholdForHoleLevel(level))
                {
                    resolvedLevel = level;
                }
                else
                {
                    break;
                }
            }

            return resolvedLevel;
        }

        public static float GetHoleDiameter(float baseHoleDiameter, int holeLevel)
        {
            float safeBaseDiameter = Mathf.Max(baseHoleDiameter, SizeEpsilon);
            int clampedLevel = Mathf.Clamp(holeLevel, MinHoleLevel, MaxHoleLevel);
            return safeBaseDiameter * Mathf.Pow(HoleGrowthFactor, clampedLevel - MinHoleLevel);
        }

        public static int GetItemTierBySize(float objectSize, float baseHoleDiameter)
        {
            float safeObjectSize = Mathf.Max(0f, objectSize);

            for (int tier = MinItemTier; tier <= MaxItemTier; tier++)
            {
                float absorbDiameter = GetHoleDiameter(baseHoleDiameter, tier);
                if (safeObjectSize <= absorbDiameter + SizeEpsilon)
                {
                    return tier;
                }
            }

            return MaxItemTier;
        }
    }
}
