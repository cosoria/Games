﻿using System;
using System.Security.Policy;
using RayCaster01.Framework;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace RayCaster01
{
    public class Map : GameObject 
    {
        private const int MAP_WIDTH = 24;
        private const int MAP_HEIGHT = 24;

        private int[][] _map =
        {
           new []  {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
           new []  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,0,0,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
           new []  {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1},
           new []  {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
           new []  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,4,4,5,5,5,5,5,4,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1},
           new []  {1,4,0,4,0,0,0,5,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,4,0,0,0,6,0,5,4,0,0,0,0,0,0,0,0,5,0,0,0,0,0,1},
           new []  {1,4,0,4,0,0,0,5,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
           new []  {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        };


        public int MapWidth { get { return MAP_WIDTH; } }
        public int MapHeight { get { return MAP_HEIGHT; } }


        public int[][] World
        {
            get { return _map; }
        }

        public int GetBlock(float x, float y)
        {
            return GetBlock((int) x, (int) y);
        }

        public int GetBlock(int x, int y)
        {
            if (x < 0)
            {
                x = 0;
            }
            if (x > 23)
            {
                x = 23;
            }
            if (y < 0)
            {
                y = 0;
            }
            if (y > 23)
            {
                y = 23;
            }

            //if (x < 0 || x > 23 || y < 0 || y > 23)
            //{
            //    throw new ArgumentOutOfRangeException("x and y should be in range");
            //}

            return _map[x][y];
        }

        public override void LoadContent(IGame game)
        {
           

        }
    }
}