using Jazz;
using UnityEngine;

namespace Gameplay
{
    public class GameLobbyManager : MonoBehaviour
    {
        [SerializeField] private BodyPoseDetectionManager bodyPoseDetectionManager;
        [SerializeField] private GameManager gameManagerPrefab;
        
        private GameManager gameManager;
        private QuestionMode questionMode = QuestionMode.ColorAndShape;
        private void Awake()
        {
            StartLobby();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Destroy(gameManager.gameObject);
                StartLobby();
            }
        }

        private void StartLobby()
        {
            ChooseGameMode();
            StartGame();
        }

        private void ChooseGameMode()
        {
            questionMode = QuestionMode.ColorAndShape;
        }
        private void StartGame()
        {
            gameManager = Instantiate(gameManagerPrefab, transform);
            gameManager.Initialize(bodyPoseDetectionManager, questionMode);
        }
    }
}
