using DG.Tweening;
using UnityEngine;

namespace Gameplay.GameElements
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public int buttonId;
        
        private Color pressedColor, unpressedColor;

        public bool isPressed;
        private float remainTime = 0;
        private const float _coolDownTime = 1.5f;
        private float normalYPosition;
        private float pressedYPosition;
        public void Initialize(Color newUnpressedColor, Color newPressedColor, int newButtonId, float yPos, float pressedYPos)
        {
            buttonId = newButtonId;
            unpressedColor = newUnpressedColor;
            pressedColor = newPressedColor;
            spriteRenderer.color = newUnpressedColor;
            isPressed = false;
            normalYPosition = yPos;
            pressedYPosition = pressedYPos;
        }

        public bool SetPressed(bool pressed)
        {
            var pos = gameObject.transform.position;
            if (pressed)
            {
                if (remainTime > 0) return false;
                isPressed = true;
                spriteRenderer.color = pressedColor;
                remainTime = _coolDownTime;
                gameObject.transform.position = new Vector3(pos.x, pressedYPosition, pos.z);
                return true;
            }
            isPressed = false;
            spriteRenderer.color = unpressedColor;
            gameObject.transform.position = new Vector3(pos.x, normalYPosition, pos.z);
            return true;
        }
        private void FixedUpdate()
        {
            if (remainTime > 0) remainTime -= Time.fixedDeltaTime;
            else
            {
                SetPressed(false);
                remainTime = 0;
            }
        }
    }
}


