using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Generation
{
    [ExecuteAlways]
    public class ShoreFoamTrigger : MonoBehaviour
    {
        [Tooltip("Радиус пены вокруг острова")]
        public float foamRadius = 5f;

        [Tooltip("Толщина кольца пены (0.1 - узкое, 2 - широкое)")]
        [Range(0.1f, 2f)] public float foamThickness = 0.5f;

        [Tooltip("Интенсивность пены (0-1)")]
        [Range(0, 1)] public float foamIntensity = 1f;
    }
}