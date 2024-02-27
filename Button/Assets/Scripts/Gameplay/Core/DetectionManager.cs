#nullable enable

using Gameplay;
using Gameplay.GameLobby;
using Jazz;
using UnityEngine;

namespace Nex
{
    public class DetectionManager : MonoBehaviour
    {
        [SerializeField] CvDetectionManager cvDetectionManager = null!;
        [SerializeField] BodyPoseDetectionManager bodyPoseDetectionManager = null!;
        [SerializeField] PlayersManager playersManager = null!;
        [SerializeField] PreviewsManager previewsManager = null!;
        [SerializeField] SetupStateManager setupStateManager = null!;
        [SerializeField] SetupUI setupUI = null!;
        [SerializeField] private GameLobbyManager gameLobbyManager = null!;

        private int numOfPlayers; // Here is where we can config 1P / 2P game.
        #region Life Cycle

        void Awake()
        {
            numOfPlayers = gameLobbyManager.GetNumberOfPlayers();
            Initialize();
        }

        void Initialize()
        {

            ConfigMdk(numOfPlayers);

            playersManager.Initialize(numOfPlayers, bodyPoseDetectionManager);
            previewsManager.Initialize(numOfPlayers, cvDetectionManager, bodyPoseDetectionManager);
            setupStateManager.Initialize(numOfPlayers, cvDetectionManager, bodyPoseDetectionManager);
            setupUI.Initialize(numOfPlayers, setupStateManager);

            setupStateManager.SetTrackingEnabled(true);
        }

        void ConfigMdk(int numNumOfPlayers)
        {
            cvDetectionManager.numOfPlayers = numNumOfPlayers;
            CvDetectionManager.gameViewportController.SetUseDetectionViewportForPlayerTracking(true);
        }

        #endregion
    }
}
