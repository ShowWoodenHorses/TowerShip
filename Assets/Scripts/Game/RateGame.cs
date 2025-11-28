using UnityEngine;
using YG;

namespace Assets.Scripts.Game
{
    public class RateGame : MonoBehaviour
    {
        [SerializeField] private int rewardCoins;

        private ScoreManager scoreManager;

        //private void OnEnable()
        //{
        //    YG2.onReviewSent += Reward;
        //}

        //private void OnDisable()
        //{
        //    YG2.onReviewSent -= Reward;
        //}
        public void Initialize(ScoreManager scoreManager)
        {
            this.scoreManager = scoreManager;
        }

        private void Reward(bool canReview)
        {
            if (canReview)
            {
                YG2.MetricaSend("rateUs");
                scoreManager.AddMoney(rewardCoins);
            }
        }

        public void GetReward()
        {
            YG2.MetricaSend("rateUs");
            scoreManager.AddMoney(rewardCoins);
        }
    }
}