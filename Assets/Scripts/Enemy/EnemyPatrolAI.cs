using Assets.Scripts.Enemy;
using Assets.Scripts.Spawner;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolAI : MonoBehaviour
{
    [SerializeField] private StandCannon[] cannons;

    private Transform[] patrolPoints;
    private NavMeshAgent agent;
    private Transform player;
    private SpawnFromShip spawner;

    private int currentPoint = 0;

    public void Initialize(Transform[] points, Transform playerTarget)
    {
        patrolPoints = points;
        player = playerTarget;

        transform.position = patrolPoints[Random.Range(0, patrolPoints.Length)].position;

        agent = GetComponent<NavMeshAgent>();

        if(cannons.Length > 0)
        {
            foreach (var cannon in cannons)
            {
                cannon.Initialize(playerTarget);
            }
        }

        if(spawner == null)
        {
            spawner = GetComponent<SpawnFromShip>();
        }
        spawner.Initialize(playerTarget);

        if (patrolPoints != null && patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || player == null)
            return;

        Patrol();
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPoint].position);
        }
    }
}
