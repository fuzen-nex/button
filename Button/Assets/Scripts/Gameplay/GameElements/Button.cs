using UnityEngine;

namespace Gameplay.GameElements
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public int buttonId;
        
        private Color pressedColor, unpressedColor;

        public bool previouslyPressed;
        public bool isNowPressed;
        private float remainTime = 0;
        private const float _coolDownTime = 0.4f;
        private float normalYPosition;
        private float pressedYPosition;
        public bool ableToHit;
        
        public void Initialize(Color newUnpressedColor, Color newPressedColor, int newButtonId, float yPos, float pressedYPos)
        {
            buttonId = newButtonId;
            unpressedColor = newUnpressedColor;
            pressedColor = newPressedColor;
            spriteRenderer.color = newUnpressedColor;
            previouslyPressed = isNowPressed = false;
            normalYPosition = yPos;
            pressedYPosition = pressedYPos;
            ableToHit = true;
        }

        public void SetPressed(bool pressed)
        {
            if (pressed)
            {
                ResetRemainTime();
                ableToHit = false;
            }
            else ableToHit = true;
            
            var pos = gameObject.transform.position;
            spriteRenderer.color = pressed ? pressedColor : unpressedColor;
            gameObject.transform.position = new Vector3(pos.x, pressed ? pressedYPosition : normalYPosition, pos.z);
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

        private void ResetRemainTime()
        {
            remainTime = _coolDownTime;
        }
    }
}


