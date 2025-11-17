using TMPro;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class AddHealth : MonoBehaviour
    {
        [SerializeField] private int dopHealth;
        [SerializeField] private int cost;

        [SerializeField] private TextMeshProUGUI costText;

        private PLayerHealth pLayerHealth;
        private ScoreManager scoreManager;

        public void Initialize(PLayerHealth pLayerHealth, ScoreManager scoreManager)
        {
            this.pLayerHealth = pLayerHealth;
            this.scoreManager = scoreManager;

            costText.text = cost.ToString();
        }

        public void AddPlayerHealth()
        {
            if(scoreManager.GetCurrentMoney() >= cost)
            {
                scoreManager.RemoveMoney(cost);
                pLayerHealth.UpdateHealth(dopHealth);
            }
        }
    }
}