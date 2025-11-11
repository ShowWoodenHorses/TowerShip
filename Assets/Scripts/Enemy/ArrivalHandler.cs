using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof(BoxCollider))]
    public class ArrivalHandler : MonoBehaviour
    {
        [Header("Game Manager")]
        [SerializeField] private GameManager gameManager;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<EnemyTransportAI>() != null)
            {
                gameManager.EnemyLost();
            }
        }
    }
}