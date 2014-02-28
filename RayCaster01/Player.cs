using SharpDX.Toolkit;

namespace RayCaster01
{
    public class Player : GameObject
    {
        private double _x;
        private double _y;
        private double _dirX; 
        private double _dirY; 
        
        public override void Initialize(IGame game)
        {
            base.Initialize(game);

            _x = 22;
            _y = 12;
            _dirX = -1;
            _dirY = 0;
        }
    }
}