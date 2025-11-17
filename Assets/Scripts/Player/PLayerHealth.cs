using System;
using Assets.Scripts.Interface;
using Assets.Scripts.Save;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PLayerHealth : MonoBehaviour, IDamagable
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int currentHealth;
        [SerializeField] private TextMeshProUGUI textPlayerHealth;

        public static event Action<GameObject> OnPlayerDie;

        private SaveLifecycle saveLifecycle;

        public void Initialize(int maxHealth, SaveLifecycle saveLifecycle)
        {
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            textPlayerHealth.text = currentHealth.ToString();
            this.saveLifecycle = saveLifecycle;
        }
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            textPlayerHealth.text = currentHealth.ToString();

            saveLifecycle.UpdateHealth(currentHealth);

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

        public void UpdateHealth(int health)
        {
            currentHealth += health;
            textPlayerHealth.text = currentHealth.ToString();
            saveLifecycle.UpdateHealth(currentHealth);

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