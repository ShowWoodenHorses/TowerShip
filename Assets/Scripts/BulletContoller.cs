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
        [SerializeField] private float speed;
        [SerializeField] private int damageEnemy;
        [SerializeField] private int damageBuilding;
        [SerializeField] private float lifeBeforeDestroy;

        [SerializeField] private GameObject effectShotInWater;
        [SerializeField] private GameObject effectShotInBuilding;
        [SerializeField] private GameObject effectShotInEnemy;

        [Header("Sounds")]
        [SerializeField] private AudioSource soundShotPrefab;
        [SerializeField] private AudioSource soundShotWavePrefab;
        [SerializeField] private AudioSource soundTakeDamagePrefab;

        [SerializeField] private BulletConfig bulletConfig;

        private Rigidbody rb;

        private void Awake()
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

        private void Deactive()
        {
            BulletObjectPool.Instance.ReturnObject(gameObject);
        }

        IEnumerator LifeBeforeDestroy()
        {
            yield return new WaitForSeconds(lifeBeforeDestroy);
            PLaySoundEffect(soundShotWavePrefab);
            SpawnEffectShotInWater();
            Deactive();
        }

        private void OnTriggerEnter(Collider other)
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

        private void SpawnEffect(GameObject effectObj)
        {
            GameObject effect = EffectObjectPool.Instance.GetObject(effectObj);
            effect.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            EffectController effectController = effect.GetComponent<EffectController>();
            if (effectController != null)
            {
                effectController.Initialize(effect);
            }
        }

        private void SpawnEffectShotInWater()
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

        private void PLaySoundEffect(AudioSource audioSource)
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