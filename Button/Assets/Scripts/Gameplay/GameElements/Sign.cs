using UnityEngine;

namespace Gameplay.GameElements
{
    public class Sign : MonoBehaviour
    {
        [SerializeField] private SignChoice signChoice;
        private Vector3 angle;
        private const float _animationConstant = 15;

        private void Update()
        {
            if (angle != Vector3.zero)
            {
                gameObject.transform.Rotate(-angle / _animationConstant);
                angle -= angle / _animationConstant;
            }
        }
        
        public void InitializeAngle(Vector3 finalAngle, Vector3 animationAngle)
        {
            angle = animationAngle;
            gameObject.transform.Rotate(finalAngle + animationAngle);
        }
        
        public void SetSign(SignShape shape, Color color)
        {
            signChoice.SetShape(shape);
            signChoice.SetColor(color);
        }
    }
}