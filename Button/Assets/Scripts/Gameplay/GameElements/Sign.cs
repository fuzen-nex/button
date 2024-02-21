using UnityEngine;

namespace Gameplay.GameElements
{
    public class Sign : MonoBehaviour
    {
        [SerializeField] private SignChoice signChoice;

        private void Initialize()
        {
        
        }

        public void SetSign(SignShape shape, Color color)
        {
            signChoice.SetShape(shape);
            signChoice.SetColor(color);
        }
    }
}