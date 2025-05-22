using UnityEngine;

namespace Match3
{
    public class GamePiece : MonoBehaviour
    {
        public int score;

        private int _x;
        private int _y;
        private PieceType _type;
        private GridController _GridController;

        private MovablePiece _movableComponent;
        private ColorPiece _colorComponent;
        private RemovablePiece _clearableComponent;

        public int X
        {
            get => _x;
            set
            {
                if (IsMovable())
                {
                    _x = value;
                }
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                if (IsMovable())
                {
                    _y = value;
                }
            }
        }

        public PieceType Type => _type;
        public GridController GridControllerRef => _GridController;
        public MovablePiece MovableComponent => _movableComponent;
        public ColorPiece ColorComponent => _colorComponent;
        public RemovablePiece ClearableComponent => _clearableComponent;

        private void Awake()
        {
            _movableComponent = GetComponent<MovablePiece>();
            _colorComponent = GetComponent<ColorPiece>();
            _clearableComponent = GetComponent<RemovablePiece>();
        }

        public void Init(int x, int y, GridController GridController, PieceType type)
        {
            _x = x;
            _y = y;
            _GridController = GridController;
            _type = type;
        }

        private void OnMouseEnter() => _GridController?.EnterPiece(this);

        private void OnMouseDown() => _GridController?.PressPiece(this);

        private void OnMouseUp() => _GridController?.ReleasePiece();

        public bool IsMovable() => _movableComponent != null;
        public bool IsColored() => _colorComponent != null;
        public bool IsClearable() => _clearableComponent != null;
    }
}