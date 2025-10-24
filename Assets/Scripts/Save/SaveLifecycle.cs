using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Save
{
    public class SaveLifecycle : MonoBehaviour
    {
        public static SaveData Data { get; private set; }  // доступ к данным сейва из других скриптов

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
    }
}