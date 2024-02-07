using Jazz;
using UnityEngine;

namespace Nex
{
    public class OnePlayerController : MonoBehaviour
    {
        [SerializeField] OnePlayerDetectionEngine detectionEngine = null!;

        #region Public

        public void Initialize(
            int aPlayerIndex,
            int numOfPlayers,
            BodyPoseDetectionManager aBodyPoseDetectionManager
        )
        {
            detectionEngine.Initialize(aPlayerIndex, aBodyPoseDetectionManager);
        }

        #endregion


    }
}
