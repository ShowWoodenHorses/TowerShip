using UnityEngine;

namespace Assets.Scripts.Generation
{
    [ExecuteAlways]
    public class FoamArrayManager : MonoBehaviour
    {
        [Header("Настройки пены")]
        [SerializeField] private Material waterMaterial;
        [SerializeField, Range(1, 64)] private int maxFoamObjects = 16;
        [SerializeField] private Color foamColor = Color.white;
        [SerializeField, Range(0, 1)] private float foamGlobalIntensity = 1f;
        [SerializeField] private bool autoFindTriggers = true;

        private Vector4[] foamDataArray;
        private ShoreFoamTrigger[] triggers;

        private void Awake()
        {
            Initialize();
        }

        private void OnValidate()
        {
            // Автообновление в редакторе
            if (!Application.isPlaying)
                Initialize();
        }

        private void Initialize()
        {
            if (foamDataArray == null || foamDataArray.Length != maxFoamObjects)
                foamDataArray = new Vector4[maxFoamObjects];

            if (autoFindTriggers)
                FindAllTriggers();
        }

        private void FindAllTriggers()
        {
            triggers = FindObjectsByType<ShoreFoamTrigger>(FindObjectsSortMode.None);
        }

        private void Update()
        {
            if (waterMaterial == null) return;
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
                foamDataArray[i] = new Vector4(pos.x, pos.y, pos.z, t.foamRadius);
            }

            // Заполняем оставшиеся элементы нулями
            for (int i = count; i < maxFoamObjects; i++)
                foamDataArray[i] = Vector4.zero;

            // Передаём данные в материал
            waterMaterial.SetInt("_FoamCount", count);
            waterMaterial.SetVectorArray("_FoamPosArray", foamDataArray);
            waterMaterial.SetColor("_FoamColor", foamColor);
            waterMaterial.SetFloat("_FoamGlobalIntensity", foamGlobalIntensity);
        }
    }
}
