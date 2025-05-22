using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Match3
{
    public class DisplayController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Level level;
        [SerializeField] private GameOverController gameOver;

        [Header("UI Elements")]
        [SerializeField] private Text remainingText;
        [SerializeField] private Text remainingSubText;
        [SerializeField] private Text targetText;
        [SerializeField] private Text targetSubtext;
        [SerializeField] private Text scoreText;
        [SerializeField] private Image[] stars;

        private int currentStarIndex = 0;

        private void Start()
        {
            UpdateStarDisplay(currentStarIndex);
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();

            int visibleStar = CalculateStarIndex(score);
            UpdateStarDisplay(visibleStar);
            currentStarIndex = visibleStar;
        }

        public void SetTarget(int target)
        {
            targetText.text = target.ToString();
        }

        public void SetRemaining(int remaining)
        {
            remainingText.text = remaining.ToString();
        }

        public void SetRemaining(string remaining)
        {
            remainingText.text = remaining;
        }

        public void SetLevelType(LevelType type)
        {
            switch (type)
            {
                case LevelType.Moves:
                    remainingSubText.text = "moves remaining";
                    targetSubtext.text = "target score";
                    break;
                case LevelType.Obstacle:
                    remainingSubText.text = "moves remaining";
                    targetSubtext.text = "bubbles remaining";
                    break;
                case LevelType.Timer:
                    remainingSubText.text = "time remaining";
                    targetSubtext.text = "target score";
                    break;
            }
        }

        public void OnGameWin(int score)
        {
            gameOver.ShowWin(score, currentStarIndex);

            string sceneName = SceneManager.GetActiveScene().name;
            int bestStars = PlayerPrefs.GetInt(sceneName, 0);

            if (currentStarIndex > bestStars)
            {
                PlayerPrefs.SetInt(sceneName, currentStarIndex);
            }
        }

        public void OnGameLose()
        {
            gameOver.ShowLose();
        }

        private int CalculateStarIndex(int score)
        {
            if (score >= level.score3Star)
                return 3;
            if (score >= level.score2Star)
                return 2;
            if (score >= level.score1Star)
                return 1;
            return 0;
        }

        private void UpdateStarDisplay(int activeStarIndex)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].enabled = (i == activeStarIndex);
            }
        }
    }
}
