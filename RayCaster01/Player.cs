using System;
using System.Linq;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace RayCaster01
{
    public class Player : GameObject
    {
        private double _rotationSpeed;
        private double _walkingSpeed;
        private Vector2 _position;
        private Vector2 _direction;

        public double RotationSpeed
        {
            get { return _rotationSpeed; }
        }

        public double WalkingSpeed
        {
            get { return _walkingSpeed; }
        }

        public Vector2 Position
        {
            get { return _position; }
        }

        public Vector2 Direction
        {
            get { return _direction; }
        }

        public override void Initialize(IGame game)
        {
            base.Initialize(game);

            _position.X = 12;
            _position.Y = 12;
            _direction.X = 1;
            _direction.Y = 0;
            //speed modifiers
            _walkingSpeed = 0.05; //the constant value is in squares/second
            _rotationSpeed = 0.05; //the constant value is in radians/second
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var pressedKeys = Game.Keyboard.GetPressedKeys();

            // Move Forward 
            if (pressedKeys.Contains(Keys.Up))
            {
                if (Game.Map.World[(int)(_position.X + _direction.X * _walkingSpeed), (int)_position.Y] == 0)
                    _position.X += (float)(_direction.X * _walkingSpeed);

                if (Game.Map.World[(int)_position.X, (int)(_position.Y + _direction.Y * _walkingSpeed)] == 0)
                    _position.Y += (float)(_direction.Y * _walkingSpeed);
            }

            // Move Backwards 
            if (pressedKeys.Contains(Keys.Down))
            {
                if (Game.Map.World[(int)(_position.X - _direction.X * _walkingSpeed), (int)_position.Y] == 0)
                    _position.X -= (float)(_direction.X * _walkingSpeed);

                if (Game.Map.World[(int)_position.X, (int)(_position.Y - _direction.Y * _walkingSpeed)] == 0)
                    _position.Y -= (float)(_direction.Y * _walkingSpeed);
            }

            // Rotate Left 
            if (pressedKeys.Contains(Keys.Left))
            {
                //both camera direction and camera plane must be rotated
                double oldDirX = _direction.X;
                double oldDirY = _direction.Y;

                _direction.X = (float)(oldDirX * Math.Cos(_rotationSpeed) - _direction.Y * Math.Sin(_rotationSpeed));
                _direction.Y = (float)(oldDirX * Math.Sin(_rotationSpeed) + oldDirY * Math.Cos(_rotationSpeed));

                Game.Scene.RotatePlaneLeft();
                //  double oldPlaneX = planeX;
                //  planeX = planeX * cos(rotSpeed) - planeY * sin(rotSpeed);
                //  planeY = oldPlaneX * sin(rotSpeed) + planeY * cos(rotSpeed);
            }

            // Rotate Right 
            if (pressedKeys.Contains(Keys.Right))
            {
                //both camera direction and camera plane must be rotated
                double oldDirX = _direction.X;
                double oldDirY = _direction.Y;

                _direction.X = (float)(oldDirX * Math.Cos(-_rotationSpeed) - _direction.Y * Math.Sin(-_rotationSpeed));
                _direction.Y = (float)(oldDirX * Math.Sin(-_rotationSpeed) + oldDirY * Math.Cos(-_rotationSpeed));

                Game.Scene.RotatePlaneRight();
            }


           
    

        }
    }
}