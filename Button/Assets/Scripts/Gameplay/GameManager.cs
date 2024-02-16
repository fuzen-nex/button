#nullable enable
using Gameplay.GameElement;
using Jazz;
using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BodyPoseDetectionManager bodyPoseDetectionManager;
        [SerializeField] private GameElements gameElements;
        
        private Vector2? leftWristPos, rightWristPos;
        
        private void Awake()
        {
            Initialize();
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
            leftWristPos = leftWrist;
            rightWristPos = rightWrist;
            var buttonId = -1;
            if (leftWristPos != null) buttonId = gameElements.CheckHittingButtons((Vector2)leftWristPos!);
            if (rightWristPos != null) buttonId = gameElements.CheckHittingButtons((Vector2)rightWristPos!);
            if (buttonId != -1)
            {
                Answer(buttonId);
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

        private void Answer(int buttonId)
        {
            Debug.Log("answering button " + buttonId);
        }
    }
}
