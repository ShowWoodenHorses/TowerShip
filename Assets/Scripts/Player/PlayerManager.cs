using System.Collections;
using Assets.Scripts.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Игрок")]
        [SerializeField] private PlayerDatabase playerDatabase;
        [SerializeField] private Slider healthBarSlider;
        [SerializeField] private ShipAimLine shipAimLine;

        [Header("Место спавна")]
        [SerializeField] private Transform spawnPLayerPosition;
        [SerializeField] private Transform parentPosition;

        private GameObject currentPlayerInstance;
        private PLayerHealth health;

        public void Initialize(string playerId)
        {
            SetHealth();
            UpgradePlayer(playerId);
        }

        private void SetHealth()
        {
            health = GetComponent<PLayerHealth>();

            if (health != null)
            {
                int maxHealth = health.GetMaxHealth();
                health.Initialize(maxHealth, healthBarSlider);
                healthBarSlider.maxValue = maxHealth;
                healthBarSlider.value = maxHealth;
            }
        }

        public void UpgradePlayer(string playerId)
        {
            PlayerConfig config = playerDatabase.GetPlayerById(playerId);
            if (config == null)
            {
                Debug.LogError($"Player with id '{playerId}' not found in database!");
                return;
            }

            SpawnPlayer(config);
        }



        private void SpawnPlayer(PlayerConfig config)
        {
            if (currentPlayerInstance != null)
            {
                Destroy(currentPlayerInstance);
            }

            currentPlayerInstance = Instantiate(config.playerPrefab, spawnPLayerPosition.position, transform.rotation, parentPosition);
            GunController gunController = currentPlayerInstance.GetComponent<GunController>();
            if(gunController != null)
            {
                gunController.Initialize(shipAimLine);
            }
        }

    }
}