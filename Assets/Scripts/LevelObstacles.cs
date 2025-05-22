using System.Linq;

namespace Match3
{
    public class LevelObstacles : Level
    {
        public int numMoves;
        public PieceType[] obstacleTypes;

        private const int ScorePerPieceCleared = 1000;

        private int _movesUsed;
        private int _numObstaclesLeft;

        private void Start()
        {
            type = LevelType.Obstacle;

            foreach (var obstacleType in obstacleTypes)
            {
                _numObstaclesLeft += GridController.GetPiecesOfType(obstacleType).Count;
            }

            display.SetLevelType(type);
            display.SetScore(currentScore);
            display.SetTarget(_numObstaclesLeft);
            display.SetRemaining(numMoves);
        }

        public override void OnMove()
        {
            _movesUsed++;
            int remainingMoves = numMoves - _movesUsed;
            display.SetRemaining(remainingMoves);

            if (remainingMoves == 0 && _numObstaclesLeft > 0)
            {
                GameLose();
            }
        }

        public override void OnPieceCleared(GamePiece piece)
        {
            base.OnPieceCleared(piece);

            if (!obstacleTypes.Contains(piece.Type)) return;

            _numObstaclesLeft--;
            display.SetTarget(_numObstaclesLeft);

            if (_numObstaclesLeft > 0) return;

            int bonusScore = ScorePerPieceCleared * (numMoves - _movesUsed);
            currentScore += bonusScore;
            display.SetScore(currentScore);
            GameWin();
        }
    }
}
