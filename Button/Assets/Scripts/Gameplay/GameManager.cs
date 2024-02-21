#nullable enable
using Gameplay.GameElements;
using Jazz;
using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        
        [SerializeField] private GameElements.GameElements gameElementsPrefab = null!;
        [SerializeField] private QuestionManager questionManagerPrefab = null!;
        [SerializeField] private GameplayCanvas gameplayCanvasPrefab = null!;
        
        private GameElements.GameElements gameElements = null!;
        private QuestionManager questionManager = null!;
        private GameplayCanvas gameplayCanvas = null!;
        private QuestionMode questionMode;

        private BodyPoseDetectionManager bodyPoseDetectionManager = null!;
        
        private bool startNextQuestion;
        private Question currentQuestion = new();
        private int score;
        private float remainTime;
        private bool startedGame;
        private bool endedGame;
        public void Initialize(BodyPoseDetectionManager newBodyPoseDetectionManager, QuestionMode newQuestionMode)
        {
            bodyPoseDetectionManager = newBodyPoseDetectionManager;
            questionMode = newQuestionMode;
            InitializeElements();
            StartGame();
        }
        private void StartGame()
        {
            score = 0;
            ChangeScore(0);
            remainTime = 59.99f;
            UpdateRemainTime();
            startNextQuestion = true;
            startedGame = true;
            gameplayCanvas.SetActiveScoreText(true);
            gameplayCanvas.SetActiveRemainTime(true);
            gameplayCanvas.SetActiveQuestionHint(true);
            gameplayCanvas.SetActiveEndGameScore(false);
        }
        
        private void FixedUpdate()
        {
            if (startedGame == false || endedGame) return;
            remainTime -= Time.fixedDeltaTime;
            UpdateRemainTime();
            if (remainTime < 0)
            {
                EndGame();
            }
            if (startNextQuestion)
            {
                NewQuestion();
            }
        }

        private void EndGame()
        {
            endedGame = true;
            startNextQuestion = false;
            bodyPoseDetectionManager.processed.captureAspectNormalizedDetection -= HandleDetection;
            Destroy(gameElements.gameObject);
            gameplayCanvas.SetEndGameScore("Score: " + score);
            gameplayCanvas.SetActiveScoreText(false);
            gameplayCanvas.SetActiveRemainTime(false);
            gameplayCanvas.SetActiveEndGameScore(true);
        }

        private void InitializeElements()
        {
            gameElements = Instantiate(gameElementsPrefab, transform);
            questionManager = Instantiate(questionManagerPrefab, transform);
            gameplayCanvas = Instantiate(gameplayCanvasPrefab, transform);
            gameElements.Initialize();
            bodyPoseDetectionManager.processed.captureAspectNormalizedDetection += HandleDetection;
        }

        private void OnDestroy()
        {
            bodyPoseDetectionManager.processed.captureAspectNormalizedDetection -= HandleDetection;
        }

        private void HandleDetection(BodyPoseDetectionResult detectionResult)
        {
            const int playerIndex = 0;
            var leftHand = HandCalculation.CalculateHandPosition(detectionResult, playerIndex, Hand.Left);
            var rightHand = HandCalculation.CalculateHandPosition(detectionResult, playerIndex, Hand.Right);
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

        private void NewQuestion()
        {
            startNextQuestion = false;
            currentQuestion = questionManager.GenerateQuestion(gameElements.GetNumberOfButtons(), questionMode);
            gameElements.SetSigns(currentQuestion);
            gameplayCanvas.SetQuestionHint(currentQuestion.QueryString);
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
            gameplayCanvas.SetScoreText("Score: " + score);
        }

        private void UpdateRemainTime()
        {
            var time = (int)remainTime + 1;
            gameplayCanvas.SetRemainTime("Time: " + time);
        }
    }
}
