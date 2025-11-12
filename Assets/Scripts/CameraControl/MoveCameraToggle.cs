using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CameraControl
{
    public class MoveCameraToggle : MonoBehaviour
    {
        private CameraController cameraController;
        private PlayerManager playerManager;

        [Header("Images")]
        private Image baseImage;
        [SerializeField] private Sprite startImage;
        [SerializeField] private Sprite moveImage;

        private bool canMove = false;

        public void Initialize(CameraController cameraController, PlayerManager playerManager)
        {
            this.cameraController = cameraController;
            this.playerManager = playerManager;
            baseImage = GetComponent<Image>();
            baseImage.sprite = startImage;
        }

        public void ToggleMoveCamera()
        {
            canMove = !canMove;
            if (canMove)
            {
                cameraController.SetActiveMoving();
                playerManager.SetDisableShoot();
                baseImage.sprite = moveImage;
            }
            else
            {
                cameraController.SetDisableMoving();
                playerManager.SetCanShoot();
                baseImage.sprite = startImage;
            }
        }

        public void SwitchToggleOnDisableMove()
        {
            if (!canMove)
                return;

            canMove = false;
            cameraController.SetDisableMoving();
            playerManager.SetCanShoot();
            baseImage.sprite = startImage;
        }

        public void SwitchToggleOnActiveMove()
        {
            if (canMove)
                return;

            canMove = true;
            cameraController.SetActiveMoving();
            playerManager.SetDisableShoot();
            baseImage.sprite = moveImage;
        }
    }
}