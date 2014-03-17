using System;
using System.Linq;
using RayCaster01.Framework;
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
        private Vector2 _strafeDirection;

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
            _position.X = 6.50f;
            _position.Y = 12.50f;

            // Initial Player Direction 
            _direction.X = 0f;
            _direction.Y = 1f;

            
            // Strafe Direction is perpendicular to direction 
            var rotated = _direction.Rotate(-(float)Math.PI / 2);
            _strafeDirection.X = rotated.X;
            _strafeDirection.Y = rotated.Y;

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
            var sizeStep = Vector2.Multiply(_strafeDirection, _walkingSpeed);

            var wallBoundX = stepX > 0 ? 0.2f : -0.2f;
            var wallBoundY = stepY > 0 ? 0.2f : -0.2f;

            // Move Forward 
            if (pressedKeys.Contains(Keys.Up))
            {
                if (Game.Map.GetBlock(_position.X + (stepX + wallBoundX), _position.Y) == 0)
                    _position.X += stepX;

                if (Game.Map.GetBlock(_position.X, _position.Y + (stepY + wallBoundY)) == 0)
                    _position.Y += stepY;
            }

            // Move Backwards 
            if (pressedKeys.Contains(Keys.Down))
            {
                if (Game.Map.GetBlock(_position.X - (stepX + wallBoundX), _position.Y) == 0)
                    _position.X -= stepX;

                if (Game.Map.GetBlock(_position.X, (_position.Y - (stepY + wallBoundY))) == 0)
                    _position.Y -= stepY;
            }

            // Strafe Left 
            if (pressedKeys.Contains(Keys.Z))
            {
                if (Game.Map.GetBlock(_position.X + sizeStep.X, _position.Y) == 0)
                    _position.X += sizeStep.X;

                if (Game.Map.GetBlock(_position.X, _position.Y + sizeStep.Y) == 0)
                    _position.Y += sizeStep.Y;
            }

            // Strafe Right 
            if (pressedKeys.Contains(Keys.X))
            {
                if (Game.Map.GetBlock(_position.X - sizeStep.X, _position.Y) == 0)
                    _position.X -= sizeStep.X;

                if (Game.Map.GetBlock(_position.X, (_position.Y - sizeStep.Y)) == 0)
                    _position.Y -= sizeStep.Y;
            }

            // Rotate Left 
            if (pressedKeys.Contains(Keys.Left))
            {
                var rotated = _direction.Rotate(-_rotationSpeed);
                var strafeRotated = _strafeDirection.Rotate(-_rotationSpeed);

                _direction.X = rotated.X;
                _direction.Y = rotated.Y;
                _strafeDirection.X = strafeRotated.X;
                _strafeDirection.Y = strafeRotated.Y;

                Game.Scene.RotateCameraPlane(-_rotationSpeed);
            }

            // Rotate Right 
            if (pressedKeys.Contains(Keys.Right))
            {
                var rotated = _direction.Rotate(_rotationSpeed);
                var strafeRotated = _strafeDirection.Rotate(_rotationSpeed);

                _direction.X = rotated.X;
                _direction.Y = rotated.Y;
                _strafeDirection.X = strafeRotated.X;
                _strafeDirection.Y = strafeRotated.Y;

                Game.Scene.RotateCameraPlane(_rotationSpeed);
            }
        }
    }
}