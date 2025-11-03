using UnityEngine;

namespace Assets.Scripts.Generation
{
    public class ShoreFoamTrigger : MonoBehaviour
    {
        [Tooltip("Радиус области пены вокруг берега")]
        public float foamRadius = 3f;

        [Tooltip("Направление, куда пена должна растекаться (обычно от берега в воду)")]
        public Vector3 foamNormal = Vector3.forward;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, foamRadius);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + foamNormal.normalized * foamRadius);
        }
    }
}
