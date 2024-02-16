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
        private const float _coolDownTime = 2;
        public void Initialize(Color unpressedColor, Color pressedColor, int buttonId)
        {
            this.buttonId = buttonId;
            this.unpressedColor = unpressedColor;
            this.pressedColor = pressedColor;
            spriteRenderer.color = unpressedColor;
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
            if (remainTime < 0)
            {
                SetPressed(false);
                remainTime = 0;
            }
        }
    }
}


