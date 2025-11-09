using Assets.Scripts.Animation;
using Assets.Scripts.CameraControl;
using Assets.Scripts.Control;
using Assets.Scripts.Game;
using Assets.Scripts.Generation;
using Assets.Scripts.Interface;
using Assets.Scripts.ObjectPool;
using Assets.Scripts.Player;
using Assets.Scripts.Save;
using Assets.Scripts.Sound;
using Assets.Scripts.TowerDefence;
using Assets.Scripts.TowerDefence.UI;
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
        [SerializeField] private ShopPlayerController shopPlayerController;
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

        [Header("Camera")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private MoveCameraToggle moveCameraToggle;

        [Header("Sound")]
        [SerializeField] private AudioMixer mainMixer;

        //[Header("Ads")]
        //[SerializeField] private RewardsAds rewardsAds;

        [Header("Towers")]
        [SerializeField] private TowerBuildUI towerBuild; 
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
            shopPlayerController.Initialize(data.ownedItems, data.selectedPLayerId, saveLifecycle);
            scoreManager.Initialize(data.currentCoins, data.allCoins, saveLifecycle);

            soundPoolManager.Initialize(mainMixer);
            //rewardsAds.Initialize(scoreManager);

            //shipManager.Initialize(data.selectedShipId, data.selectedBulletId, gameplayAnimationController, uiDisplayCannon, shipInput);
            playerManager.Initialize(data.selectedPLayerId);
            gameManager.Initialize(uiController, scoreManager);
            enemySpawner.Initialize(data.currentWaveEnemyId, playerTransform, gameplayAnimationController);

            towerBuild.Initialize();
            moveCameraToggle.Initialize(cameraController, playerManager);

            YG2.GameplayStart();
        }

        private void Start()
        {
            TowerActionUI.Instance.Initialize(scoreManager);
            BuildManager.Instance.Initizlixe(scoreManager, saveLifecycle);
        }
    }
}