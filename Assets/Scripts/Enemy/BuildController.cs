using System;
using System.Collections;
using Assets.Scripts.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Enemy
{
    public class BuildController : MonoBehaviour, IDamagable, IReward
    {
        // Событие смерти врага
        public event Action<BuildController> OnBuildingDestroyed;

        private bool isDestroy = false;

        public int maxHealth = 100;
        [SerializeField] private int currentHealth;
        [SerializeField] private int reward;

        [Header("Состояние")]
        [SerializeField] private GameObject buildingActive;
        [SerializeField] private GameObject buildingDestroy;

        [Header("Canvas")]
        [SerializeField] private GameObject HealthObject;
        [SerializeField] private float timeShowHealth = 2f;
        [SerializeField] private Slider healthSlider;

        private void OnEnable()
        {
            // Когда объект берётся из пула — восстанавливаем здоровье
            currentHealth = maxHealth;
            isDestroy = false;
            InitializeSliderHealth();
        }

        public void Initialize()
        {
            currentHealth = maxHealth;
            isDestroy = false;
            buildingActive.SetActive(true);
            buildingDestroy.SetActive(false);
            InitializeSliderHealth();
        }

        public void TakeDamage(int damage)
        {
            if (isDestroy) return;

            currentHealth -= damage;
            StartCoroutine(ShowHealth());
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Destroyed();
            }
        }

        public void Destroyed()
        {
            if (isDestroy) return;

            isDestroy = true;

            // Вызываем событие смерти
            OnBuildingDestroyed?.Invoke(this);

            buildingActive.SetActive(false);
            buildingDestroy.SetActive(true);
        }

        public int GetReward()
        {
            return reward;
        }
        public bool IsDiedEnemy() => isDestroy;

        private IEnumerator ShowHealth()
        {
            HealthObject.SetActive(true);
            healthSlider.value = currentHealth;
            yield return new WaitForSeconds(timeShowHealth);
            HealthObject.SetActive(false);
        }

        private void InitializeSliderHealth()
        {
            HealthObject.SetActive(false);
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
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