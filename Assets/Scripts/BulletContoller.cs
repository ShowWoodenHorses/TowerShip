using System.Collections;
using Assets.Scripts.Configs;
using Assets.Scripts.Interface;
using Assets.Scripts.ObjectPool;
using Assets.Scripts.Sound;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class BulletContoller : MonoBehaviour
    {
        [SerializeField] private protected float speed;
        [SerializeField] private protected int damageEnemy;
        [SerializeField] private protected int damageBuilding;
        [SerializeField] private protected float lifeBeforeDestroy;

        [SerializeField] private protected GameObject effectShotInWater;
        [SerializeField] private protected GameObject effectShotInBuilding;
        [SerializeField] private protected GameObject effectShotInEnemy;

        [Header("Sounds")]
        [SerializeField] private protected AudioSource soundShotPrefab;
        [SerializeField] private protected AudioSource soundShotWavePrefab;
        [SerializeField] private protected AudioSource soundTakeDamagePrefab;

        [SerializeField] private protected BulletConfig bulletConfig;

        private protected Rigidbody rb;

        private protected void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Initialize(Vector3 pos)
        {
            rb.linearVelocity = pos * speed;
            SoundPoolManager.Instance.PlaySound(soundShotPrefab);

            StartCoroutine(LifeBeforeDestroy());
        }
        public void InitializeWithTimer(Vector3 pos, float distance)
        {
            float time = distance / speed;
            lifeBeforeDestroy = time;
            rb.linearVelocity = pos * speed;
            SoundPoolManager.Instance.PlaySound(soundShotPrefab);

            StartCoroutine(LifeBeforeDestroy());
        }

        private protected void Deactive()
        {
            BulletObjectPool.Instance.ReturnObject(gameObject);
        }

        private protected virtual IEnumerator LifeBeforeDestroy()
        {
            yield return new WaitForSeconds(lifeBeforeDestroy);
            PLaySoundEffect(soundShotWavePrefab);
            SpawnEffectShotInWater();
            Deactive();
        }

        private protected void OnTriggerEnter(Collider other)
        {
            var objectForDamage = other.gameObject.GetComponent<IDamagable>();
            var building = other.gameObject.GetComponent<IObstaclable>();

            if (building != null && objectForDamage != null)
            {
                PLaySoundEffect(soundTakeDamagePrefab);
                SpawnEffect(effectShotInBuilding);
                objectForDamage.TakeDamage(damageBuilding);
                Deactive();
            }

            else if (objectForDamage != null)
            {
                PLaySoundEffect(soundTakeDamagePrefab);
                SpawnEffect(effectShotInEnemy);
                objectForDamage.TakeDamage(damageEnemy);
                Deactive();
            }

            else if (building != null)
            {
                PLaySoundEffect(soundTakeDamagePrefab);
                SpawnEffect(effectShotInBuilding);
                Deactive();
            }
        }

        private protected void SpawnEffect(GameObject effectObj)
        {
            GameObject effect = EffectObjectPool.Instance.GetObject(effectObj);
            effect.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            EffectController effectController = effect.GetComponent<EffectController>();
            if (effectController != null)
            {
                effectController.Initialize(effect);
            }
        }

        private protected void SpawnEffectShotInWater()
        {
            GameObject effect = EffectObjectPool.Instance.GetObject(effectShotInWater);
            Vector3 position = new Vector3(transform.position.x, 0f, transform.position.z);

            effect.transform.SetPositionAndRotation(position, Quaternion.identity);
            EffectController effectController = effect.GetComponent<EffectController>();
            if (effectController != null)
            {
                effectController.Initialize(effect);
            }
        }

        private protected void PLaySoundEffect(AudioSource audioSource)
        {
            SoundPoolManager.Instance.PlaySound(audioSource);
        }

        public void SetSettings()
        {
            speed = bulletConfig.speed;
            damageBuilding = bulletConfig.damageBuilding;
            damageEnemy = bulletConfig.damageEnemy;
            lifeBeforeDestroy = bulletConfig.lifeBeforeDestroy;
            soundShotPrefab = bulletConfig.soundShotPrefab;
        }

        public void SetSpeed(float s)
        {
            speed = s;
        }

        public void SetDamageEnemy(int dmg)
        {
            damageEnemy = dmg;
        }

        public void SetDamageBuilding(int dmg)
        {
            damageBuilding = dmg;
        }

        public void SetLifeTime(float time)
        {
            lifeBeforeDestroy = time;
        }

        public void SetSoundShot(AudioSource audio)
        {
            soundShotPrefab = audio;
        }
    }
}