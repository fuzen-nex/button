#nullable enable

using Jazz;
using UnityEngine;

namespace Nex
{
    public class PreviewsManager : MonoBehaviour
    {
        [SerializeField] PreviewFrame previewFramePrefab = null!;
        [SerializeField] GameObject fullFramePreviewContainer = null!;
        [SerializeField] GameObject p1PreviewContainer = null!;
        [SerializeField] GameObject p2PreviewContainer = null!;

        #region Public

        public void Initialize(
            int aNumOfPlayers,
            CvDetectionManager aCvDetectionManager,
            BodyPoseDetectionManager aBodyPoseDetectionManager
        )
        {
            var displayMode = PreviewFrame.DisplayMode.Full;
            var previewContainer = fullFramePreviewContainer;
            previewContainer.SetActive(true);
            var previewFrame = Instantiate(previewFramePrefab, previewContainer.transform);
            previewFrame.Initialize(displayMode, aCvDetectionManager, aBodyPoseDetectionManager);
            // This only works for showing preview for individual players.
            /*
            for (var playerIndex = 0; playerIndex < aNumOfPlayers; playerIndex++)
            {
                var displayMode = aNumOfPlayers == 1 ? PreviewFrame.DisplayMode.Full :
                    playerIndex == 0 ? PreviewFrame.DisplayMode.LeftHalf :
                    PreviewFrame.DisplayMode.RightHalf;
                var previewContainer = GetPreviewContainer(playerIndex, aNumOfPlayers);
                previewContainer.SetActive(true);

                var previewFrame = Instantiate(previewFramePrefab, previewContainer.transform);
                previewFrame.Initialize(displayMode, aCvDetectionManager, aBodyPoseDetectionManager);
            }
            */
        }

        #endregion

        #region Config

        GameObject GetPreviewContainer(int playerIndex, int numOfPlayers)
        {
            return numOfPlayers == 1 ? fullFramePreviewContainer :
                playerIndex == 0 ? p1PreviewContainer : p2PreviewContainer;
        }

        #endregion
    }
}
