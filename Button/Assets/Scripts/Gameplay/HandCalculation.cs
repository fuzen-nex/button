using Jazz;
using UnityEngine;

namespace Gameplay
{
    public enum Hand
    {
        Left,
        Right
    }
    public class HandCalculation : MonoBehaviour
    {
        public static Vector2? CalculateHandPosition(BodyPoseDetectionResult detectionResult, int playerIndex, Hand hand)
        {
            var playerPose = detectionResult.processed.GetPlayerPose(playerIndex);
            var pose = playerPose?.bodyPose;
            var wrist = CalculatePosition((hand == Hand.Left ? pose?.LeftWrist().ToVector2() : pose?.RightWrist().ToVector2()));
            var elbow = CalculatePosition(hand == Hand.Left? pose?.LeftElbow().ToVector2() : pose?.RightElbow().ToVector2());
            Vector2? handPosition = null;
            if (elbow != null && wrist != null)
            {
                handPosition = Vector2.LerpUnclamped((Vector2)elbow, (Vector2)wrist, 1.3f);
            }
            return handPosition;
        }

        private static Vector2? CalculatePosition(Vector2? originalPosition)
        {
            if (originalPosition is null) return null;
            var newPosition = (Vector2)originalPosition * 10;
            newPosition.x -= 10.0f / 9 * 16 / 2;
            newPosition.y -= 10.0f / 2;
            return newPosition;
        }
    }
}
