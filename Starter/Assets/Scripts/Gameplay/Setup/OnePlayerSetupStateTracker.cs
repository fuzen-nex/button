using System;
using Jazz;
using UnityEngine;
using UnityEngine.Events;

#nullable enable

namespace Nex
{
    public enum SetupCheckType
    {
        GoodPosition,
        RaisingHand
    }

    public struct SetupHistoryItem
    {
        public SetupIssueType SetupIssue;
        public bool IsRaisingLeftHand;
        public bool IsRaisingRightHand;

        #region Public

        public void UpdateStateWithPose(BodyPose? pose)
        {
            if (pose == null)
            {
                IsRaisingLeftHand = false;
                IsRaisingRightHand = false;
                return;
            }

            IsRaisingLeftHand = IsRaisingHand(pose.LeftWrist(), pose.LeftShoulder());
            IsRaisingRightHand = IsRaisingHand(pose.RightWrist(), pose.RightShoulder());
        }

        public bool Check(SetupCheckType checkType)
        {
            return checkType switch
            {
                SetupCheckType.GoodPosition => SetupIssue == SetupIssueType.None,
                SetupCheckType.RaisingHand => IsRaisingLeftHand || IsRaisingRightHand,
                _ => throw new ArgumentOutOfRangeException(nameof(checkType), checkType, null)
            };
        }

        #endregion

        #region Helper

        bool IsRaisingHand(PoseNode wrist, PoseNode shoulder)
        {
            if (!wrist.isDetected || !shoulder.isDetected)
            {
                return false;
            }

            return wrist.y > shoulder.y;
        }

        #endregion
    }

    public enum SetupStateType
    {
        Preparing = 0,
        WaitingForGoodPlayerPosition = 1,
        WaitingForRaisingHand = 2,
        Playing = 3
    }

    public struct SetupSummary
    {
        public SetupStateType SetupStateType;
        public SetupIssueType CurrentSetupIssue;
        public bool IsStateChanged;

        public float GoodPositionProgress;
        public float RaiseHandProgress;
        public float NoPlayerDuration;

        public static SetupSummary CreateDummy()
        {
            return new SetupSummary
            {
                SetupStateType = SetupStateType.Preparing,
                CurrentSetupIssue = SetupIssueType.None,
            };
        }
    }

    public class OnePlayerSetupStateTracker : MonoBehaviour
    {
        const float state0GoodPositionRatioThreshold = 0.9f;
        const float state0GoodPositionCheckDuration = 0.3f;
        const float state1RaiseHandRatioThreshold = 0.7f;
        const float state1RaiseHandCheckDuration = 1.5f;
        const float state1GoodPositionRatioThreshold = 0.3f;
        const float state2NoPlayerDurationThreshold = 10;
        const float historyDurationInSeconds = 4f;

        [SerializeField] SetupDetector setupDetectorPrefab = null!;

        SetupDetector setupDetector = null!;
        History<SetupHistoryItem> setupHistory = null!;

        SetupStateType curState = SetupStateType.Preparing;

        int playerIndex;
        BodyPose? lastPose;
        float lastPlayerIsSeenTimestamp;
        float lastStateStartTime;
        bool isTracking;

        public event UnityAction<SetupSummary>? Updated;

        #region Public

        public void Initialize(
            int aPlayerIndex,
            BodyPoseDetectionManager aBodyPoseDetectionManager
        )
        {
            playerIndex = aPlayerIndex;

            setupHistory = new History<SetupHistoryItem>(historyDurationInSeconds);

            setupDetector = Instantiate(setupDetectorPrefab, transform);
            InitializeSetupDetector(setupDetector, aBodyPoseDetectionManager, playerIndex);
            setupDetector.captureDetection += SetupDetectorOnCaptureDetection;
            UpdateSetupDetectorConfigBasedOnState(curState);

            aBodyPoseDetectionManager.processed.captureAspectNormalizedDetection += ProcessedOnCaptureAspectNormalizedDetection;
        }

        public void SetIsTracking(bool value)
        {
            isTracking = value;
        }

        #endregion

        #region Setup Detector

        static void InitializeSetupDetector(
            SetupDetector setupDetector,
            BodyPoseDetectionManager bodyPoseDetectionManager,
            int playerIndex,
            float chestStrictLooseHalfMarginInches = 1,
            float chestToTopMinInches = 12,
            float chestToBottomMinInches = 18,
            float chestToLeftMinInches = 12,
            float chestToRightMinInches = 12,
            float chestXToCenterMaxInches = 10,
            float issueEvaluationTimeWindow = 0.5f,
            float issueMinRequiredDataTime = 0.2f,
            float issueEntryStateRatio = 0.8f,
            float issueCancelStateRatio = 0.4f,
            float distanceRatioStrictLooseHalfMarginForDistanceIssue = 0.03f,
            float frameHeightMinInches = 40,
            float frameHeightMaxInches = 240,
            float processFrameHeightMinInches = 40,
            float processFrameHeightMaxInches = 100
        )
        {
            setupDetector.bodyDetector = bodyPoseDetectionManager;
            setupDetector.playerIndex = playerIndex;
            setupDetector.chestStrictLooseHalfMarginInches = chestStrictLooseHalfMarginInches;
            setupDetector.chestToTopMinInches = chestToTopMinInches;
            setupDetector.chestToBottomMinInches = chestToBottomMinInches;
            setupDetector.chestToLeftMinInches = chestToLeftMinInches;
            setupDetector.chestToRightMinInches = chestToRightMinInches;
            setupDetector.chestXToCenterMaxInches = chestXToCenterMaxInches;
            setupDetector.issueEvaluationTimeWindow = issueEvaluationTimeWindow;
            setupDetector.issueMinRequiredDataTime = issueMinRequiredDataTime;
            setupDetector.issueEntryStateRatio = issueEntryStateRatio;
            setupDetector.issueCancelStateRatio = issueCancelStateRatio;
            setupDetector.distanceRatioStrictLooseHalfMarginForDistanceIssue = distanceRatioStrictLooseHalfMarginForDistanceIssue;
            setupDetector.frameHeightMinInches = frameHeightMinInches;
            setupDetector.frameHeightMaxInches = frameHeightMaxInches;
            setupDetector.processFrameHeightMinInches = processFrameHeightMinInches;
            setupDetector.processFrameHeightMaxInches = processFrameHeightMaxInches;
            setupDetector.centerStrategy = SetupCenterStrategy.ProcessFrame;
            setupDetector.Initialize();
        }

        void UpdateSetupDetectorConfigBasedOnState(SetupStateType state)
        {
            const int chestXToCenterMaxInchesForStrictCase = 10;
            const int chestXToCenterMaxInchesForLooseCase = 34;
            setupDetector.chestXToCenterMaxInches = state switch
            {
                SetupStateType.Preparing => chestXToCenterMaxInchesForStrictCase,
                SetupStateType.WaitingForGoodPlayerPosition => chestXToCenterMaxInchesForStrictCase,
                SetupStateType.WaitingForRaisingHand => chestXToCenterMaxInchesForStrictCase,
                SetupStateType.Playing => chestXToCenterMaxInchesForLooseCase,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        void SetupDetectorOnCaptureDetection(SetupDetection detection)
        {
            if (!isTracking)
            {
                return;
            }

            var setupState = new SetupHistoryItem
            {
                SetupIssue = detection.currentIssue
            };

            setupState.UpdateStateWithPose(lastPose);

            setupHistory.UpdateCurrentFrameTime(Time.fixedTime);
            setupHistory.Add(setupState, Time.fixedTime);

            var summary = new SetupSummary
            {
                CurrentSetupIssue = detection.currentIssue
            };

            if (detection.hasEnoughData && detection.currentIssue != SetupIssueType.NoPose)
            {
                lastPlayerIsSeenTimestamp = Time.fixedTime;
            }

            switch (curState)
            {
                case SetupStateType.Preparing:
                {
                    if (detection.hasEnoughData)
                    {
                        summary.IsStateChanged = true;
                        ChangeState(SetupStateType.WaitingForGoodPlayerPosition);
                    }

                    break;
                }
                case SetupStateType.WaitingForGoodPlayerPosition:
                {
                    // Check forward: good pose.
                    var goodPositionRatio = CheckRatio(SetupCheckType.GoodPosition, state0GoodPositionCheckDuration);
                    summary.GoodPositionProgress = goodPositionRatio / state0GoodPositionRatioThreshold;
                    if (goodPositionRatio > state0GoodPositionRatioThreshold)
                    {
                        summary.IsStateChanged = true;
                        ChangeState(SetupStateType.WaitingForRaisingHand);
                    }

                    break;
                }
                case SetupStateType.WaitingForRaisingHand:
                {
                    // Check forward: raise hand.
                    var raiseHandRatio = CheckRatio(SetupCheckType.RaisingHand, state1RaiseHandCheckDuration, lastStateStartTime);
                    summary.RaiseHandProgress = raiseHandRatio / state1RaiseHandRatioThreshold;
                    if (raiseHandRatio > state1RaiseHandRatioThreshold)
                    {
                        summary.IsStateChanged = true;
                        ChangeState(SetupStateType.Playing);
                    }
                    else
                    {
                        // Check backward: bad pose.
                        var goodPositionRatio = CheckRatio(SetupCheckType.GoodPosition, 1);
                        if (goodPositionRatio < state1GoodPositionRatioThreshold)
                        {
                            summary.IsStateChanged = true;
                            ChangeState(SetupStateType.WaitingForGoodPlayerPosition);
                        }
                    }

                    break;
                }
                case SetupStateType.Playing:
                    // Check backward: no pose.
                    var noPlayerDuration = Time.fixedTime - Math.Max(lastStateStartTime, lastPlayerIsSeenTimestamp);
                    summary.NoPlayerDuration = noPlayerDuration;
                    if (noPlayerDuration > state2NoPlayerDurationThreshold)
                    {
                        summary.IsStateChanged = true;
                        ChangeState(SetupStateType.WaitingForGoodPlayerPosition);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            summary.SetupStateType = curState;
            Updated?.Invoke(summary);
        }

        #endregion

        #region Pose Detection

        void ProcessedOnCaptureAspectNormalizedDetection(BodyPoseDetectionResult detectionResult)
        {
            if (!isTracking)
            {
                return;
            }

            var playerPose = detectionResult.processed.GetPlayerPose(playerIndex);
            lastPose = playerPose?.bodyPose;
        }

        #endregion

        #region State

        float CheckRatio(SetupCheckType checkType, float duration, float startTime = 0)
        {
            if (setupHistory.items.Count == 0)
            {
                return 0;
            }

            var totalYesTime = 0f;
            var minTimestamp = Math.Max(Time.fixedTime - duration, startTime);
            for (var i = 0; i < setupHistory.items.Count - 1; i++)
            {
                TimedItem<SetupHistoryItem> curItem = setupHistory.items[i];
                TimedItem<SetupHistoryItem> prevItem = setupHistory.items[i + 1];

                if (curItem.frameTime < minTimestamp)
                {
                    // Too old
                    break;
                }

                if (curItem.item.Check(checkType))
                {
                    totalYesTime += (float)curItem.frameTime - Math.Max(minTimestamp, (float)prevItem.frameTime);
                }
            }

            return totalYesTime / duration;
        }

        void ChangeState(SetupStateType stateType)
        {
            if (curState == stateType)
            {
                return;
            }

            curState = stateType;
            lastStateStartTime = Time.fixedTime;

            UpdateSetupDetectorConfigBasedOnState(curState);
        }

        #endregion
    }
}
