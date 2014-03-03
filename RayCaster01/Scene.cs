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

            DoRayCasting(gameTime);
        }

        private void DoRayCasting(GameTime gameTime)
        {
            var w = Game.ScreenWidth;
            var h = Game.ScreenHeight;
            var camera = new Vector2(0, 0);
            
            var pos = new Vector2(Game.Player.Position.X, Game.Player.Position.Y);
            var dir = new Vector2(Game.Player.Direction.X, Game.Player.Direction.Y);

            var ray = new Vector2(0, 0);
            var rayDir = new Vector2(0, 0);
            var map = new Vector2(0, 0);
            var sideDist = new Vector2(0, 0);
            var deltaDist = new Vector2(0, 0);
       

            double cameraX = 0;

            for (var x = 0; x < Game.ScreenWidth; x++)
            {
                //calculate ray position and direction 
                camera.X = ((2.0f * x) / w) - 1.0f; //x-coordinate in camera space

                // Ray start at the player position 
                ray.X = pos.X;
                ray.Y = pos.Y;
                
                map.X = ray.Y;
                map.Y = ray.X;

                _rays[x].SetStart(map.Y, map.X);

                // Ray direction goes from left to right using the camera plane 
                rayDir.X = dir.X + _plane.X * camera.X;
                rayDir.Y = dir.Y + _plane.Y * camera.X;

                //length of ray from current position to next x or y-side
                double sideDistX;
                double sideDistY;
                double perpWallDist;

                //length of ray from one x or y-side to next x or y-side
                deltaDist.X = (float)Math.Sqrt(1.0 + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X));
                deltaDist.Y = (float)Math.Sqrt(1.0 + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y));
                

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
                        sideDistX += deltaDist.X;
                        map.X += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDist.Y;
                        map.Y += stepY;
                        side = 1;
                    }

                    //Check if ray has hit a wall
                    //if (Game.Map.World[(int) map.X, (int) map.Y] > 0)
                    if (Game.Map.World[(int) map.Y, (int) map.X] > 0)
                    {
                        hit = 1;
                    }
                    
                }
 
                _rays[x].SetEnd(map.Y, map.X);

                //Calculate distance projected on camera direction (oblique distance will give fisheye effect!)
                if (side == 0)
                {
                    perpWallDist = Math.Abs((map.X - ray.X + (1 - stepX) / 2) / rayDir.X);
                }
                else
                {
                    perpWallDist = Math.Abs((map.Y - ray.Y + (1 - stepY) / 2) / rayDir.Y);
                    
                }
      
                //Calculate height of line to draw on screen
                int lineHeight = (int)Math.Abs(h / perpWallDist);
       
                //calculate lowest and highest pixel to fill in current stripe
                int drawStart = -lineHeight / 2 + h / 2;
                if(drawStart < 0) drawStart = 0;
                int drawEnd = lineHeight / 2 + h / 2;
                if(drawEnd >= h) drawEnd = h - 1;
        
                //choose wall color
                // Color color = GetMapColor(Game.Map.World[(int) map.X, (int) map.Y]);
                Color color = GetMapColor(Game.Map.World[(int) map.Y, (int) map.X], side);
                
       
                //give x and y sides different brightness
                //if (side == 1)
                //{
                //    color.R = color.R / 2;
                //}

                //draw the pixels of the stripe as a vertical line
                SetVerticalLine(x, drawStart, drawEnd, color);
                 
            }  


        }

        public Color GetMapColor(int color, int side = 0)
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

        public void RotatePlaneRight()
        {
            double oldPlaneX = _plane.X;
            double oldPlaneY = _plane.Y;

            
            _plane.X = (float)(oldPlaneX * Math.Cos(-Game.Player.RotationSpeed) - oldPlaneY * Math.Sin(-Game.Player.RotationSpeed));
            _plane.Y = (float)(oldPlaneX * Math.Sin(-Game.Player.RotationSpeed) + oldPlaneY * Math.Cos(-Game.Player.RotationSpeed));
        }

        public void RotatePlaneLeft()
        {
            //  double oldPlaneX = planeX;
            //  planeX = planeX * cos(rotSpeed) - planeY * sin(rotSpeed);
            //  planeY = oldPlaneX * sin(rotSpeed) + planeY * cos(rotSpeed);
            double oldPlaneX = _plane.X;
            double oldPlaneY = _plane.Y;


            _plane.X = (float)(oldPlaneX * Math.Cos(Game.Player.RotationSpeed) - oldPlaneY * Math.Sin(Game.Player.RotationSpeed));
            _plane.Y = (float)(oldPlaneX * Math.Sin(Game.Player.RotationSpeed) + oldPlaneY * Math.Cos(Game.Player.RotationSpeed));
        }
    }
}