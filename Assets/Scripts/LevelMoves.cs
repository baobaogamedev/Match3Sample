namespace Match3
{
    public class LevelMoves : Level
    {
        public int numMoves;
        public int targetScore;

        private int _movesUsed;

        private void Start()
        {
            type = LevelType.Moves;

            display.SetLevelType(type);
            display.SetScore(currentScore);
            display.SetTarget(targetScore);
            display.SetRemaining(RemainingMoves);
        }

        public override void OnMove()
        {
            _movesUsed++;
            int remaining = RemainingMoves;
            display.SetRemaining(remaining);

            if (remaining > 0) return;

            if (currentScore >= targetScore)
            {
                GameWin();
            }
            else
            {
                GameLose();
            }
        }

        private int RemainingMoves => numMoves - _movesUsed;
    }
}