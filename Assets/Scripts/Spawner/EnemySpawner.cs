using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Animation;
using Assets.Scripts.Configs;
using Assets.Scripts.Spawner;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Виды спавнеров")]
    public List<SpawnerBase> spawners;

    [Header("Настройки спавна")]
    public List<EnemyType> enemyTypes;
    public Transform playerTransform;
    public int maxTotalEnemies = 10;
    public float checkInterval = 5f;
    public float timeForUpdate = 10f;

    [SerializeField] private int currentTotalEnemies = 0;
    private float timer;

    [Header("Score Manager")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private EnemyWaveDatabase enemyWaveDatabase;
    [SerializeField] private string currentEnemyWaveId;

    [Header("Animation")]
    private GameplayAnimationController gameplayAnimation;

    [Header("Для Дебага")]
    public List<EnemyType> availableEnemies;

    private void OnEnable()
    {
        scoreManager.OnUpdateWave += OnUpdateSettingSpawn;
    }

    private void OnDisable()
    {
        scoreManager.OnUpdateWave -= OnUpdateSettingSpawn;
    }

    //Bootstrap
    //private void Start()
    //{
    //    if (playerTransform == null)
    //    {
    //        var playerObj = GameObject.FindGameObjectWithTag("Player");
    //        if (playerObj != null) playerTransform = playerObj.transform;
    //    }

    //    if (playerTransform == null)
    //    {
    //        Debug.LogError("EnemySpawner: Игрок не найден!");
    //        enabled = false;
    //        return;
    //    }

    //    timer = checkInterval;
    //}

    public void Initialize(string enemyWaveId, Transform playerTransform, GameplayAnimationController gameplayAnimation)
    {
        this.playerTransform = playerTransform;
        this.gameplayAnimation = gameplayAnimation;

        timer = checkInterval;
        OnUpdateSettingSpawn(enemyWaveId);
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            CheckAndSpawn();
            timer = checkInterval;
        }
    }

    protected virtual void CheckAndSpawn()
    {
        if (currentTotalEnemies >= maxTotalEnemies) return;

        availableEnemies = new List<EnemyType>();

        foreach (EnemyType enemy in enemyTypes)
        {
            if (enemy.currentCount < enemy.maxCount)
            {
                availableEnemies.Add(enemy);
            }
        }

        if (availableEnemies.Count == 0) return;

        EnemyType enemyToSpawn = availableEnemies[Random.Range(0, availableEnemies.Count)];

        Debug.Log("Выпал префаб: " +  enemyToSpawn);

        foreach(var spawner in spawners)
        {
            if (spawner.HaveThisPrefab(enemyToSpawn.prefab))
            {
                GameObject enemyInstance = spawner.Spawn(playerTransform, enemyToSpawn.prefab, gameplayAnimation);
                if (enemyInstance != null)
                {
                    enemyToSpawn.currentCount++;
                    currentTotalEnemies++;

                    // Добавляем обработчик события, чтобы отслеживать уничтожение врага
                    var enemyController = enemyInstance.GetComponent<EnemyController>();
                    if (enemyController != null)
                    {
                        enemyController.Initialize(enemyToSpawn.prefab, gameplayAnimation);
                        enemyController.OnEnemyDeath += OnEnemyDeath;
                    }
                }
            }
        }
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        // Уменьшаем счетчики
        currentTotalEnemies--;

        var enemyController = enemy.GetComponent<EnemyController>();

        if (enemyController.IsReward())
        {
            scoreManager.AddMoney(enemyController.GetReward());
        }

        foreach (EnemyType enemyType in enemyTypes)
        {
            if (enemyType.prefab == enemyController.prefabRef)
            {
                enemyType.currentCount--;
                break;
            }
        }
        enemyController.OnEnemyDeath -= OnEnemyDeath;

        // Возвращаем объект в пул
        EnemyObjectPool.Instance.ReturnObject(enemy);
    }

    private void OnUpdateSettingSpawn(string enemyWaveId)
    {
        EnemyWaveConfig config = enemyWaveDatabase.GetEnemyConfigById(enemyWaveId);

        if (config == null) return;

        if (currentEnemyWaveId == config.id) return;

        checkInterval = timeForUpdate;
        currentEnemyWaveId = config.id;
        UpdateEnemyTypes(config.enemyTypes);
        UpdateMaxCountEnemy(config.maxLimitEnemy);
        UpdateCheckInterval(config.intervalSpawn);

        Debug.Log($"Новый конфиг: {config.id}");
    }

    private void UpdateEnemyTypes(EnemyType[] newEnemyTypes)
    {
        enemyTypes.Clear();
        foreach(var newEnemy in newEnemyTypes)
        {
            enemyTypes.Add(newEnemy);
            newEnemy.currentCount = 0;
        }
    }

    private void UpdateMaxCountEnemy(int newMaxLimitEnemy)
    {
        maxTotalEnemies = newMaxLimitEnemy;
    }

    private void UpdateCheckInterval(float newInterval)
    {
        checkInterval = newInterval;
    }
}