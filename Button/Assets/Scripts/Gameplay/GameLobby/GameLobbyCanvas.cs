using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameLobby
{
    public class GameLobbyCanvas : MonoBehaviour
    {
        [SerializeField] private List<GameModeButton> gameModeButtons;
        [SerializeField] private Canvas canvas;
        public int CheckHittingButtons(Vector2 pos)
        {
            foreach (var button in gameModeButtons)
            {
                var buttonPos = (Vector2)button.transform.position;
                if (IsHitting(pos, buttonPos)) return button.GetButtonId();
            }
            return -1;
        }
        private bool IsHitting(Vector2 pos1, Vector2 pos2)
        {
            
            var dx = pos1.x - pos2.x;
            var dy = pos1.y - pos2.y;
            var dis = Math.Sqrt(dx * dx + dy * dy);
            return dis <= 2;
        }
        
        public Canvas GetCanvas()
        {
            return canvas;
        }
    }
}
