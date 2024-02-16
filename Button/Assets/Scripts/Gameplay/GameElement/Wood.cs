using UnityEngine;

namespace Gameplay.GameElement
{
    public class Wood : MonoBehaviour
    {
        [SerializeField] private Transform woodTransform;

        public void SetScale(float height)
        {
            var localScale = woodTransform.localScale;
            localScale = new Vector3(localScale.x, height, localScale.z);
            woodTransform.localScale = localScale;
        }
    }
}
