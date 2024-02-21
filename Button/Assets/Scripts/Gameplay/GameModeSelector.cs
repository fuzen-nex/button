using System;
using Gameplay.GameLobby;
using Jazz;
using UnityEngine;

namespace Gameplay
{
    public class GameModeSelector : MonoBehaviour
    {
        [SerializeField] private GameLobbyCanvas gameLobbyCanvasPrefab;
        
        private BodyPoseDetectionManager bodyPoseDetectionManager = null!;
        private GameLobbyCanvas gameLobbyCanvas;
        private new Camera camera;
        private int numberOfPlayers;
        
        public event Action<QuestionMode> CaptureQuestionMode;
        private void Awake()
        {
            gameLobbyCanvas = Instantiate(gameLobbyCanvasPrefab, transform);
        }
        public void Initialize(BodyPoseDetectionManager newBodyPoseDetectionManager, Camera newCamera, int newNumberOfPLayers)
        {
            camera = newCamera;
            numberOfPlayers = newNumberOfPLayers;
            var canvas = gameLobbyCanvas.GetCanvas();
            canvas.worldCamera = camera;
            canvas.planeDistance = 0.6f;
            bodyPoseDetectionManager = newBodyPoseDetectionManager;
            bodyPoseDetectionManager.processed.captureAspectNormalizedDetection += HandleDetection;
        }

        private void OnDestroy()
        {
            bodyPoseDetectionManager.processed.captureAspectNormalizedDetection -= HandleDetection;
        }

        private int leftHandId = -1;
        private int rightHandId = -1;

        private void HandleDetection(BodyPoseDetectionResult detectionResult)
        {
            for (var playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
            {
                var leftHand = HandCalculation.CalculateHandPosition(detectionResult, playerIndex, Hand.Left);
                var rightHand = HandCalculation.CalculateHandPosition(detectionResult, playerIndex, Hand.Right);
                if (leftHand != null)
                {
                    leftHandId = gameLobbyCanvas.CheckHittingButtons((Vector2)leftHand);
                }

                if (rightHand != null)
                {
                    rightHandId = gameLobbyCanvas.CheckHittingButtons((Vector2)rightHand);
                }

                if (leftHandId == rightHandId && leftHandId != -1)
                {
                    var mode = QuestionMode.ColorOnly;
                    switch (leftHandId)
                    {
                        case 0:
                            mode = QuestionMode.ColorOnly;
                            break;
                        case 1:
                            mode = QuestionMode.ShapeOnly;
                            break;
                        case 2:
                            mode = QuestionMode.ColorAndShape;
                            break;
                        default:
                            break;
                    }

                    CaptureQuestionMode?.Invoke(mode);
                    return;
                }
            }
        }
    }
}
