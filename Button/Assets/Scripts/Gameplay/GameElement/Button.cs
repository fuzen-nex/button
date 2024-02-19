using System;
using UnityEngine;

namespace Gameplay.GameElement
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public int buttonId;
        
        private Color pressedColor, unpressedColor;

        public bool isPressed;
        private float remainTime = 0;
        private const float _coolDownTime = 1.5f;
        public void Initialize(Color newUnpressedColor, Color newPressedColor, int newButtonId)
        {
            buttonId = newButtonId;
            unpressedColor = newUnpressedColor;
            pressedColor = newPressedColor;
            spriteRenderer.color = newUnpressedColor;
            isPressed = false;
        }

        public bool SetPressed(bool pressed)
        {
            if (pressed)
            {
                if (remainTime > 0) return false;
                isPressed = true;
                spriteRenderer.color = pressedColor;
                remainTime = _coolDownTime;
                return true;
            }
            isPressed = false;
            spriteRenderer.color = unpressedColor;
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


