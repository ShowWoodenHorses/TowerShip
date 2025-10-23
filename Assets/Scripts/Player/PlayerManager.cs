using System.Collections;
using Assets.Scripts.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Корабль")]
        [SerializeField] private ShipDatabase shipDatabase;
        [SerializeField] private GameObject currentBulletPrefab;
        [SerializeField] private Slider healthBarSlider;

        private PLayerHealth health;

        public void Initialize()
        {
            SetHealth();
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

    }
}