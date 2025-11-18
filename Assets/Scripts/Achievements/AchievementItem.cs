using Assets.Scripts.Achievements.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Scripts.Achievements
{
    public class AchievementItem : MonoBehaviour
    {
        [SerializeField] private Image iconAchievement;
        [SerializeField] private GameObject checkGetAcievement;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI desciptionText;
        [SerializeField] private TextMeshProUGUI pointsText;

        private string gunId;
        private int finishCountPoints;

        private string achievementId;
        private const string ACHIEVEMENT_ID_DAMAGE = "damage";
        private const string ACHIEVEMENT_ID_KILL = "kill";

        public void Initialize(AchievementData data)
        {
            checkGetAcievement.SetActive(false);
            gunId = data.gunId;
            iconAchievement.sprite = data.icon;
            finishCountPoints = data.countPointsForFinish;
            pointsText.text = "0/" + data.countPointsForFinish.ToString();

            if (ACHIEVEMENT_ID_DAMAGE == data.achievementId)
            {
                achievementId = ACHIEVEMENT_ID_DAMAGE;
            }
            else
            {
                achievementId = ACHIEVEMENT_ID_KILL;
            }

            switch (YG2.lang)
            {
                case "ru":
                    nameText.text = data.name_RU;
                    desciptionText.text = data.description_RU;
                    break;
                case "tr":
                    nameText.text = data.name_TR;
                    desciptionText.text = data.description_TR;
                    break;
                default:
                    nameText.text = data.name_EN;
                    desciptionText.text = data.description_EN;
                    break;
            }
        }

        public string GetGunId()
        {
            return gunId;
        }

        public string GetAchievementId()
        {
            return achievementId;
        }

        public void SetCurrentPoints(int points)
        {
            pointsText.text = points.ToString()+ "/" + finishCountPoints.ToString();
        }

        public int GetCountFinishPoints()
        {
            return finishCountPoints;
        }

        public void SetGetAchievement()
        {
            checkGetAcievement.SetActive(true);
        }
    }
}