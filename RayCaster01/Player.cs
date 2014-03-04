using System;
using System.Linq;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace RayCaster01
{
    public class Player : GameObject
    {
        private float _rotationSpeed;
        private float _walkingSpeed;
        private Vector2 _position;
        private Vector2 _direction;

        public float RotationSpeed
        {
            get { return _rotationSpeed; }
        }

        public float WalkingSpeed
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

            // Initial Player Position 
            _position.X = 12;
            _position.Y = 12;

            // Initial Player Direction 
            _direction.X = -1;
            _direction.Y = 0;

            //speed modifiers
            _walkingSpeed = 0.05f; 
            _rotationSpeed = 0.05f; 
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var pressedKeys = Game.Keyboard.GetPressedKeys();

            var stepX = _direction.X * _walkingSpeed;
            var stepY = _direction.Y * _walkingSpeed;
            var step = Vector2.Multiply(_direction, _walkingSpeed);

            // Move Forward 
            if (pressedKeys.Contains(Keys.Up))
            {
                if (Game.Map.GetBlock(_position.X + stepX, _position.Y) == 0)
                    _position.X += stepX;

                if (Game.Map.GetBlock(_position.X, _position.Y + stepY) == 0)
                    _position.Y += stepY;
            }

            // Move Backwards 
            if (pressedKeys.Contains(Keys.Down))
            {
                if (Game.Map.GetBlock(_position.X - stepX, _position.Y) == 0)
                    _position.X -= stepX;

                if (Game.Map.GetBlock(_position.X, (_position.Y - stepY)) == 0)
                    _position.Y -= stepY;
            }

            // Rotate Left 
            if (pressedKeys.Contains(Keys.Left))
            {
                var rotated = _direction.Rotate(-_rotationSpeed);
                _direction.X = rotated.X;
                _direction.Y = rotated.Y;

                Game.Scene.RotateCameraPlane(-_rotationSpeed);
            }

            // Rotate Right 
            if (pressedKeys.Contains(Keys.Right))
            {
                var rotated = _direction.Rotate(_rotationSpeed);
                _direction.X = rotated.X;
                _direction.Y = rotated.Y;

                Game.Scene.RotateCameraPlane(_rotationSpeed);
            }


           
    

        }
    }
}