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
        private List<Line> _verticalLines;
        private List<Line> _rays;

        public List<Line> VerticalLines
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

            _plane.X = 0.0f;
            _plane.Y = 0.66f;

            _verticalLines = new List<Line>();

            for (var x = 0; x < Game.ScreenWidth; x++)
            {
                _verticalLines.Add(new Line(x, 1, x, 480));
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

            DoRayCasting1(gameTime);
        }

        private void DoRayCasting1(GameTime gameTime)
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

                var hit = CastRay(Game.Player.Position, rayDirection);

                //Calculate height of line to draw on screen
                float lineHeight = h / hit.Distance;

                //calculate lowest and highest pixel to fill in current stripe
                float drawStart = -lineHeight / 2.0f + h / 2.0f;
                if (drawStart < 0) drawStart = 0;
                float drawEnd = lineHeight / 2.0f + h / 2.0f;
                if (drawEnd >= h) drawEnd = h - 1;
                

                //draw the pixels of the stripe as a vertical line
                SetVerticalLine(x, (int)drawStart, (int)drawEnd, hit.Color);
            }
        }

        private RayHit CastRay(Vector2 playerPosition, Vector2 rayDirection)
        {
            var hit = new RayHit();
            
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

                var mapBlock = Game.Map.GetBlock(mapX, mapY);

                //Check if ray has hit a wall
                if (mapBlock > 0)
                {
                    hit.SetColor(GetWallColor(mapBlock, side));
                    hit.Side = side;
                    hit.SetHit(rayPosX, rayPosY);

                    wallhit = 1;
                }
            }

            //Calculate distance projected on camera direction (oblique distance will give fisheye effect!)
            var perpWallDist = 0.0;
            if (side == 0)
            {
                perpWallDist = (mapX - rayPosX + (1.0 - stepX) / 2.0) / rayDirX;
            }
            else
            {
                perpWallDist = (mapY - rayPosY + (1.0 - stepY) / 2.0) / rayDirY;
            }

            hit.Distance = (float)Math.Abs(perpWallDist);
            
            return hit;
        }

        private void DoRayCasting(GameTime gameTime)
        {
            float w = Game.ScreenWidth;
            float h = Game.ScreenHeight;
           
            var pos = new Vector2(Game.Player.Position.X, Game.Player.Position.Y);
            var dir = new Vector2(Game.Player.Direction.X, Game.Player.Direction.Y);

            var ray = new Vector2(0, 0);
            var rayDir = new Vector2(0, 0);
            var map = new Vector2(0, 0);
            var sideDist = new Vector2(0, 0);
            var deltaDist = new Vector2(0, 0);
       
            for (var x = 0; x < Game.ScreenWidth; x++)
            {
                float cameraX = 0.0f;

                //calculate ray position and direction 
                cameraX = ((2.0f * x) / w) - 1.0f; //x-coordinate in camera space

                // Ray start at the player position 
                ray.X = pos.X;
                ray.Y = pos.Y;
                
                map.X = ray.X;
                map.Y = ray.Y;

                _rays[x].SetStart(pos.X, pos.Y);

                // Ray direction goes from left to right using the camera plane 
                rayDir.X = dir.X + _plane.X * cameraX;
                rayDir.Y = dir.Y + _plane.Y * cameraX;

                //length of ray from current position to next x or y-side
                double sideDistX, totalDistanceX;
                double sideDistY, totalDistanceY;
                float perpWallDist;

                //length of ray from one x or y-side to next x or y-side
                deltaDist.X = (float)Math.Sqrt(1.0 + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X));
                deltaDist.Y = (float)Math.Sqrt(1.0 + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y));

                totalDistanceX = pos.X;
                totalDistanceY = pos.Y;

                //what direction to step in x or y-direction (either +1 or -1)
                int stepX;
                int stepY;

                int hit = 0; //was there a wall hit?
                int side = 0; //was a NS or a EW wall hit?

                //calculate step and initial sideDist
                if (rayDir.X < 0)
                {
                    stepX = -1;
                    sideDistX = (ray.X - map.X) * deltaDist.X;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (map.X + 1.0 - ray.X) * deltaDist.X;
                }

                if (rayDir.Y < 0)
                {
                    stepY = -1;
                    sideDistY = (ray.Y - map.Y) * deltaDist.Y;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (map.Y + 1.0 - ray.Y) * deltaDist.Y;
                }

                //perform DDA
                while (hit == 0)
                {
                    //jump to next map square, OR in x-direction, OR in y-direction
                    if (sideDistX < sideDistY)
                    {
                        totalDistanceX += deltaDist.X;
                        sideDistX += deltaDist.X;
                        map.X += stepX;
                        side = 0;
                    }
                    else
                    {
                        totalDistanceY += deltaDist.Y;
                        sideDistY += deltaDist.Y;
                        map.Y += stepY;
                        side = 1;
                    }

                    //Check if ray has hit a wall
                    if (Game.Map.GetBlock(map.X, map.Y) > 0)
                    {
                        hit = 1;
                    }
                    
                }
 
                _rays[x].SetEnd((float)totalDistanceX, (float)totalDistanceY);

                //Calculate distance projected on camera direction (oblique distance will give fisheye effect!)
                if (side == 0)
                {
                    perpWallDist = ((float)totalDistanceX - ray.X + (1.0f - stepX) / 2.0f) / rayDir.X;
                }
                else
                {
                    perpWallDist = ((float)totalDistanceY - ray.Y + (1.0f - stepY) / 2.0f) / rayDir.Y;
                }
      
                //Calculate height of line to draw on screen
                float lineHeight = h / perpWallDist;
       
                //calculate lowest and highest pixel to fill in current stripe
                float drawStart = -lineHeight / 2.0f + h / 2.0f;
                if(drawStart < 0) drawStart = 0;
                float drawEnd = lineHeight / 2.0f + h / 2.0f;
                if(drawEnd >= h) drawEnd = h - 1;
        
                //choose wall color
                Color color = GetWallColor(Game.Map.GetBlock(map.X, map.Y), side);
                
                //draw the pixels of the stripe as a vertical line
                SetVerticalLine(x, (int)drawStart, (int)drawEnd, color);
                 
            }  


        }

        public Color GetWallColor(int color, int side = 0)
        {
            if (side == 0)
            {
                switch (color)
                {
                    case 1: return Color.Red; break; //red
                    case 2: return Color.Green; break; //green
                    case 3: return Color.Blue; break; //blue
                    case 4: return Color.Orange; break; //white
                    default: return Color.Yellow; break; //yellow
                }
            }
            else
            {
                switch (color)
                {
                    case 1: return Color.DarkRed; break; //red
                    case 2: return Color.DarkGreen; break; //green
                    case 3: return Color.DarkBlue; break; //blue
                    case 4: return Color.DarkOrange; break; //white
                    default: return Color.Yellow; break; //yellow
                }
            }
            
        }

        private void SetVerticalLine(int x, int start, int end, Color color)
        {
            _verticalLines[x].Set(x, start, x, end, color);
        }
        
        public void RotateCameraPlane(float rotationAngle)
        {
            var rotated = _plane.Rotate(rotationAngle);
            
            _plane.X = rotated.X;
            _plane.Y = rotated.Y;
        }
    }

    public class RayHit
    {
        private Vector2 _hit;
        private Color _color;

        public float Distance { get; set; }

        public Vector2 Hit { get { return _hit; }}
        public Color Color { get { return _color; }}
        public int Side { get; set; }

        public RayHit()
        {
            Distance = 0.0f;
            _hit = new Vector2();
            _color = Color.White;
            Side = 0;
        }

        public void SetHit(double x, double y)
        {
            _hit.X = (float)x;
            _hit.Y = (float)y;
        }

        public void SetColor(Color color)
        {
            _color = color;
        }
    }
}