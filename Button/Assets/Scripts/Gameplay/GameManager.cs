#nullable enable
using System;
using Gameplay.GameElements;
using Jazz;
using UnityEngine;

namespace Gameplay
{
    public enum GameState
    {
        Initializing,
        CountingDown,
        Started,
        Ended,
    }
    public class GameManager : MonoBehaviour
    {
        
        [SerializeField] private GameElements.GameElements gameElementsPrefab = null!;
        [SerializeField] private QuestionManager questionManagerPrefab = null!;
        [SerializeField] private GameplayCanvas gameplayCanvasPrefab = null!;
        
        private GameElements.GameElements gameElements = null!;
        private QuestionManager questionManager = null!;
        private GameplayCanvas gameplayCanvas = null!;
        private QuestionMode questionMode;
        private int numberOfPlayers;
        private int numberOfButtons;
        
        private BodyPoseDetectionManager bodyPoseDetectionManager = null!;
        
        private bool startNextQuestion;
        private Question currentQuestion = new();
        private int score;
        private float remainTime;

        private GameState gameState;
        private float countDownTime;
        
        public void Initialize(BodyPoseDetectionManager newBodyPoseDetectionManager, QuestionMode newQuestionMode,
            int newNumberOfPlayers, int newNumberOfButtons)
        {
            gameState = GameState.Initializing;
            countDownTime = 2.99f;
            bodyPoseDetectionManager = newBodyPoseDetectionManager;
            numberOfPlayers = newNumberOfPlayers;
            numberOfButtons = newNumberOfButtons;
            questionMode = newQuestionMode;
            InitializeCanvas();
        }
        private void StartGame()
        {
            score = 0;
            ChangeScore(0);
            remainTime = 59.99f;
            UpdateRemainTime();
            startNextQuestion = true;
            gameplayCanvas.StartGameSetUp();
        }
        
        private void FixedUpdate()
        {
            switch (gameState)
            {
                case GameState.Initializing:
                    break;
                case GameState.CountingDown:
                    countDownTime -= Time.fixedDeltaTime;
                    if (countDownTime < 0)
                    {
                        InitializeElements();
                        gameState = GameState.Started;
                        StartGame();
                    }
                    else UpdateCountDown();
                    break;
                case GameState.Started:
                {
                    remainTime -= Time.fixedDeltaTime;
                    UpdateRemainTime();
                    if (remainTime < 0)
                    {
                        gameState = GameState.Ended;
                        EndGame();
                    }

                    if (startNextQuestion)
                    {
                        NewQuestion();
                    }

                    break;
                }
                case GameState.Ended:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void EndGame()
        {
            bodyPoseDetectionManager.processed.captureAspectNormalizedDetection -= HandleDetection;
            Destroy(gameElements.gameObject);
            gameplayCanvas.SetEndGameScore("Score: " + score);
            gameplayCanvas.EndGameSetUp();
        }

        private void InitializeCanvas()
        {
            gameplayCanvas = Instantiate(gameplayCanvasPrefab, transform);
            gameplayCanvas.CountDownSetUp();
            gameState = GameState.CountingDown;
        }
        
        private void InitializeElements()
        {
            gameElements = Instantiate(gameElementsPrefab, transform);
            questionManager = Instantiate(questionManagerPrefab, transform);
            gameElements.Initialize(numberOfButtons);
            bodyPoseDetectionManager.processed.captureAspectNormalizedDetection += HandleDetection;
        }

        private void OnDestroy()
        {
            bodyPoseDetectionManager.processed.captureAspectNormalizedDetection -= HandleDetection;
        }

        private void HandleDetection(BodyPoseDetectionResult detectionResult)
        {
            gameElements.ButtonsGetReady();
            for (var playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
            {
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

            gameElements.UpdateButtonsStates();
        }

        private void NewQuestion()
        {
            startNextQuestion = false;
            currentQuestion = questionManager.GenerateQuestion(numberOfButtons, questionMode);
            gameElements.SetSigns(currentQuestion);
            gameplayCanvas.SetQuestionHint(currentQuestion.QueryString);
            currentQuestion.QueryAudioSource.Play();
        }
        private void Answer(int buttonId)
        {
            Debug.Log("answering button " + buttonId + " correct answer is " + currentQuestion.Answer);
            if (buttonId == currentQuestion.Answer)
            {
                ChangeScore(10);
                startNextQuestion = true;
            }
            else
            {
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

        private void UpdateCountDown()
        {
            var time = (int)countDownTime + 1;
            gameplayCanvas.SetCountDown(time.ToString());
        }
    }
}
