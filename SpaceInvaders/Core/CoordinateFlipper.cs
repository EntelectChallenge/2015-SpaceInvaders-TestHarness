namespace SpaceInvaders.Core
{
    public class CoordinateFlipper
    {
        private readonly int _midX;
        private readonly int _midY;

        public CoordinateFlipper(int width, int height)
        {
            _midX = width/2;
            _midY = height/2;
        }

        public int CalculateFlippedX(int x)
        {
            return _midX - (x - _midX);
        }

        public int CalculateFlippedY(int y)
        {
            return _midY - (y - _midY);
        }
    }
}