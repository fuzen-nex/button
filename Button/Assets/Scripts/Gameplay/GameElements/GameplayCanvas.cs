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
        
        public void SetActiveScoreText(bool active)
        {
            scoreText.gameObject.SetActive(active);
        }
        public void SetScoreText(string text)
        {
            scoreText.text = text;
        }
        public void SetActiveQuestionHint(bool active)
        {
            questionHint.gameObject.SetActive(active);
        }
        public void SetQuestionHint(string text)
        {
            questionHint.text = text;
        }
        public void SetActiveEndGameScore(bool active)
        {
            endGameScore.gameObject.SetActive(active);
        }
        public void SetEndGameScore(string text)
        {
            endGameScore.text = text;
        }
        public void SetActiveRemainTime(bool active)
        {
            remainTime.gameObject.SetActive(active);
        }
        public void SetRemainTime(string text)
        {
            remainTime.text = text;
        }
    }
}
