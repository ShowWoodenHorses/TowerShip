using System.Collections;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Save
{
    public class SaveLifecycle : MonoBehaviour
    {
        public static SaveData Data { get; private set; }  // доступ к данным сейва из других скриптов
        bool isSaving = false;

        //Bootstrap
        //void Awake()
        //{
        //    Data = SaveSystem.Load();
        //}

        public void Initialize(SaveData save)
        {
            Data = save;
        }

        void OnApplicationPause(bool pause)
        {
            if (pause) SaveSystem.Save(Data);              // мобилки: при сворачивании сохраняем
        }

        void OnApplicationFocus(bool focus)
        {
            if (!focus) SaveSystem.Save(Data);             // WebGL/десктоп: при потере фокуса сохраняем
        }

        
        public void BuyItem(string itemId)
        {                     
            if (!Data.ownedItems.Contains(itemId))
                Data.ownedItems.Add(itemId);

            SaveSystem.Save(Data);
        }

        public void SelectPlayer(string playerId)
        {
            if(Data.selectedPLayerId != playerId)
                Data.selectedPLayerId = playerId;

            SaveSystem.Save(Data);
        }

        public void SelectBullet(string bulletId)
        {
            if(Data.selectedBulletId != bulletId)
                Data.selectedBulletId = bulletId;

            SaveSystem.Save(Data);
        }

        public void AddMoney(int currentCoins, int allCoins)
        {
            Data.currentCoins = currentCoins;
            Data.allCoins = allCoins;

            SaveSystem.Save(Data);
        }

        public void RemoveMoney(int currentCoins)
        {
            Data.currentCoins = currentCoins;

            SaveSystem.Save(Data);
        }

        public void ChangeWave(string waveEnemyId)
        {
            if(Data.currentWaveEnemyId != waveEnemyId)
                Data.currentWaveEnemyId = waveEnemyId;

            SaveSystem.Save(Data);
        }

        public void UpdateTileTower(int indexTile, string towerName, int level = 1)
        {
            SaveTileData newTowerData = new() { tileId = indexTile, towerName = towerName, level = level};

            int foundIndex = Data.ownedTowersDict.FindIndex(t => t.tileId == indexTile);

            if(foundIndex >= 0)
            {
                Data.ownedTowersDict[foundIndex] = newTowerData;
            }
            else
            {
                Data.ownedTowersDict.Add(newTowerData);
            }

            SaveSystem.Save(Data);
        }

        public void DestroyTower(int indexTile)
        {
            int foundIndex = Data.ownedTowersDict.FindIndex(t => t.tileId == indexTile);

            if (foundIndex >= 0)
            {
                Data.ownedTowersDict.RemoveAt(foundIndex);
            }

            SaveSystem.Save(Data);
        }

        public void UpdateGunDamageStatistic(string gunId, int damage)
        {
            SaveGunStatData gunData = new() { gunId = gunId, damage = damage, countKill = 0 };

            int foundIndex = Data.ownedGunStatDict.FindIndex(g => g.gunId == gunId);

            if(foundIndex >= 0)
            {
                Data.ownedGunStatDict[foundIndex].damage += damage;
            }
            else
            {
                Data.ownedGunStatDict.Add(gunData);
            }

            SaveSystem.Save(Data);
        }

        public void UpdateGunKillStatistic(string gunId)
        {
            if (isSaving)
                return;

            isSaving = true;

            int foundIndex = Data.ownedGunStatDict.FindIndex(g => g.gunId == gunId);

            if (foundIndex >= 0)
            {
                Data.ownedGunStatDict[foundIndex].countKill++;
            }

            SaveSystem.Save(Data);

            isSaving = false;
        }
    }
}