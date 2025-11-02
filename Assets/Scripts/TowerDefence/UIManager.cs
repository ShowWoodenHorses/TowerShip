using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.TowerDefence
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public TextMeshProUGUI moneyText;

        private void Awake() => Instance = this;

        private void Start() => UpdateMoney();

        public void UpdateMoney()
        {
            moneyText.text = $"{BuildManager.Instance.playerMoney}$";
        }
    }
}