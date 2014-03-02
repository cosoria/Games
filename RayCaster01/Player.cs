using SharpDX;
using SharpDX.Toolkit;

namespace RayCaster01
{
    public class Player : GameObject
    {
        private Vector2 _position;
        private Vector2 _direction;

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
            _direction.X = -1;
            _direction.Y = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

    //        // Update position and direction from keyboard input 
    //        readKeys();
    ////move forward if no wall in front of you
    //if (keyDown(SDLK_UP))
    //{
    //  if(worldMap[int(posX + dirX * moveSpeed)][int(posY)] == false) posX += dirX * moveSpeed;
    //  if(worldMap[int(posX)][int(posY + dirY * moveSpeed)] == false) posY += dirY * moveSpeed;
    //}
    ////move backwards if no wall behind you
    //if (keyDown(SDLK_DOWN))
    //{
    //  if(worldMap[int(posX - dirX * moveSpeed)][int(posY)] == false) posX -= dirX * moveSpeed;
    //  if(worldMap[int(posX)][int(posY - dirY * moveSpeed)] == false) posY -= dirY * moveSpeed;
    //}
    ////rotate to the right   
    //if (keyDown(SDLK_RIGHT))
    //{
    //  //both camera direction and camera plane must be rotated
    //  double oldDirX = dirX;
    //  dirX = dirX * cos(-rotSpeed) - dirY * sin(-rotSpeed);
    //  dirY = oldDirX * sin(-rotSpeed) + dirY * cos(-rotSpeed);
    //  double oldPlaneX = planeX;
    //  planeX = planeX * cos(-rotSpeed) - planeY * sin(-rotSpeed);
    //  planeY = oldPlaneX * sin(-rotSpeed) + planeY * cos(-rotSpeed);
    //}
    ////rotate to the left
    //if (keyDown(SDLK_LEFT))
    //{
    //  //both camera direction and camera plane must be rotated
    //  double oldDirX = dirX;
    //  dirX = dirX * cos(rotSpeed) - dirY * sin(rotSpeed);
    //  dirY = oldDirX * sin(rotSpeed) + dirY * cos(rotSpeed);
    //  double oldPlaneX = planeX;
    //  planeX = planeX * cos(rotSpeed) - planeY * sin(rotSpeed);
    //  planeY = oldPlaneX * sin(rotSpeed) + planeY * cos(rotSpeed);
    //}

        }
    }
}