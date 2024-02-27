using Jazz;
using UnityEngine;

namespace Gameplay
{
    public class GameLobbyManager : MonoBehaviour
    {
        [SerializeField] private BodyPoseDetectionManager bodyPoseDetectionManager;
        [SerializeField] private GameManager gameManagerPrefab;
        [SerializeField] private GameModeSelector gameModeSelectorPrefab;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private int numberOfPlayers;
        [SerializeField] private int numberOfButtons;
        
        private GameManager gameManager;
        private GameModeSelector gameModeSelector;
        private QuestionMode questionMode = QuestionMode.ColorAndShape;
        private void Awake()
        {
            StartLobby();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (gameModeSelector is not null)
                {
                    Destroy(gameModeSelector.gameObject);
                    gameModeSelector = null;
                }

                if (gameManager is not null)
                {
                    Destroy(gameManager.gameObject);
                    gameManager = null;
                }
                StartLobby();
            }
        }

        private void StartLobby()
        {
            ChooseGameMode();
        }

        private void ChooseGameMode()
        {
            gameModeSelector = Instantiate(gameModeSelectorPrefab, transform);
            gameModeSelector.Initialize(bodyPoseDetectionManager, mainCamera, numberOfPlayers);
            gameModeSelector.CaptureQuestionMode += ChoseGameMode;
        }

        private void ChoseGameMode(QuestionMode mode)
        {
            questionMode = mode;
            if (gameModeSelector is not null)
            {
                Destroy(gameModeSelector.gameObject);
                gameModeSelector = null;
            }
            StartGame();
        }
        private void StartGame()
        {
            gameManager = Instantiate(gameManagerPrefab, transform);
            gameManager.Initialize(bodyPoseDetectionManager, questionMode, numberOfPlayers, numberOfButtons);
        }

        public int GetNumberOfPlayers()
        {
            return numberOfPlayers;
        }
    }
}
