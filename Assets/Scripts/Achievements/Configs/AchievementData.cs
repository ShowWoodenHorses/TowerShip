using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Achievements.Configs
{
    [CreateAssetMenu(fileName = "AchievementData", menuName = "Achievement/AchievementData")]
    public class AchievementData : ScriptableObject
    {
        [Header("Name")]
        public string name_EN;
        public string name_RU;
        public string name_TR;

        [Header("Description")]
        public string description_EN;
        public string description_RU;
        public string description_TR;

        [Header("Main info")]
        public string achievementId;
        public string gunId;
        public Sprite icon;
        public int countPointsForFinish;

        [Header("Constants")]
        public string damage = "damage";
        public string kill = "kill";

    }
}