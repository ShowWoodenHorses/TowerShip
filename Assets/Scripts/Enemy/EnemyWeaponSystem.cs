using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponSystem : MonoBehaviour
{
    public enum CannonSide { None, Left, Right, Front, Rear }

    [Header("Боеприпасы")]
    public GameObject cannonballPrefab;
    public float projectileSpeed = 30f;
    public float timeBetweenShots = 0.3f; // Задержка между выстрелами пушек

    [Header("Группировка пушек")]
    public bool enableStaggeredFiring = true;

    private Transform _currentTarget;
    private readonly List<EnemyCannon> _allCannons = new List<EnemyCannon>();
    private readonly Dictionary<CannonSide, List<EnemyCannon>> _cannonsBySide = new Dictionary<CannonSide, List<EnemyCannon>>();
    private bool _isShootingInProgress = false;
    private Coroutine _shootingCoroutine;

    private void Awake()
    {
        _allCannons.Clear();
        _allCannons.AddRange(GetComponentsInChildren<EnemyCannon>(true));

        // Группируем пушки по сторонам
        InitializeCannonGroups();

        // Инициализируем пушки
        foreach (var cannon in _allCannons)
        {
            if (cannon != null)
            {
                cannon.OnReadyToFire += HandleCannonReadyToFire;
            }
        }
    }

    private void InitializeCannonGroups()
    {
        _cannonsBySide.Clear();
        foreach (var side in System.Enum.GetValues(typeof(CannonSide)))
        {
            _cannonsBySide[(CannonSide)side] = new List<EnemyCannon>();
        }

        foreach (var cannon in _allCannons)
        {
            if (cannon != null && _cannonsBySide.ContainsKey(cannon.side))
            {
                _cannonsBySide[cannon.side].Add(cannon);
            }
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        foreach (var cannon in _allCannons)
        {
            if (cannon != null)
            {
                cannon.OnReadyToFire -= HandleCannonReadyToFire;
            }
        }

        if (_shootingCoroutine != null)
        {
            StopCoroutine(_shootingCoroutine);
        }
    }

    public void SetTarget(Transform target)
    {
        _currentTarget = target;
        foreach (var cannon in _allCannons)
        {
            if (cannon != null)
            {
                cannon.Initialize(_currentTarget, cannonballPrefab);
            }
        }
    }

    private void HandleCannonReadyToFire(EnemyCannon cannon)
    {
        if (!_isShootingInProgress && enableStaggeredFiring)
        {
            // Запускаем последовательную стрельбу для стороны этой пушки
            _shootingCoroutine = StartCoroutine(ShootSideSequentially(cannon.side));
        }
    }

    private IEnumerator ShootSideSequentially(CannonSide side)
    {
        _isShootingInProgress = true;

        var sideCannons = _cannonsBySide[side];
        var readyCannons = new List<EnemyCannon>();

        // Собираем пушки, готовые к стрельбе
        foreach (var cannon in sideCannons)
        {
            if (cannon != null && cannon.CanFire())
            {
                readyCannons.Add(cannon);
            }
        }

        // Стреляем по очереди
        foreach (var cannon in readyCannons)
        {
            cannon.ForceFire();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        _isShootingInProgress = false;
    }

    // Альтернативный вариант - стрельба по таймеру
    //public void StartAutoFiring(float interval)
    //{
    //    StopAutoFiring();
    //    StartCoroutine(AutoFireRoutine(interval));
    //}

    //public void StopAutoFiring()
    //{
    //    StopAllCoroutines();
    //    _isShootingInProgress = false;
    //}

    private IEnumerator AutoFireRoutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            // Стреляем из всех сторон по очереди
            foreach (var side in _cannonsBySide.Keys)
            {
                if (_cannonsBySide[side].Count > 0 && !_isShootingInProgress)
                {
                    yield return ShootSideSequentially(side);
                    yield return new WaitForSeconds(timeBetweenShots * 2); // Пауза между сторонами
                }
            }
        }
    }
}