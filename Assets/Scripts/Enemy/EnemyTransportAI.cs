using UnityEngine;
using UnityEngine.AI;

public class EnemyTransportAI : MonoBehaviour
{
    public float detectionRadius = 25f;
    public float destanationDestroyBeforePointB = 2f;

    [SerializeField] private Transform player;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    private NavMeshAgent agent;
    private EnemyWeaponSystem weaponSystem;
    private Transform target;

    public void Initialize(
        Transform playerTransform, 
        Transform pointA,
        Transform pointB)
    {
        player = playerTransform;
        this.pointA = pointA;
        this.pointB = pointB;
        agent = GetComponent<NavMeshAgent>();
        weaponSystem = GetComponent<EnemyWeaponSystem>();
    }

    private void Update()
    {
        if (Vector3.Distance(pointB.position, transform.position) < destanationDestroyBeforePointB)
        {
            var enemyController = GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.Die(false);
            }
        }
    }


    public virtual void SetTarget(Transform newTarget)
    {
        target = newTarget;
        weaponSystem?.SetTarget(newTarget);
        agent.SetDestination(newTarget.position);
    }

    public virtual void SetStartPosition(Vector3 startPosition)
    {
        agent.Warp(startPosition);
        transform.rotation = Quaternion.identity;
    }
}
