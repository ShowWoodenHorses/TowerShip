using UnityEngine;
using System;
using Assets.Scripts.Interface;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Assets.Scripts.Animation;

public class EnemyController : MonoBehaviour, IDamagable, IReward
{
    // Ссылка на префаб, от которого был создан враг
    [HideInInspector]
    public GameObject prefabRef;

    // Событие смерти врага
    public event Action<GameObject> OnEnemyDeath;

    private bool isDead = false;
    private bool checkDiedEnemy = false;

    // Пример здоровья (можно заменить своей системой)
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int reward;
    [SerializeField] private bool isReward = true;

    [Header("Canvas")]
    [SerializeField] private GameObject HealthObject;
    [SerializeField] private float timeShowHealth = 2f;
    [SerializeField] private Slider healthSlider;

    [Header("Animation")]
    [SerializeField] private GameObject modelForAnim;
    [SerializeField] private float wobbleAmount = 5f;
    [SerializeField] private float wobbleDuration = 2f;
    private GameplayAnimationController gameplayAnimation;
    private Sequence sequence;
    private Tween enemySwayTween;

    private void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
        InitializeSliderHealth();
    }

    public void Initialize(GameObject prefabRef, GameplayAnimationController gameplayAnimation)
    {
        this.prefabRef = prefabRef;
        this.gameplayAnimation = gameplayAnimation;

        if (gameplayAnimation != null)
        {
            enemySwayTween = gameplayAnimation.EnemySway(modelForAnim.transform, wobbleAmount, wobbleDuration);
        }

        currentHealth = maxHealth;
        isDead = false;
        checkDiedEnemy = false;
        InitializeSliderHealth();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(ShowHealth());
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die(true);
        }
    }

    public void Die(bool haveReward)
    {
        if (isDead) return;

        isDead = true;
        isReward = haveReward;

        // Важно: сам объект не уничтожаем, он вернётся в пул из EnemySpawner
        StartCoroutine(DestroyEnemy());
    }

    public bool IsDiedEnemy() => isDead;
    public bool CheckDiedEnemy() => checkDiedEnemy;

    public void SetCheckDiedEnemy()
    {
        checkDiedEnemy = true;
    }

    public int GetReward()
    {
        return reward;
    }

    public bool IsReward()
    {
        return isReward;
    }

    private IEnumerator ShowHealth()
    {
        HealthObject.SetActive(true);
        healthSlider.value = currentHealth;
        yield return new WaitForSeconds(timeShowHealth);
        HealthObject.SetActive(false);
    }

    private void InitializeSliderHealth()
    {
        HealthObject.SetActive(false);
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    private IEnumerator DestroyEnemy()
    {
        HealthObject.SetActive(false);
        sequence = gameplayAnimation.DestroyShip(transform);
        yield return new WaitForSeconds(2f);
        
        OnEnemyDeath?.Invoke(gameObject);
        gameObject.SetActive(false);
        sequence?.Kill();
        enemySwayTween?.Kill();

    }
}
