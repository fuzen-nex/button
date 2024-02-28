using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.GameElements
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
        [SerializeField] private float buttonPressedYPosition;
        [SerializeField] private float buttonSize;
        [SerializeField] private Sign signPrefab;
        [SerializeField] private List<ButtonConfig> buttonsConfig;

        private int numberOfButtons;
        private const int _cameraHeight = 5;
        
        #region Game Elements
        
        private Wood wood;
        private List<Button> buttons;
        private List<Sign> signs;

        #endregion

        public void Initialize(int newNumberOfButtons)
        {
            numberOfButtons = newNumberOfButtons;
            InitializeWood();
            InitializeButtonsAndSigns();
        }

        private void OnDestroy()
        {
            Destroy(wood);
            foreach (var button in buttons)
            {
                Destroy(button);
            }

            foreach (var sign in signs)
            {
                Destroy(sign);
            }
        }

        private void InitializeWood()
        {
            wood = Instantiate(woodPrefab, transform);
            wood.SetScale(woodHeight);
            wood.transform.position += new Vector3(0, -(_cameraHeight - woodHeight / 2), 0);
        }

        private void InitializeButtonsAndSigns()
        {
            buttons = new List<Button>();
            signs = new List<Sign>();
            const float left = -10.0f / 9 * 16 / 2;
            const float right = 10.0f / 9 * 16 / 2;
            for (var i = 0; i < numberOfButtons; i++)
            {
                var button = Instantiate(buttonPrefab, transform);
                button.Initialize(buttonsConfig[i].unpressedColor, buttonsConfig[i].pressedColor, i, buttonYPosition, buttonPressedYPosition);
                var buttonTransform = button.transform;
                var newPosition = buttonTransform.position;
                newPosition = new Vector3(left + (right - left) / (numberOfButtons + 1) * (i + 1), buttonYPosition, newPosition.z);
                buttonTransform.position = newPosition;
                var localScale = buttonTransform.localScale;
                localScale = new Vector3(localScale.x * buttonSize, localScale.y * buttonSize, localScale.z * buttonSize);
                buttonTransform.localScale = localScale;
                buttons.Add(button);
                
                var sign = Instantiate(signPrefab, transform);
                var signTransform = sign.transform;
                newPosition = signTransform.position;
                newPosition = new Vector3(left + (right - left) / (numberOfButtons + 1) * (i + 1), buttonYPosition, newPosition.z + 1);
                signTransform.position = newPosition;
                if (i == 0) sign.InitializeAngle(Vector3.forward * 20, Vector3.forward * 80);
                else if (i == numberOfButtons - 1) sign.InitializeAngle(Vector3.forward * -20, Vector3.forward * -80);
                else if (i * 2 < numberOfButtons - 1) sign.InitializeAngle(Vector3.zero, Vector3.forward * 70);
                else if (i * 2 > numberOfButtons - 1) sign.InitializeAngle(Vector3.zero, Vector3.forward * -70);
                else sign.InitializeAngle(Vector3.zero, Vector3.right * -80);
                signs.Add(sign);
            }
        }
        public int CheckHittingButtons(Vector2 pos)
        {
            foreach (var button in buttons)
            {
                var buttonPos = button.transform.position;
                if (IsNotHit(pos, buttonPos)) continue;
                if (IsNewHit(pos, buttonPos))
                {
                    button.isNowPressed = true;
                    if (button.ableToHit)
                    {
                        return button.buttonId;
                    }
                }
                else if (button.previouslyPressed) button.isNowPressed = true;
            }
            return -1;
        }

        public void ButtonsGetReady()
        {
            foreach (var button in buttons)
            {
                button.previouslyPressed = button.isNowPressed;
                button.isNowPressed = false;
            }
        }

        public void UpdateButtonsStates()
        {
            foreach (var button in buttons.Where(button => button.isNowPressed))
            {
                button.SetPressed(true);
            }
        }
        
        private bool IsNewHit(Vector2 pos1, Vector2 pos2)
        {
            var dx = pos1.x - pos2.x;
            var dy = pos1.y - pos2.y;
            var dis = Math.Sqrt(dx * dx + dy * dy);
            return dis <= buttonSize / 2;
        }

        private bool IsNotHit(Vector2 pos1, Vector2 pos2)
        {
            var dx = pos1.x - pos2.x;
            var dy = pos1.y - pos2.y;
            var dis = Math.Sqrt(dx * dx + dy * dy);
            return dis >= buttonSize / 2 * 1.2f;
        }

        public void SetSigns(Question question)
        {
            for (var i = 0; i < numberOfButtons; i++)
            {
                var color = question.Colors[i];
                var shape = question.Shapes[i];
                signs[i].SetSign(shape, color);
            }
        }
    }
}
