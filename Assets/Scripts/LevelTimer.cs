using UnityEngine;

namespace Match3
{
    public class LevelTimer : Level
    {
        public int timeInSeconds;
        public int targetScore;

        private float _elapsedTime;

        private void Start()
        {
            type = LevelType.Timer;
            display.SetLevelType(type);
            display.SetScore(currentScore);
            display.SetTarget(targetScore);
            UpdateHUDTime();
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            int remainingTime = Mathf.Max(timeInSeconds - (int)_elapsedTime, 0);

            UpdateHUDTime(remainingTime);

            if (remainingTime <= 0)
            {
                if (currentScore >= targetScore)
                {
                    GameWin();
                }
                else
                {
                    GameLose();
                }
            }
        }

        private void UpdateHUDTime()
        {
            UpdateHUDTime(timeInSeconds);
        }

        private void UpdateHUDTime(int seconds)
        {
            int minutes = seconds / 60;
            int secs = seconds % 60;
            display.SetRemaining($"{minutes}:{secs:00}");
        }
    }
}
