using SharpDX;

namespace RayCaster01
{
    public class Line
    {
        private Vector2 _start;
        private Vector2 _end;
        private Color _color;

        public Vector2 Start
        {
            get { return _start; }
        }

        public Vector2 End
        {
            get { return _end; }
        }

        public Color Color
        {
            get { return _color; }
        }

        public Line(float x1, float y1, float x2, float y2) 
            : this(x1, y1, x2, y2, Color.White)
        {
        }

        public Line(Vector2 start, Vector2 end)
            : this(start.X, start.Y, end.X, end.Y, Color.White)
        {
        }


        public Line(Vector2 start, Vector2 end, Color color)
            : this(start.X, start.Y, end.X, end.Y, color)
        {
        }

        public Line(float x1, float y1, float x2, float y2, Color color)
        {
            _start = new Vector2(x1, y1);
            _end = new Vector2(x2, y2);
            _color = color;
        }

        public void Set(Vector2 start, Vector2 end, Color color)
        {
            Set(start.X, start.Y, end.X, end.Y, color);
        }

        public void SetStart(float x, float y)
        {
            _start.X = x;
            _start.Y = y;
        }

        public void SetEnd(float x, float y)
        {
            _end.X = x;
            _end.Y = y;
        }

        public void Set(float x1, float y1, float x2, float y2, Color color)
        {
            _start.X = x1;
            _start.Y = y1;
            _end.X = x2;
            _end.Y = y2;
            _color.R = color.R;
            _color.G = color.G;
            _color.B = color.B;
            _color.A = color.A;
        }
    }
}