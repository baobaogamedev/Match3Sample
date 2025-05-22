using System.Collections;
using UnityEngine;

namespace Match3
{
    public class Level : MonoBehaviour
    {
        [Header("References")]
        public GridController GridController;
        public DisplayController display;

        [Header("Star Score Thresholds")]
        public int score1Star;
        public int score2Star;
        public int score3Star;

        protected LevelType type;
        protected int currentScore;

        private bool _didWin;

        private void Start()
        {
            UpdateHUDScore();
        }

        public LevelType Type => type;

        protected virtual void GameWin()
        {
            EndGame(true);
        }

        protected virtual void GameLose()
        {
            EndGame(false);
        }

        private void EndGame(bool didWin)
        {
            GridController.GameOverController();
            _didWin = didWin;
            StartCoroutine(WaitForGridFill());
        }

        public virtual void OnMove()
        {
            // Override in derived classes if needed
        }

        public virtual void OnPieceCleared(GamePiece piece)
        {
            currentScore += piece.score;
            UpdateHUDScore();
        }

        private void UpdateHUDScore()
        {
            display.SetScore(currentScore);
        }

        protected virtual IEnumerator WaitForGridFill()
        {
            yield return new WaitUntil(() => !GridController.IsFilling);

            if (_didWin)
            {
                display.OnGameWin(currentScore);
            }
            else
            {
                display.OnGameLose();
            }
        }
    }
}