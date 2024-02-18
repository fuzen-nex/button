using UnityEngine;

namespace Gameplay.GameElement
{
    public enum SignShape
    {
        Triangle,
        Circle,
        Square,
        Rectangle
    }
    public class SignChoice : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer triangle;
        [SerializeField] private SpriteRenderer circle;
        [SerializeField] private SpriteRenderer square;
        [SerializeField] private SpriteRenderer rectangle;
        
        public void SetShape(SignShape shape)
        {
            triangle.enabled = false;
            circle.enabled = false;
            square.enabled = false;
            rectangle.enabled = false;
            switch (shape)
            {
                case SignShape.Triangle:
                    triangle.enabled = true;
                    break;
                case SignShape.Circle:
                    circle.enabled = true;
                    break;
                case SignShape.Square:
                    square.enabled = true;
                    break;
                case SignShape.Rectangle:
                    rectangle.enabled = true;
                    break;
                default:
                    break;
            }
        }

        public void SetColor(Color color)
        {
            triangle.color = color;
            circle.color = color;
            square.color = color;
            rectangle.color = color;
        }
    }
}
