using System;
using System.Text;
using Assets.Scripts;
using Assets.Scripts.ObjectPool;
using UnityEngine;

[ExecuteAlways]
public class EnemyCannon : MonoBehaviour
{
    public enum DebugLevel { None = 0, Minimal = 1, Full = 2 }

    [Header("Общее")]
    public EnemyWeaponSystem.CannonSide side = EnemyWeaponSystem.CannonSide.None;

    [Header("Трансформы")]
    [Tooltip("Трансформ, который должен вращаться (pivot). Если пусто - используется сам объект.")]
    public Transform pivot;
    [Tooltip("Точка/объект, откуда летит снаряд и чей forward используется для проверки попадания. Если пусто - попытаемся взять первый дочерний.")]
    public Transform barrel;

    [Header("Ось прицеливания (локально на pivot)")]
    [Tooltip("Локальная ось на pivot, которую считаем 'вперёд' (обычно (0,0,1)). Если AutoDetectAxis = true, будет подобрана автоматически.")]
    public Vector3 aimForwardLocal = Vector3.forward;
    public bool autoDetectAxis = true;

    // Событие для уведомления системы оружия
    public event Action<EnemyCannon> OnReadyToFire;

    [Header("Поворот и стрельба")]
    public float rotationSpeed = 60f;   // deg/sec
    public float maxLeftRotation = 45f;
    public float maxRightRotation = 45f;
    public float fireRadius = 50f;
    public float reloadTime = 3f;
    public float aimToleranceDeg = 6f; // допустимая угловая погрешность прицеливания в градусах
    public bool autoFire = false;


    // --- внутреннее состояние ---
    [SerializeField] private Transform _target;
    [SerializeField] private GameObject _projectilePrefab;
    private float _reloadTimer;
    private bool _isReadyToFire = false;

    // pivot-related
    private Transform _pivot;
    private Transform _barrel;
    private Quaternion _initialLocalRot;       // локальная ориентация pivot в Awake (базовая)
    private Vector3 _zeroForwardLocal;         // базовый forward (в системе координат родителя pivot)

    [Header("Эффекты")]
    [SerializeField] private GameObject effectShot;

    public void Initialize(Transform target, GameObject projectilePrefab)
    {
        _target = target;
        _projectilePrefab = projectilePrefab;
    }

    private void Awake()
    {
        _pivot = pivot != null ? pivot : transform;
        _barrel = barrel != null ? barrel : (_pivot.childCount > 0 ? _pivot.GetChild(0) : _pivot);
        _initialLocalRot = _pivot.localRotation;

        // Auto-detect aim axis if requested
        if (autoDetectAxis && _barrel != null)
            AutoDetectAimAxis();

        // compute zero forward local (в локальной системе родителя pivot)
        _zeroForwardLocal = _initialLocalRot * aimForwardLocal;
    }

    private void AutoDetectAimAxis()
    {
        // Попытаемся понять, какая локальная ось pivot соответствует forward барреля
        // Кандидаты: +Z, -Z, +X, -X
        Vector3[] candidates = new Vector3[] { Vector3.forward, -Vector3.forward, Vector3.right, -Vector3.right };
        string[] names = new string[] { "+Z", "-Z", "+X", "-X" };

        float bestDot = -Mathf.Infinity;
        Vector3 best = Vector3.forward;
        int bestIndex = 0;

        for (int i = 0; i < candidates.Length; i++)
        {
            // candidate в мировом направлении:
            Vector3 candWorld = _pivot.TransformDirection(candidates[i]); // локальная ось -> world
            // сравним с forward барабеля (мировой)
            float dot = Vector3.Dot(candWorld.normalized, _barrel.forward.normalized);
            if (dot > bestDot)
            {
                bestDot = dot;
                best = candidates[i];
                bestIndex = i;
            }
        }

        aimForwardLocal = best;
    }

    private void Update()
    {
        // Обновляем перезарядку
        _reloadTimer -= Time.deltaTime;

        // Проверяем, стала ли пушка готовой к стрельбе после перезарядки
        if (_reloadTimer <= 0f && !_isReadyToFire && CanFire())
        {
            _isReadyToFire = true;
            OnReadyToFire?.Invoke(this);
        }

        if (_target == null) return;

        RotateToTarget();

        // Авто-стрельба
        //if (autoFire && _reloadTimer <= 0f)
        //{
        //    TryFire();
        //}
    }

    public bool CanFire()
    {
        if (_barrel == null || _target == null) return false;

        float dist = Vector3.Distance(_barrel.position, _target.position);
        if (dist > fireRadius) return false;

        Vector3 dirToTarget = (_target.position - _barrel.position).normalized;
        float worldAngle = Vector3.Angle(_barrel.forward, dirToTarget);

        return worldAngle <= aimToleranceDeg;
    }

    private void RotateToTarget()
    {
        if (_pivot == null || _target == null) return;

        // Работаем в локальной системе родителя pivot (если родитель есть)
        Transform parent = _pivot.parent;
        Vector3 localPivot = _pivot.localPosition;
        Vector3 localTarget;
        if (parent != null)
            localTarget = parent.InverseTransformPoint(_target.position);
        else
            localTarget = _target.position; // fallback, world coords

        Vector3 localDelta = localTarget - localPivot;
        localDelta.y = 0f;

        if (localDelta.sqrMagnitude < 0.0001f) return;

        // Целевой угол: от базового forward (zeroForwardLocal) к направлению на цель
        Vector3 localDeltaDir = localDelta.normalized;
        float targetAngle = Vector3.SignedAngle(_zeroForwardLocal, localDeltaDir, Vector3.up);

        // Текущий локальный forward pivot
        Vector3 currentForwardLocal = _pivot.localRotation * aimForwardLocal;
        float currentAngle = Vector3.SignedAngle(_zeroForwardLocal, currentForwardLocal, Vector3.up);

        // Плавный поворот к целевому углу
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        // Клэмп в пределах сектора
        float clampedAngle = Mathf.Clamp(newAngle, -maxLeftRotation, maxRightRotation);

        // Применяем: поворачиваем pivot относительно его начальной локальной ориентации
        _pivot.localRotation = Quaternion.AngleAxis(clampedAngle, Vector3.up) * _initialLocalRot;
    }

    public void TryFire()
    {
        if (_reloadTimer > 0f) return;
        if (!CanFire()) return;

        Fire();
    }

    public void ForceFire()
    {
        if (_reloadTimer > 0f) return;
        Fire();
    }

    private void Fire()
    {
        if (_projectilePrefab == null) return;
        if (_barrel == null) _barrel = _pivot;

        var bullet = BulletObjectPool.Instance.GetObject(_projectilePrefab);
        if (bullet != null)
        {
            bullet.transform.SetPositionAndRotation(_barrel.position, Quaternion.LookRotation(_barrel.forward));
            BulletContoller bulletController = bullet.GetComponent<BulletContoller>();
            if (bulletController != null)
            {
                bulletController.Initialize(_barrel.forward);
            }
        }

        EffectShot(_barrel.position, _barrel.forward);

        // Перезарядка
        _reloadTimer = reloadTime;
        _isReadyToFire = false;
    }
    private void EffectShot(Vector3 position, Vector3 direction)
    {
        GameObject effect = EffectObjectPool.Instance.GetObject(effectShot);
        effect.transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
        EffectController effectController = effect.GetComponent<EffectController>();
        if (effectController != null)
        {
            effectController.Initialize(effect);
        }
    }
}
