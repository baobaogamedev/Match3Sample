namespace Match3
{
    public class ClearColorPiece : RemovablePiece
    {
        public ColorType Color { get; set; }

        public override void Clear()
        {
            base.Clear();

            if (piece?.GridControllerRef != null)
            {
                piece.GridControllerRef.ClearColor(Color);
            }
        }
    }
}
