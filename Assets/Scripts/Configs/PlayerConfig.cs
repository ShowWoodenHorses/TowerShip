using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Player/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Идентификаторы")]
        public string id;
        public string displayName;

        [Header("Настройка игрока")]
        public GameObject playerPrefab;   // Префаб с пушками и моделью
    }
}