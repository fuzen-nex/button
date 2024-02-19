#nullable enable
using Gameplay.GameElement;
using Jazz;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BodyPoseDetectionManager bodyPoseDetectionManager = null!;
        [SerializeField] private GameElements gameElements = null!;
        [SerializeField] private QuestionManager questionManager = null!;
        [SerializeField] private QuestionMode questionMode;
        [SerializeField] private TextMeshProUGUI scoreText = null!;
        
        private bool startNextQuestion;
        private Question currentQuestion = new();
        private int score;
        private void Awake()
        {
            Initialize();
            StartGame();
        }

        private void StartGame()
        {
            score = 0;
            startNextQuestion = true;
        }

        private void FixedUpdate()
        {
            if (startNextQuestion)
            {
                NewQuestion();
            }
        }

        private void Initialize()
        {
            bodyPoseDetectionManager.processed.captureAspectNormalizedDetection += HandleDetection;
        }

        private void OnDestroy()
        {
            bodyPoseDetectionManager.processed.captureAspectNormalizedDetection -= HandleDetection;
        }

        private void HandleDetection(BodyPoseDetectionResult detectionResult)
        {
            const int playerIndex = 0;
            var playerPose = detectionResult.processed.GetPlayerPose(playerIndex);
            var pose = playerPose?.bodyPose;
            var leftWrist = CalculatePosition(pose?.LeftWrist().ToVector2());
            var rightWrist = CalculatePosition(pose?.RightWrist().ToVector2());
            var leftElbow = CalculatePosition(pose?.LeftElbow().ToVector2());
            var rightElbow = CalculatePosition(pose?.RightElbow().ToVector2());
            Vector2? leftHand = null;
            Vector2? rightHand = null;
            if (leftElbow != null && leftWrist != null)
            {
                leftHand = Vector2.LerpUnclamped((Vector2)leftElbow, (Vector2)leftWrist, 1.3f);
            }

            if (rightElbow != null && rightWrist != null)
            {
                rightHand = Vector2.LerpUnclamped((Vector2)rightElbow, (Vector2)rightWrist, 1.3f);
            }
            
            if (leftHand != null)
            {
                var buttonId = gameElements.CheckHittingButtons((Vector2)leftHand);
                if (buttonId != -1) Answer(buttonId);
            }
            
            if (rightHand != null)
            {
                var buttonId = gameElements.CheckHittingButtons((Vector2)rightHand);
                if (buttonId != -1) Answer(buttonId);
            }
        }

        private static Vector2? CalculatePosition(Vector2? originalPosition)
        {
            if (originalPosition is null) return null;
            var newPosition = (Vector2)originalPosition * 10;
            newPosition.x -= 10.0f / 9 * 16 / 2;
            newPosition.y -= 10.0f / 2;
            return newPosition;
        }

        private void NewQuestion()
        {
            startNextQuestion = false;
            currentQuestion = questionManager.GenerateQuestion(gameElements.GetNumberOfButtons(), questionMode);
            gameElements.SetSigns(currentQuestion);
            currentQuestion.QueryAudioSource.Play();
        }
        private void Answer(int buttonId)
        {
            Debug.Log("answering button " + buttonId + " correct answer is " + currentQuestion.Answer);
            if (buttonId == currentQuestion.Answer)
            {
                Debug.Log("correct");
                ChangeScore(10);
                startNextQuestion = true;
            }
            else
            {
                Debug.Log("wrong");
                ChangeScore(-5);
                currentQuestion.QueryAudioSource.Play();
            }
        }

        private void ChangeScore(int delta)
        {
            score += delta;
            scoreText.text = "Score: " + score;
        }
    }
}
