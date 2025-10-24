using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(fileName = "PlayerDatabase", menuName = "Player/PlayerDatabase")]
    public class PlayerDatabase : ScriptableObject
    {
        public PlayerConfig[] playerConfigs;

        public PlayerConfig GetPlayerById(string id)
        {
            return playerConfigs.FirstOrDefault(p => p.id == id);
        }

        public PlayerConfig GetPlayerByName(string name)
        {
            return playerConfigs.FirstOrDefault(p => p.name == name);
        }
    }
}