using Assets.Scripts.Animation;
using Assets.Scripts.Control;
using Assets.Scripts.Game;
using Assets.Scripts.Interface;
using Assets.Scripts.ObjectPool;
using Assets.Scripts.Player;
using Assets.Scripts.Save;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Shop;
using UnityEngine;
using UnityEngine.Audio;
using YG;

namespace Assets.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        private SaveData data;

        [Header("Platform")]
        [SerializeField] private CheckPLatform platform;

        [Header("Shop")]
        //[SerializeField] private ShopBulletController shopBulletController;
        //[SerializeField] private ShopShipController shopShipController;
        [SerializeField] private ScoreManager scoreManager;

        [Header("Pool")]
        [SerializeField] private EnemyObjectPool enemyPool;
        [SerializeField] private BulletObjectPool bulletPool;
        [SerializeField] private EffectObjectPool effectPool;
        [SerializeField] private SoundPoolManager soundPoolManager;

        [Header("Player")]
        //[SerializeField] private ShipManager shipManager;
        //[SerializeField] private ShipMovement shipMovement;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private Transform playerTransform;

        [Header("Spawner")]
        [SerializeField] private EnemySpawner enemySpawner;

        [Header("Generation")]
        //[SerializeField] private MapGeneration mapGeneration;

        [Header("Save")]
        [SerializeField] private SaveLifecycle saveLifecycle;

        [Header("Animation")]
        [SerializeField] private GameplayAnimationController gameplayAnimationController;

        [Header("Game")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private UIController uiController;
        //[SerializeField] private UIDisplayCannon uiDisplayCannon;

        [Header("Sound")]
        [SerializeField] private AudioMixer mainMixer;

        //[Header("Ads")]
        //[SerializeField] private RewardsAds rewardsAds;
        private void Awake()
        {
            data = SaveSystem.Load();
            //IShipInput shipInput = platform.CheckCurrentPlatform();

            saveLifecycle.Initialize(data);

            enemyPool.Initialize();
            bulletPool.Initialize();
            effectPool.Initialize();

            //shopBulletController.Initialize(data.ownedItems, data.selectedBulletId, saveLifecycle);
            //shopShipController.Initialize(data.ownedItems, data.selectedShipId, saveLifecycle);
            scoreManager.Initialize(data.currentCoins, data.allCoins, saveLifecycle);

            soundPoolManager.Initialize(mainMixer);

            //rewardsAds.Initialize(scoreManager);

            //shipManager.Initialize(data.selectedShipId, data.selectedBulletId, gameplayAnimationController, uiDisplayCannon, shipInput);
            playerManager.Initialize();
            gameManager.Initialize(uiController, scoreManager);
            enemySpawner.Initialize(data.currentWaveEnemyId, playerTransform, gameplayAnimationController);

            YG2.GameplayStart();
        }
    }
}