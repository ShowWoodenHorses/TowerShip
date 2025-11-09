using System.Collections.Generic;
using Assets.Scripts.Save;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(fileName = "DefaultSaveConfig", menuName = "ScriptableObject/DefaultSaveConfig")]
    public class DefaultSaveConfig : ScriptableObject
    {
        [Header("Стартовые значения")]
        public int currentCoins = 10000;
        public int allCoins = 10000;
        public string selectedPLayerId = "player_cannon";
        public string selectedBulletId = "bullet_basic";
        public string currentWaveEnemyId = "wave_1";
        public List<string> ownedItems = new() { "player_cannon", "bullet_basic" };
        public List<SaveTileData> ownedTowersDict = new() { };
    }
}