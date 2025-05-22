namespace Match3
{
    internal class RemoveColumnPiece : RemovablePiece
    {
        public bool IsRow { get; set; }

        public override void Clear()
        {
            base.Clear();

            if (piece?.GridControllerRef == null)
            {
                return; // Safeguard against null references
            }

            if (IsRow)
            {
                piece.GridControllerRef.ClearRow(piece.Y);
            }
            else
            {
                piece.GridControllerRef.ClearColumn(piece.X);
            }
        }
    }
}
