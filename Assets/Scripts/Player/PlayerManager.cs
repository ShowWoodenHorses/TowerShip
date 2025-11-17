using Assets.Scripts.Configs;
using Assets.Scripts.Save;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Игрок")]
        [SerializeField] private PlayerDatabase playerDatabase;
        [SerializeField] private ShipAimLine shipAimLine;
        [SerializeField] private CircleNoFire circleNoFire;
        [SerializeField] private GunController gunController;

        [Header("Место спавна")]
        [SerializeField] private Transform spawnPLayerPosition;
        [SerializeField] private Transform parentPosition;

        [Header("UI")]
        [SerializeField] private ReloadPlayerUI reloadPlayerUI;

        private GameObject currentPlayerInstance;
        private SaveLifecycle saveLifecycle;

        public void Initialize(string playerId, SaveLifecycle saveLifecycle)
        {
            this.saveLifecycle = saveLifecycle;
            UpgradePlayer(playerId);
        }

        public void UpgradePlayer(string playerId)
        {
            PlayerConfig config = playerDatabase.GetPlayerById(playerId);
            if (config == null)
            {
                Debug.LogError($"Player with id '{playerId}' not found in database!");
                return;
            }

            SpawnPlayer(config);
        }



        private void SpawnPlayer(PlayerConfig config)
        {
            if (currentPlayerInstance != null)
            {
                Destroy(currentPlayerInstance);
            }

            currentPlayerInstance = Instantiate(config.playerPrefab, spawnPLayerPosition.position, transform.rotation, parentPosition);
            GunController gunController = currentPlayerInstance.GetComponent<GunController>();
            if (gunController != null)
            {
                this.gunController = gunController;
                reloadPlayerUI.Initialize(gunController);
                gunController.Initialize(shipAimLine, circleNoFire, saveLifecycle, config.id);
            }
        }

        public void SetCanShoot()
        {
            if(gunController != null)
                gunController.CanShoot(true);
        }

        public void SetDisableShoot()
        {
            if (gunController != null)
                gunController.CanShoot(false);
        }

    }
}