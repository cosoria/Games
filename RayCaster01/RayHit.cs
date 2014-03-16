using SharpDX;

namespace RayCaster01
{
    public class RayHit
    {
        private Vector2 _hit;
        private Color _color;
        private Vector2 _mapCoord;
        private Line _line;


        public float Distance { get; set; }
        public int Side { get; set; }
        public int TextureLine { get; set; }
        public int MapTexture { get; set; }
        public Vector2 MapCoordinates { get { return _mapCoord; } }
        public Vector2 Hit { get { return _hit; } }
        public Color Color { get { return _color; } }
        public Line VerticalLine { get { return _line; }}
        //public Line Texture

        public RayHit()
        {
            Distance = 0.0f;
            TextureLine = 0;
            Side = 0;
            _hit = new Vector2();
            _color = Color.White;
            _line = new Line(0,0,0,0);
        }

        public void SetHit(double x, double y)
        {
            _hit.X = (float)x;
            _hit.Y = (float)y;
        }

        public void SetMapCoordinates(double x, double y)
        {
            _mapCoord.X = (float)x;
            _mapCoord.Y = (float)y;
        }

        public void SetColor(Color color)
        {
            _color = color;
        }


    }
}