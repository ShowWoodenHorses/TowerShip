using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Achievements.Configs;
using Assets.Scripts.Save;
using TMPro;
using UnityEngine;
using YG;

namespace Assets.Scripts.Achievements
{
    public class AchievementManager : MonoBehaviour
    {
        [SerializeField] private AchievementData[] achievementsData;
        [SerializeField] private GameObject acievementPrefab;
        [SerializeField] private Transform parentPosition;

        [SerializeField] private TextMeshProUGUI generalCountAchievText;

        [Header("Special Achievements")]
        [SerializeField] private AchievementData achievForAllDamage;
        [SerializeField] private AchievementData achievForAllKill;
        [SerializeField] private AchievementData achievForAllMoney;

        private List<AchievementItem> allAchievementsForDamage = new();
        private List<AchievementItem> allAchievementsForKill = new();

        private const string ACHIEVEMENT_ID_DAMAGE = "damage";
        private const string ACHIEVEMENT_ID_KILL = "kill";

        private int countAllAchievements;
        private int countReceviedAchievements;

        private int allDamage;
        private int allKill;

        public void Initialize()
        {
            CreateAchevementDialog();
            InitializeFromSave();
            CreateSpecialAchievements();
            UpdateGeneralCountAchievements();

            YG2.MetricaSend("openAchievements");
        }

        private void CreateAchevementDialog()
        {
            foreach (var item in achievementsData)
            {
                GameObject achievObj = Instantiate(acievementPrefab, parentPosition);
                AchievementItem achievementItem = achievObj.GetComponent<AchievementItem>();
                if (achievementItem != null)
                {
                    achievementItem.Initialize(item);

                    if (achievementItem.GetAchievementId() == ACHIEVEMENT_ID_DAMAGE)
                    {
                        allAchievementsForDamage.Add(achievementItem);
                    }
                    else if (achievementItem.GetAchievementId() == ACHIEVEMENT_ID_KILL)
                    {
                        allAchievementsForKill.Add(achievementItem);
                    }

                    countAllAchievements++;
                }
            }
        }

        private void InitializeFromSave()
        {
            var gunsById = SaveLifecycle.Data.ownedGunStatDict.ToDictionary(g => g.gunId);

            foreach (var item in allAchievementsForDamage)
            {
                if (gunsById.TryGetValue(item.GetGunId(), out var gunStatDict))
                {
                    item.SetCurrentPoints(gunStatDict.damage);
                    allDamage += gunStatDict.damage;

                    if (gunStatDict.damage >= item.GetCountFinishPoints())
                    {
                        item.SetGetAchievement();
                        countReceviedAchievements++;
                    }
                }
                else
                {
                    item.SetCurrentPoints(0);
                }
            }


            foreach (var item in allAchievementsForKill)
            {
                if (gunsById.TryGetValue(item.GetGunId(), out var gunStatDict))
                {
                    item.SetCurrentPoints(gunStatDict.countKill);
                    allKill += gunStatDict.countKill;

                    if (gunStatDict.countKill >= item.GetCountFinishPoints())
                    {
                        item.SetGetAchievement();
                        countReceviedAchievements++;
                    }
                }
                else
                {
                    item.SetCurrentPoints(0);
                }
            }
        }

        private void CreateSpecialAchievements()
        {
            CreateSpecialAchievForDamage();
            CreateSpecialAchievForKill();
            CreateSpecialAchievForMoney();
        }

        private void CreateSpecialAchievForDamage()
        {
            if (achievForAllDamage == null) return;

            GameObject achievObj = Instantiate(acievementPrefab, parentPosition);
            AchievementItem achievementItem = achievObj.GetComponent<AchievementItem>();
            if (achievementItem != null)
            {
                achievementItem.Initialize(achievForAllDamage);
                achievementItem.SetCurrentPoints(allDamage);
                if(allDamage >= achievementItem.GetCountFinishPoints())
                {
                    achievementItem.SetGetAchievement();
                    countReceviedAchievements++;
                }
                countAllAchievements++;
            }
        }

        private void CreateSpecialAchievForKill()
        {
            if (achievForAllKill == null) return;

            GameObject achievObj = Instantiate(acievementPrefab, parentPosition);
            AchievementItem achievementItem = achievObj.GetComponent<AchievementItem>();
            if (achievementItem != null)
            {
                achievementItem.Initialize(achievForAllKill);
                achievementItem.SetCurrentPoints(allKill);
                if (allKill >= achievementItem.GetCountFinishPoints())
                {
                    achievementItem.SetGetAchievement();
                    countReceviedAchievements++;
                }
                countAllAchievements++;
            }
        }

        private void CreateSpecialAchievForMoney()
        {
            if (achievForAllMoney == null) return;

            int allMoney = SaveLifecycle.Data.allCoins;

            GameObject achievObj = Instantiate(acievementPrefab, parentPosition);
            AchievementItem achievementItem = achievObj.GetComponent<AchievementItem>();
            if (achievementItem != null)
            {
                achievementItem.Initialize(achievForAllMoney);
                achievementItem.SetCurrentPoints(allMoney);
                if (allMoney >= achievementItem.GetCountFinishPoints())
                {
                    achievementItem.SetGetAchievement();
                    countReceviedAchievements++;
                }
                countAllAchievements++;
            }
        }

        private void UpdateGeneralCountAchievements()
        {
            generalCountAchievText.text = $"{countReceviedAchievements}/{countAllAchievements}";
        }
    }
}