using System;
using System.Collections;
using Assets.Scripts.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class ShipHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int currentHealth;
        private Slider healthBarSlider;


        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void Initialize(int maxHealth, Slider slider)
        {
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            healthBarSlider = slider;
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }
    }
}