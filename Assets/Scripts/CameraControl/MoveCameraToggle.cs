using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.CameraControl
{
    public class MoveCameraToggle : MonoBehaviour
    {
        private CameraController cameraController;
        private PlayerManager playerManager;

        private bool canMove = false;

        public void Initialize(CameraController cameraController, PlayerManager playerManager)
        {
            this.cameraController = cameraController;
            this.playerManager = playerManager;
        }

        public void ToggleMoveCamera()
        {
            canMove = !canMove;
            if (canMove)
            {
                cameraController.SetActiveMoving();
                playerManager.SetDisableShoot();
            }
            else
            {
                cameraController.SetDisableMoving();
                playerManager.SetCanShoot();
            }
        }

        public void SwitchToggleOnDisableMove()
        {
            if (!canMove)
                return;

            canMove = false;
            cameraController.SetDisableMoving();
            playerManager.SetCanShoot();
        }

        public void SwitchToggleOnActiveMove()
        {
            if (canMove)
                return;

            canMove = true;
            cameraController.SetActiveMoving();
            playerManager.SetDisableShoot();
        }
    }
}