using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using SharpDX;
using SharpDX.Toolkit;

namespace RayCaster01
{
    public class Scene : GameObject
    {
        private Vector2 _plane;
        private List<RayHit> _verticalLines;
        private List<Line> _rays;

        public List<RayHit> VerticalLines
        {
            get { return _verticalLines; }
        }

        public List<Line> Rays
        {
            get { return _rays; }
        }

        public override void Initialize(IGame game)
        {
            base.Initialize(game);

            // Rotate player direction 90 degrees to obtain the camera plane 
            var rotated = Game.Player.Direction.Rotate((float) Math.PI/2);
            // Will not multiply that by .66 to obtain the right FOV 
            var plane = Vector2.Multiply(rotated, 0.66f);

            // _plane.X = 0.0f;
            _plane.X = plane.X;
            // _plane.Y = 0.66f;
            _plane.Y = plane.Y;

            _verticalLines = new List<RayHit>();

            for (var x = 0; x < Game.ScreenWidth; x++)
            {
                _verticalLines.Add(new RayHit());
            }

            _rays = new List<Line>();

            for (var x = 0; x < Game.ScreenWidth; x++)
            {
                _rays.Add(new Line(0, 0, 0, 0));
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            DoRayCasting(gameTime);
        }

        private void DoRayCasting(GameTime gameTime)
        {
            float w = Game.ScreenWidth;
            float h = Game.ScreenHeight;
            var rayDirection = new Vector2();
            float cameraX = 0.0f;

            for (var x = 0; x < w; x++)
            {
                //calculate ray position and direction 
                cameraX = ((2.0f * x) / w) - 1.0f; //x-coordinate in camera space

                // Ray direction goes from left to right using the camera plane 
                rayDirection.X = Game.Player.Direction.X + _plane.X * cameraX;
                rayDirection.Y = Game.Player.Direction.Y + _plane.Y * cameraX;

                var hit = CastRay(x, Game.Player.Position, rayDirection);
            

                _rays[x].Set(Game.Player.Position, hit.Hit, Color.White);
               
            }
        }

        private RayHit CastRay(int rayIndex, Vector2 playerPosition, Vector2 rayDirection)
        {
            var hit = _verticalLines[rayIndex];
            
            var rayPosX = (double)playerPosition.X;
            var rayPosY = (double)playerPosition.Y;
            var rayDirX = (double)rayDirection.X;
            var rayDirY = (double)rayDirection.Y;

            var mapX = (int) rayPosX;
            var mapY = (int) rayPosY;

            //length of ray from one x or y-side to next x or y-side
            var deltaDistX = Math.Sqrt(1.0 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
            var deltaDistY = Math.Sqrt(1.0 + (rayDirX * rayDirX) / (rayDirY * rayDirY));

            var stepX = 0; 
            var stepY = 0;

            var wallhit = 0;
            var side = 0;

            var sideDistX = 0.0;
            var sideDistY = 0.0;

            //calculate step and initial sideDist
            if (rayDirX < 0)
            {
                stepX = -1;
                sideDistX = (rayPosX - mapX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                sideDistX = (mapX + 1.0 - rayPosX) * deltaDistX;
            }

            if (rayDirY < 0)
            {
                stepY = -1;
                sideDistY = (rayPosY - mapY) * deltaDistY;
            }
            else
            {
                stepY = 1;
                sideDistY = (mapY + 1.0 - rayPosY) * deltaDistY;
            }

            //perform DDA
            while (wallhit == 0)
            {
                //jump to next map square, OR in x-direction, OR in y-direction
                if (sideDistX < sideDistY)
                {
                    sideDistX += deltaDistX;
                    mapX += stepX;
                    side = 0;
                }
                else
                {
                    sideDistY += deltaDistY;
                    mapY += stepY;
                    side = 1;
                }

                //Check if ray has hit a wall
                if (Game.Map.GetBlock(mapX, mapY) > 0)
                {
                    wallhit = 1;
                }
            }

            var mapBlock = Game.Map.GetBlock(mapX, mapY);
            hit.MapTexture = mapBlock;
            hit.SetMapCoordinates(mapX, mapY);
            hit.SetColor(GetWallColor(mapBlock, side));
            hit.Side = side;
            hit.SetHit(sideDistX, sideDistY);
            double wallX;

            //Calculate distance projected on camera direction (oblique distance will give fisheye effect!)
            var perpWallDist = 0.0;
            if (side == 0)
            {
                perpWallDist = (mapX - rayPosX + (1.0 - stepX) / 2.0) / rayDirX;
                
                wallX = rayPosY + ((mapX - rayPosX + (1 - stepX) / 2.0) / rayDirX) * rayDirY;
                
            }
            else
            {
                perpWallDist = (mapY - rayPosY + (1.0 - stepY) / 2.0) / rayDirY;

                wallX = rayPosX + ((mapY - rayPosY + (1 - stepY) / 2.0) / rayDirY) * rayDirX;
                
            }

            hit.TextureLine = (int) ((wallX - Math.Floor(wallX))*64);

            //if (Math.Abs(wallX - Math.Floor(wallX)) < 0.01)
            //{
            //    hit.SetColor(Color.White);
            //}

            wallX = Math.Floor(wallX);
            hit.Distance = (float)Math.Abs(perpWallDist);

            float w = Game.ScreenWidth;
            float h = Game.ScreenHeight;

            //Calculate height of line to draw on screen
            float lineHeight = h / hit.Distance;

            //calculate lowest and highest pixel to fill in current stripe
            float drawStart = -lineHeight / 2.0f + h / 2.0f;
            if (drawStart < 0) drawStart = 0;
            float drawEnd = lineHeight / 2.0f + h / 2.0f;
            if (drawEnd >= h) drawEnd = h - 1;

            hit.VerticalLine.Set(rayIndex, drawStart, rayIndex, drawEnd, hit.Color);
            
            return hit;
        }

        public Color GetWallColor(int color, int side = 0)
        {
            Color wallColor;

            switch (color)
            {
                case 1: wallColor = Color.DarkRed; break; //red
                case 2: wallColor = Color.Green; break; //green
                case 3: wallColor = Color.Blue; break; //blue
                case 4: wallColor= Color.DarkOrange; break; //white
                default: wallColor = Color.DarkOliveGreen; break; //yellow
            }

            if (side == 1)
            {
                wallColor.R = (byte)(wallColor.R / 2);
                wallColor.G = (byte)(wallColor.G / 2);
                wallColor.B = (byte)(wallColor.B / 2);
            }

            return wallColor;
        }

        
        public void RotateCameraPlane(float rotationAngle)
        {
            var rotated = _plane.Rotate(rotationAngle);
            
            _plane.X = rotated.X;
            _plane.Y = rotated.Y;
        }
    }
}