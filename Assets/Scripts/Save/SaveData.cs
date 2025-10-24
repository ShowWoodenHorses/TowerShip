using System;                                
using System.Collections.Generic;

namespace Assets.Scripts.Save
{
    [Serializable]
    public class SaveData
    {
        public int version = 1;
        public int currentCoins = 0;
        public int allCoins = 0;

        public string selectedPLayerId;
        public string selectedBulletId;
        public string currentWaveEnemyId;

        public List<string> ownedItems = new();
    }
}
