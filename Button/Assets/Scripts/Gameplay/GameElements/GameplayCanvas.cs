using TMPro;
using UnityEngine;

namespace Gameplay.GameElements
{
    public class GameplayCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI questionHint;
        [SerializeField] private TextMeshProUGUI endGameScore;
        [SerializeField] private TextMeshProUGUI remainTime;
        [SerializeField] private TextMeshProUGUI getReadyText;
        [SerializeField] private TextMeshProUGUI countDown;

        private void SetActiveScoreText(bool active)
        {
            scoreText.gameObject.SetActive(active);
        }
        public void SetScoreText(string text)
        {
            scoreText.text = text;
        }

        private void SetActiveQuestionHint(bool active)
        {
            questionHint.gameObject.SetActive(active);
        }
        public void SetQuestionHint(string text)
        {
            questionHint.text = text;
        }

        private void SetActiveEndGameScore(bool active)
        {
            endGameScore.gameObject.SetActive(active);
        }
        public void SetEndGameScore(string text)
        {
            endGameScore.text = text;
        }

        private void SetActiveRemainTime(bool active)
        {
            remainTime.gameObject.SetActive(active);
        }
        public void SetRemainTime(string text)
        {
            remainTime.text = text;
        }

        private void SetActiveGetReadyText(bool active)
        {
            getReadyText.gameObject.SetActive(active);
        }

        private void SetActiveCountDown(bool active)
        {
            countDown.gameObject.SetActive(active);
        }
        public void SetCountDown(string text)
        {
            countDown.text = text;
        }

        public void CountDownSetUp()
        {
            SetActiveCountDown(true);
            SetActiveGetReadyText(true);
            SetActiveEndGameScore(false);
            SetActiveQuestionHint(false);
            SetActiveRemainTime(false);
            SetActiveScoreText(false);
        }
        public void StartGameSetUp()
        {
            SetActiveCountDown(false);
            SetActiveGetReadyText(false);
            SetActiveEndGameScore(false);
            SetActiveQuestionHint(true);
            SetActiveRemainTime(true);
            SetActiveScoreText(true);
        }

        public void EndGameSetUp()
        {
            SetActiveCountDown(false);
            SetActiveGetReadyText(false);
            SetActiveEndGameScore(true);
            SetActiveQuestionHint(false);
            SetActiveRemainTime(false);
            SetActiveScoreText(false);
        }
    }
}
