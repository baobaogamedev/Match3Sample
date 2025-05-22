using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class GridController : MonoBehaviour
    {
        [System.Serializable]
        public struct PiecePrefab
        {
            public PieceType type;
            public GameObject prefab;
        };

        [System.Serializable]
        public struct PiecePosition
        {
            public PieceType type;
            public int x;
            public int y;
        };

        public int xDim;
        public int yDim;
        public float fillTime;

        public Level level;

        public PiecePrefab[] piecePrefabs;
        public GameObject backgroundPrefab;

        public PiecePosition[] initialPieces;

        private Dictionary<PieceType, GameObject> _piecePrefabDict;

        private GamePiece[,] _pieces;

        private bool _inverse;

        private GamePiece _pressedPiece;
        private GamePiece _enteredPiece;

        private bool _gameOver;
        public bool IsFilling { get; private set; }

        private void Awake()
        {
            // Populate dictionary with piece prefab types
            _piecePrefabDict = new Dictionary<PieceType, GameObject>();
            foreach (var piece in piecePrefabs)
            {
                if (!_piecePrefabDict.ContainsKey(piece.type))
                {
                    _piecePrefabDict.Add(piece.type, piece.prefab);
                }
            }

            // Instantiate backgrounds
            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    var background = Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                    background.transform.parent = transform;
                }
            }

            // Instantiate pieces array
            _pieces = new GamePiece[xDim, yDim];

            // Spawn initial pieces
            foreach (var pieceData in initialPieces)
            {
                if (pieceData.x >= 0 && pieceData.x < xDim && pieceData.y >= 0 && pieceData.y < yDim)
                {
                    SpawnNewPiece(pieceData.x, pieceData.y, pieceData.type);
                }
            }

            // Fill empty spots with Empty pieces
            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if (_pieces[x, y] == null)
                    {
                        SpawnNewPiece(x, y, PieceType.Empty);
                    }
                }
            }

            StartCoroutine(Fill());
        }

        private IEnumerator Fill()
        {
            IsFilling = true;
            bool needsRefill = true;

            while (needsRefill)
            {
                yield return new WaitForSeconds(fillTime);
                while (FillStep())
                {
                    _inverse = !_inverse;
                    yield return new WaitForSeconds(fillTime);
                }

                needsRefill = ClearAllValidMatches();
            }

            IsFilling = false;
        }

        /// <summary>
        /// One pass through all grid cells, moving them down one grid, if possible.
        /// </summary>
        /// <returns>True if at least one piece was moved down.</returns>
        private bool FillStep()
        {
            bool movedPiece = false;

            // Move pieces down starting from second to last row upwards
            for (int y = yDim - 2; y >= 0; y--)
            {
                for (int loopX = 0; loopX < xDim; loopX++)
                {
                    int x = _inverse ? xDim - 1 - loopX : loopX;

                    GamePiece piece = _pieces[x, y];
                    if (!piece.IsMovable()) continue;

                    GamePiece pieceBelow = _pieces[x, y + 1];
                    if (pieceBelow.Type == PieceType.Empty)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, fillTime);
                        _pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.Empty);
                        movedPiece = true;
                    }
                    else
                    {
                        // Check diagonals if piece cannot move directly down
                        for (int diag = -1; diag <= 1; diag += 2) // Only -1 and 1
                        {
                            int diagX = _inverse ? x - diag : x + diag;
                            if (diagX < 0 || diagX >= xDim) continue;

                            GamePiece diagonalPiece = _pieces[diagX, y + 1];
                            if (diagonalPiece.Type != PieceType.Empty) continue;

                            bool hasPieceAbove = true;
                            for (int aboveY = y; aboveY >= 0; aboveY--)
                            {
                                GamePiece pieceAbove = _pieces[diagX, aboveY];
                                if (pieceAbove.IsMovable()) break;
                                if (pieceAbove.Type != PieceType.Empty)
                                {
                                    hasPieceAbove = false;
                                    break;
                                }
                            }
                            if (hasPieceAbove) continue;

                            Destroy(diagonalPiece.gameObject);
                            piece.MovableComponent.Move(diagX, y + 1, fillTime);
                            _pieces[diagX, y + 1] = piece;
                            SpawnNewPiece(x, y, PieceType.Empty);
                            movedPiece = true;
                            break;
                        }
                    }
                }
            }

            // Fill the top row with new normal pieces where empty
            for (int x = 0; x < xDim; x++)
            {
                if (_pieces[x, 0].Type == PieceType.Empty)
                {
                    Destroy(_pieces[x, 0].gameObject);

                    GameObject newPieceObj = Instantiate(_piecePrefabDict[PieceType.Normal], GetWorldPosition(x, -1), Quaternion.identity, transform);
                    GamePiece newPiece = newPieceObj.GetComponent<GamePiece>();

                    newPiece.Init(x, -1, this, PieceType.Normal);
                    newPiece.MovableComponent.Move(x, 0, fillTime);
                    newPiece.ColorComponent.SetColor((ColorType)Random.Range(0, newPiece.ColorComponent.NumColors));

                    _pieces[x, 0] = newPiece;
                    movedPiece = true;
                }
            }

            return movedPiece;
        }

        public Vector2 GetWorldPosition(int x, int y)
        {
            float worldX = transform.position.x - xDim / 2f + x;
            float worldY = transform.position.y + yDim / 2f - y;
            return new Vector2(worldX, worldY);
        }

        private GamePiece SpawnNewPiece(int x, int y, PieceType type)
        {
            var pieceGO = Instantiate(_piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity, transform);
            var gamePiece = pieceGO.GetComponent<GamePiece>();

            _pieces[x, y] = gamePiece;
            gamePiece.Init(x, y, this, type);

            return gamePiece;
        }

        private static bool IsAdjacent(GamePiece a, GamePiece b)
        {
            bool sameColumn = a.X == b.X && Mathf.Abs(a.Y - b.Y) == 1;
            bool sameRow = a.Y == b.Y && Mathf.Abs(a.X - b.X) == 1;
            return sameColumn || sameRow;
        }

        private void SwapPieces(GamePiece piece1, GamePiece piece2)
        {
            if (_gameOver || !piece1.IsMovable() || !piece2.IsMovable())
                return;

            // Swap references in the array first
            _pieces[piece1.X, piece1.Y] = piece2;
            _pieces[piece2.X, piece2.Y] = piece1;

            bool piece1Matches = GetMatch(piece1, piece2.X, piece2.Y) != null;
            bool piece2Matches = GetMatch(piece2, piece1.X, piece1.Y) != null;
            bool specialSwap = piece1.Type == PieceType.Rainbow || piece2.Type == PieceType.Rainbow;

            if (piece1Matches || piece2Matches || specialSwap)
            {
                int piece1OldX = piece1.X;
                int piece1OldY = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1OldX, piece1OldY, fillTime);

                HandleRainbowClear(piece1, piece2);
                HandleRainbowClear(piece2, piece1);

                ClearAllValidMatches();

                ClearSpecialPieces(piece1);
                ClearSpecialPieces(piece2);

                _pressedPiece = null;
                _enteredPiece = null;

                StartCoroutine(Fill());
                level.OnMove();
            }
            else
            {
                // Swap back if no matches or special conditions
                _pieces[piece1.X, piece1.Y] = piece1;
                _pieces[piece2.X, piece2.Y] = piece2;
            }
        }

        private void HandleRainbowClear(GamePiece rainbowPiece, GamePiece otherPiece)
        {
            if (rainbowPiece.Type == PieceType.Rainbow && rainbowPiece.IsClearable() && otherPiece.IsColored())
            {
                var clearColor = rainbowPiece.GetComponent<ClearColorPiece>();
                if (clearColor != null)
                {
                    clearColor.Color = otherPiece.ColorComponent.Color;
                }
                ClearPiece(rainbowPiece.X, rainbowPiece.Y);
            }
        }

        private void ClearSpecialPieces(GamePiece piece)
        {
            if (piece.Type == PieceType.RowClear || piece.Type == PieceType.ColumnClear)
            {
                ClearPiece(piece.X, piece.Y);
            }
        }

        public void PressPiece(GamePiece piece) => _pressedPiece = piece;

        public void EnterPiece(GamePiece piece) => _enteredPiece = piece;

        public void ReleasePiece()
        {
            if (_pressedPiece != null && _enteredPiece != null && IsAdjacent(_pressedPiece, _enteredPiece))
            {
                SwapPieces(_pressedPiece, _enteredPiece);
            }
        }
        private bool ClearAllValidMatches()
        {
            bool needsRefill = false;

            for (int y = 0; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    if (!_pieces[x, y].IsClearable())
                        continue;

                    List<GamePiece> match = GetMatch(_pieces[x, y], x, y);
                    if (match == null)
                        continue;

                    PieceType specialPieceType = PieceType.Count;
                    GamePiece randomPiece = match[Random.Range(0, match.Count)];
                    int specialPieceX = randomPiece.X;
                    int specialPieceY = randomPiece.Y;

                    // Determine special piece type
                    if (match.Count >= 5)
                    {
                        specialPieceType = PieceType.Rainbow;
                    }
                    else if (match.Count == 4)
                    {
                        if (_pressedPiece == null || _enteredPiece == null)
                        {
                            specialPieceType = (PieceType)Random.Range((int)PieceType.RowClear, (int)PieceType.ColumnClear + 1);
                        }
                        else
                        {
                            specialPieceType = (_pressedPiece.Y == _enteredPiece.Y) ? PieceType.RowClear : PieceType.ColumnClear;
                        }
                    }

                    foreach (var gamePiece in match)
                    {
                        if (ClearPiece(gamePiece.X, gamePiece.Y))
                        {
                            needsRefill = true;

                            if (gamePiece == _pressedPiece || gamePiece == _enteredPiece)
                            {
                                specialPieceX = gamePiece.X;
                                specialPieceY = gamePiece.Y;
                            }
                        }
                    }

                    if (specialPieceType == PieceType.Count)
                        continue;

                    Destroy(_pieces[specialPieceX, specialPieceY]);
                    GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                    if ((specialPieceType == PieceType.RowClear || specialPieceType == PieceType.ColumnClear)
                        && newPiece.IsColored() && match[0].IsColored())
                    {
                        newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                    }
                    else if (specialPieceType == PieceType.Rainbow && newPiece.IsColored())
                    {
                        newPiece.ColorComponent.SetColor(ColorType.Any);
                    }
                }
            }

            return needsRefill;
        }

        private List<GamePiece> GetMatch(GamePiece piece, int x, int y)
        {
            if (!piece.IsColored())
                return null;

            var color = piece.ColorComponent.Color;

            // Check horizontal line
            var horizontalMatches = GetMatchingLine(x, y, color, true);

            // If horizontal match has 3 or more, check for L or T shape by scanning vertical around horizontal pieces
            if (horizontalMatches.Count >= 3)
            {
                var verticalMatches = GetLShapeMatches(horizontalMatches, color, true);
                if (verticalMatches.Count >= 3)
                    return verticalMatches;
                return horizontalMatches;
            }

            // Check vertical line
            var verticalMatchesLine = GetMatchingLine(x, y, color, false);

            // If vertical match has 3 or more, check for L or T shape by scanning horizontal around vertical pieces
            if (verticalMatchesLine.Count >= 3)
            {
                var horizontalMatchesAroundVertical = GetLShapeMatches(verticalMatchesLine, color, false);
                if (horizontalMatchesAroundVertical.Count >= 3)
                    return horizontalMatchesAroundVertical;
                return verticalMatchesLine;
            }

            return null;
        }

        // Helper: Get line of matching pieces in one direction (horizontal if isHorizontal true, vertical otherwise)
        private List<GamePiece> GetMatchingLine(int startX, int startY, ColorType color, bool isHorizontal)
        {
            var matches = new List<GamePiece>();
            matches.Add(_pieces[startX, startY]);

            // Directions: left/up = 0, right/down = 1
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int offset = 1; offset < (isHorizontal ? xDim : yDim); offset++)
                {
                    int checkX = startX;
                    int checkY = startY;

                    if (isHorizontal)
                        checkX = (dir == 0) ? startX - offset : startX + offset;
                    else
                        checkY = (dir == 0) ? startY - offset : startY + offset;

                    if (checkX < 0 || checkX >= xDim || checkY < 0 || checkY >= yDim)
                        break;

                    var currentPiece = _pieces[checkX, checkY];
                    if (currentPiece.IsColored() && currentPiece.ColorComponent.Color == color)
                        matches.Add(currentPiece);
                    else
                        break;
                }
            }

            return matches;
        }

        // Helper: From base line matches, try to find L or T shape by scanning perpendicular direction
        private List<GamePiece> GetLShapeMatches(List<GamePiece> baseLineMatches, ColorType color, bool baseIsHorizontal)
        {
            var matches = new List<GamePiece>(baseLineMatches);
            var secondaryMatches = new List<GamePiece>();

            foreach (var piece in baseLineMatches)
            {
                // Directions: up/down if baseIsHorizontal, left/right if vertical
                for (int dir = 0; dir <= 1; dir++)
                {
                    for (int offset = 1; offset < (baseIsHorizontal ? yDim : xDim); offset++)
                    {
                        int checkX = piece.X;
                        int checkY = piece.Y;

                        if (baseIsHorizontal)
                            checkY = (dir == 0) ? piece.Y - offset : piece.Y + offset;
                        else
                            checkX = (dir == 0) ? piece.X - offset : piece.X + offset;

                        if (checkX < 0 || checkX >= xDim || checkY < 0 || checkY >= yDim)
                            break;

                        var currentPiece = _pieces[checkX, checkY];
                        if (currentPiece.IsColored() && currentPiece.ColorComponent.Color == color)
                            secondaryMatches.Add(currentPiece);
                        else
                            break;
                    }
                }

                if (secondaryMatches.Count >= 2)
                {
                    matches.AddRange(secondaryMatches);
                    break;
                }
                secondaryMatches.Clear();
            }

            return matches.Count >= 3 ? matches : baseLineMatches;
        }

        private bool ClearPiece(int x, int y)
        {
            var piece = _pieces[x, y];
            if (!piece.IsClearable() || piece.ClearableComponent.IsBeingCleared)
                return false;

            piece.ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.Empty);

            ClearObstacles(x, y);

            return true;
        }

        private void ClearObstacles(int x, int y)
        {
            // Check horizontal neighbors
            for (int adjX = x - 1; adjX <= x + 1; adjX++)
            {
                if (adjX == x || adjX < 0 || adjX >= xDim) continue;

                var neighbor = _pieces[adjX, y];
                if (neighbor.Type == PieceType.Bubble && neighbor.IsClearable())
                {
                    neighbor.ClearableComponent.Clear();
                    SpawnNewPiece(adjX, y, PieceType.Empty);
                }
            }

            // Check vertical neighbors
            for (int adjY = y - 1; adjY <= y + 1; adjY++)
            {
                if (adjY == y || adjY < 0 || adjY >= yDim) continue;

                var neighbor = _pieces[x, adjY];
                if (neighbor.Type == PieceType.Bubble && neighbor.IsClearable())
                {
                    neighbor.ClearableComponent.Clear();
                    SpawnNewPiece(x, adjY, PieceType.Empty);
                }
            }
        }

        public void ClearRow(int row)
        {
            for (int x = 0; x < xDim; x++)
                ClearPiece(x, row);
        }

        public void ClearColumn(int column)
        {
            for (int y = 0; y < yDim; y++)
                ClearPiece(column, y);
        }

        public void ClearColor(ColorType color)
        {
            bool clearAny = color == ColorType.Any;
            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    var piece = _pieces[x, y];
                    if (clearAny || (piece.IsColored() && piece.ColorComponent.Color == color))
                    {
                        ClearPiece(x, y);
                    }
                }
            }
        }

        public void GameOverController() => _gameOver = true;

        public List<GamePiece> GetPiecesOfType(PieceType type)
        {
            var piecesOfType = new List<GamePiece>();
            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if (_pieces[x, y].Type == type)
                        piecesOfType.Add(_pieces[x, y]);
                }
            }
            return piecesOfType;
        }


    }
}
