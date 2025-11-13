using System;
using Assets.Scripts.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PLayerHealth : MonoBehaviour, IDamagable
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int currentHealth;
        [SerializeField] private TextMeshProUGUI textPlayerHealth;
        private Slider healthBarSlider;

        public static event Action<GameObject> OnPlayerDie;

        private void Start()
        {
            currentHealth = maxHealth;
            textPlayerHealth.text = currentHealth.ToString();
        }

        public void Initialize(int maxHealth, Slider slider)
        {
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            healthBarSlider = slider;
        }
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthBarSlider.value -= damage;
            textPlayerHealth.text = currentHealth.ToString();
            if (currentHealth < 0)
            {
                currentHealth = 0;
                OnPlayerDie?.Invoke(gameObject);
                Debug.Log("===== PLAYER DIE =========");
            }
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }

        public bool IsDiedEnemy()
        {
            throw new NotImplementedException();
        }

        public bool CheckDiedEnemy()
        {
            throw new NotImplementedException();
        }

        public void SetCheckDiedEnemy()
        {
            throw new NotImplementedException();
        }
    }
}