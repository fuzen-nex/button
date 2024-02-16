using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Gameplay.GameElement
{
    [Serializable]
    public class ButtonConfig
    {
        public Color unpressedColor;
        public Color pressedColor;
    }
    public class GameElements : MonoBehaviour
    {
        [SerializeField] private Wood woodPrefab;
        [SerializeField] private float woodHeight;
        [SerializeField] private Button buttonPrefab;
        [SerializeField] private float buttonYPosition;
        [SerializeField] private float buttonSize;
        [SerializeField] private List<ButtonConfig> buttonsConfig;
        
        private const int _cameraHeight = 5;
        
        #region Game Elements
        
        private Wood wood;
        private List<Button> buttons;

        #endregion
        public void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            InitializeWood();
            InitializeButtons();
        }

        private void InitializeWood()
        {
            wood = Instantiate(woodPrefab, transform);
            wood.SetScale(woodHeight);
            wood.transform.position += new Vector3(0, -(_cameraHeight - woodHeight / 2), 0);
        }

        private void InitializeButtons()
        {
            buttons = new List<Button>();
            const float left = -10.0f / 9 * 16 / 2;
            const float right = 10.0f / 9 * 16 / 2;
            var numberOfButtons = buttonsConfig.Count;
            for (var i = 0; i < numberOfButtons; i++)
            {
                var button = Instantiate(buttonPrefab, transform);
                button.Initialize(buttonsConfig[i].unpressedColor, buttonsConfig[i].pressedColor, i);
                var buttonTransform = button.transform;
                var newPosition = buttonTransform.position;
                newPosition = new Vector3(left + (right - left) / (numberOfButtons + 1) * (i + 1), buttonYPosition, newPosition.z);
                buttonTransform.position = newPosition;
                var localScale = buttonTransform.localScale;
                localScale = new Vector3(localScale.x * buttonSize, localScale.y * buttonSize, localScale.z * buttonSize);
                buttonTransform.localScale = localScale;
                buttons.Add(button);
            }
        }
        public int CheckHittingButtons(Vector2 pos)
        {
            foreach (var button in buttons)
            {
                if (IsHitting(pos, button.transform.position) && button.SetPressed(true))
                {
                    return button.buttonId;
                }
            }
            return -1;
        }

        private bool IsHitting(Vector2 pos1, Vector2 pos2)
        {
            
            var dx = pos1.x - pos2.x;
            var dy = pos1.y - pos2.y;
            var dis = Math.Sqrt(dx * dx + dy * dy);
            const float wristHittingMultiplier = 1.2f;
            if (dis <= buttonSize / 2 * wristHittingMultiplier)
            {
                Debug.Log(pos1 + " " + pos2 + " hit");
                return true;
            }
            return false;
        }
    }
}
