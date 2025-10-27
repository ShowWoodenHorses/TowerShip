using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CircleNoFire : MonoBehaviour
    {
        public void Initialize(float radiusNoFire)
        {
            float scaleFactor = radiusNoFire;
            transform.localScale = new Vector3(scaleFactor, transform.localScale.y, scaleFactor);
        }

        public void ShowCircleNoFire() => transform.gameObject.SetActive(true);
        public void HideCircleNoFire() => transform.gameObject.SetActive(false);
    }
}
