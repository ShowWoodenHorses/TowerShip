using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ReloadPlayerUI : MonoBehaviour
    {
        [SerializeField] Image imageReload;

        private GunController gunController;

        private float current;
        private float max;

        public void Initialize(GunController gunController)
        {
            this.gunController = gunController;
            imageReload.fillAmount = 1f;

            max = gunController.GetStartReloadTime();
            if(max == 0f)
                max = 1f;
        }

        private void Update()
        {
            if (gunController == null) return;

            current = gunController.GetCurrentReloadTime();

            imageReload.fillAmount = (current / max);
        }
    }
}