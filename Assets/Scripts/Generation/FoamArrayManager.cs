using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generation
{
    [ExecuteAlways]
    public class FoamArrayManager : MonoBehaviour
    {
        [Header("Настройки пены")]
        [SerializeField] private Material waterMaterial;

        [Tooltip("Максимум точек, где будет появляться пена (оптимум 16–32)")]
        [SerializeField, Range(1, 64)] private int maxFoamObjects = 16;

        [Tooltip("Цвет пены")]
        [SerializeField] private Color foamColor = Color.white;

        [Tooltip("Глобальная интенсивность пены")]
        [SerializeField, Range(0, 2)] private float foamGlobalIntensity = 1f;

        [Tooltip("Автоматически искать все ShoreFoamTrigger в сцене")]
        [SerializeField] private bool autoFindTriggers = true;

        private Vector4[] foamPosArray;
        private Vector4[] foamNormalArray;
        private ShoreFoamTrigger[] triggers;

        private static readonly int FoamCountID = Shader.PropertyToID("_FoamCount");
        private static readonly int FoamPosArrayID = Shader.PropertyToID("_FoamPosArray");
        private static readonly int FoamNormalArrayID = Shader.PropertyToID("_FoamNormalArray");
        private static readonly int FoamColorID = Shader.PropertyToID("_FoamColor");
        private static readonly int FoamGlobalIntensityID = Shader.PropertyToID("_FoamGlobalIntensity");

        private void Awake()
        {
            Initialize();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
                Initialize();
        }

        private void Initialize()
        {
            if (foamPosArray == null || foamPosArray.Length != maxFoamObjects)
                foamPosArray = new Vector4[maxFoamObjects];

            if (foamNormalArray == null || foamNormalArray.Length != maxFoamObjects)
                foamNormalArray = new Vector4[maxFoamObjects];

            if (autoFindTriggers)
                FindAllTriggers();
        }

        private void FindAllTriggers()
        {
            triggers = FindObjectsByType<ShoreFoamTrigger>(FindObjectsSortMode.None);
        }

        private void Update()
        {
            if (waterMaterial == null)
                return;

            if (triggers == null || triggers.Length == 0)
            {
                if (autoFindTriggers)
                    FindAllTriggers();
                else
                    return;
            }

            int count = Mathf.Min(triggers.Length, maxFoamObjects);

            for (int i = 0; i < count; i++)
            {
                var t = triggers[i];
                if (t == null) continue;

                Vector3 pos = t.transform.position;
                foamPosArray[i] = new Vector4(pos.x, pos.y, pos.z, t.foamRadius);

                Vector3 normal = t.foamNormal.sqrMagnitude > 0.001f ? t.foamNormal.normalized : t.transform.forward;
                foamNormalArray[i] = new Vector4(normal.x, normal.y, normal.z, 0);
            }

            // Очистка неиспользуемых элементов
            for (int i = count; i < maxFoamObjects; i++)
            {
                foamPosArray[i] = Vector4.zero;
                foamNormalArray[i] = Vector4.zero;
            }

            // Передаём данные в шейдер
            waterMaterial.SetInt(FoamCountID, count);
            waterMaterial.SetVectorArray(FoamPosArrayID, foamPosArray);
            waterMaterial.SetVectorArray(FoamNormalArrayID, foamNormalArray);
            waterMaterial.SetColor(FoamColorID, foamColor);
            waterMaterial.SetFloat(FoamGlobalIntensityID, foamGlobalIntensity);
        }
    }
}
