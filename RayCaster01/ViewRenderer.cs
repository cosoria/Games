﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;


namespace RayCaster01
{
    public class ViewRenderer : GameObject
    {
        private int _width;
        private int _height;
        private bool _checkF10;
        private bool _checkF11;
        private bool _checkF12;
        private bool _drawMap;
        private bool _drawRays;
        private bool _drawWalls = true;
        private SpriteBatch _spriteBatch;
        private RenderTarget _renderTarget;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private BasicEffect _basicEffect;
        private Texture2D[] _wallTextures = new Texture2D[8];
        private Texture2D[] _objectTexttures = new Texture2D[3];


        public override void Initialize(IGame game)
        {
            base.Initialize(game);
            _width = game.ScreenWidth;
            _height = game.ScreenHeight;
            
            // spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));
            // _sprite = Game.TrackDisposable(new SpriteBatch(game.Device));
            // _renderTarget = RenderTarget2D.New(game.Device, game.Device.Viewport.Width, game.Device.BackBuffer.Height, PixelFormat.R32G32B32A32.Float);
            _primitiveBatch = new PrimitiveBatch<VertexPositionColor>(game.Device);
            _basicEffect = new BasicEffect(game.Device);
            _basicEffect.VertexColorEnabled = true;
        }

        public override void LoadContent(IGame game)
        {
            base.LoadContent(game);

            _wallTextures[0] = Game.Content.Load<Texture2D>("bluestone");
            _wallTextures[1] = Game.Content.Load<Texture2D>("greystone");
            _wallTextures[2] = Game.Content.Load<Texture2D>("colorstone");
            _wallTextures[3] = Game.Content.Load<Texture2D>("purplestone");
            _wallTextures[4] = Game.Content.Load<Texture2D>("redbrick");
            _wallTextures[5] = Game.Content.Load<Texture2D>("wood");
            _wallTextures[6] = Game.Content.Load<Texture2D>("eagle");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var keys = Game.Keyboard.GetPressedKeys();

           
            _checkF10 = Game.Keyboard.IsKeyDown(Keys.F10);
            _checkF11 = Game.Keyboard.IsKeyDown(Keys.F11);
            _checkF12 = Game.Keyboard.IsKeyDown(Keys.F12);

            if (keys.Contains(Keys.F10))
            {
                _drawMap = !_drawMap;
            }

            if (keys.Contains(Keys.F11))
            {
                _drawRays = !_drawRays;
            }

            if (keys.Contains(Keys.F12))
            {
                _drawWalls = !_drawWalls;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // var presenter = this.Game.Device.Presenter;
            // var bb = presenter.BackBuffer;
            
            _basicEffect.Projection = Matrix.OrthoOffCenterRH(0, Game.Device.Viewport.Width, Game.Device.Viewport.Height, 0, 0, 1);
            _basicEffect.Alpha = 1.0f;
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            
            _primitiveBatch.Begin();

            if (_drawMap)
            {
                DrawPlayer();
                DrawMap();
            }

            if (_drawRays)
            {
                DrawRays();
            }

            if (_drawWalls)
            {
                //DrawWalls();
                DrawTexturedWall();
            }

            

            _primitiveBatch.End();
        }

        private void DrawHorizon()
        {
            var half = Game.ScreenHeight / 2;
            DrawRectangle(0, 0, Game.ScreenWidth, half, Color.SlateGray);
            DrawRectangle(0, half, Game.ScreenWidth, Game.ScreenHeight, Color.Gray);
        }

        private void DrawWalls()
        {
            float h = Game.ScreenHeight;
            float w = Game.ScreenWidth;

            foreach (var hit in Game.Scene.VerticalLines)
            {
                DrawLine(hit.VerticalLine.Start.X + 1, hit.VerticalLine.Start.Y, hit.VerticalLine.End.X + 1, hit.VerticalLine.End.Y, hit.Color);
            }

            DrawHorizon();
        }

        private void DrawTexturedWall()
        {

            // Instantiate a SpriteBatch
             var spriteBatch = Game.TrackDisposable(new SpriteBatch(Game.Device));

            spriteBatch.Begin();

            foreach (var hit in Game.Scene.VerticalLines)
            {
                var color = hit.Side == 0  ? Color.White: Color.DarkGray;
                var texture = hit.MapTexture - 1;
                var source = new Rectangle(hit.TextureLine, 0, 1, 64);
                var destination = new Rectangle((int)hit.VerticalLine.Start.X + 1, (int)hit.VerticalLine.Start.Y,
                                            1, (int)hit.VerticalLine.End.Y - (int)hit.VerticalLine.Start.Y);


                spriteBatch.Draw(_wallTextures[texture], destination, source, color, 0f, Vector2.One, SpriteEffects.None, 0f);
            }

            


            spriteBatch.End();

            //for (int y = drawStart; y < drawEnd; y++)
            //{
            //    int d = y * 256 - h * 128 + lineHeight * 128;  //256 and 128 factors to avoid floats
            //    int texY = ((d * texHeight) / lineHeight) / 256;
            //    Uint32 color = texture[texNum][texHeight * texY + texX];
            //    //make color darker for y-sides: R, G and B byte each divided through two with a "shift" and an "and"
            //    if (side == 1) color = (color >> 1) & 8355711;
            //    buffer[x][y] = color;
            //} 


            //foreach (var hit in Game.Scene.VerticalLines)
            //{
            //    DrawLine(hit.VerticalLine.Start.X + 1, hit.VerticalLine.Start.Y, hit.VerticalLine.End.X + 1, hit.VerticalLine.End.Y, hit.Color);
            //}

            //DrawHorizon();
        }

        private void DrawRays()
        {
            // Draw Player Position 
            float h = Game.ScreenHeight;
            float w = Game.ScreenWidth;
            float centerY = h/2;
            float centerX = w/2;
            float step = h / 24;
            float worldLeft = centerX - centerY;
            float worldTop = 1;
            float worldRight = centerX + centerY;
            float worldBottom = h;

            /*
            var player = new Vector2(this.Game.Player.Position.X * step + worldLeft, this.Game.Player.Position.Y * step + worldTop);
            var offset = new Vector2(player.X + (Game.Player.Direction.X * 10), player.Y + (Game.Player.Direction.Y * 10));
            DrawLine(player, offset, Color.Yellow);
            */

            foreach (var line in Game.Scene.Rays)
            {
                DrawLine(line.Start.X * step + worldLeft, 
                        line.Start.Y * step + worldTop,  
                        line.End.X * step + worldLeft,  
                        -(line.End.Y * step) + worldBottom, 
                        Color.White);

                //DrawLine(line.Start.X * step + worldLeft, 
                //         line.Start.Y * step + worldTop, 
                //         line.End.X * step + worldLeft, 
                //         line.End.Y * step + worldTop, 
                //         Color.White);
            }
        }

        private void DrawPlayer()
        {
            float h = Game.ScreenHeight;
            float w = Game.ScreenWidth;
            float centerY = h / 2;
            float centerX = w / 2;
            float step = h / 24;

            // Draw World Bounds
            float worldLeft = centerX - centerY;
            float worldTop = 1;
            float worldRight = centerX + centerY;
            float worldBottom = h;

            // Draw Player Position 
            var player = new Vector2(this.Game.Player.Position.X * step + worldLeft, this.Game.Player.Position.Y * step + worldTop);
            var offset = new Vector2(player.X + (Game.Player.Direction.X * 10), player.Y + (Game.Player.Direction.Y * 10));
            DrawLine(player, offset, Color.Yellow);
        }

        private void DrawMap()
        {
            float h = Game.ScreenHeight;
            float w = Game.ScreenWidth;
            float centerY = h/2;
            float centerX = w/2;
            float step = h / 24;

            Game.Device.Clear(Color.Black);


            var center = new Vector2(centerX, centerY);

            // Draw World Bounds
            float worldLeft = centerX - centerY;
            float worldTop = 1;
            float worldRight = centerX + centerY;
            float worldBottom = h;

                        
            DrawLine(worldLeft, worldTop, worldRight, worldTop , Color.White);
            DrawLine(worldLeft, worldBottom, worldRight, worldBottom , Color.White);
            DrawLine(worldLeft, 1, worldLeft, h, Color.White);
            DrawLine(worldRight, 1, worldRight, h, Color.White);

            // Draw World Grid
            
            float startX = worldLeft;
            float startY = worldTop;
            for (int i = 0; i < 24; i++)
            {
                startX += step;
                startY += step;
                DrawLine(worldLeft, startY, worldRight, startY, Color.Gray);
                DrawLine(startX, worldTop, startX, worldBottom, Color.Gray);
            }

            // Draw Map Walls 
            startX = worldLeft;
            startY = worldTop;
            for (int y = 0; y < 24; y++)
            {
                startX = worldLeft;
                for (int x = 0; x < 24; x++)
                {
                    if (Game.Map.GetBlock(x, y) > 0)
                    {
                        var color = Game.Scene.GetWallColor(Game.Map.GetBlock(x, y));

                        DrawLine(startX, startY, startX + step, startY + step, color);
                        DrawLine(startX + step, startY, startX, startY + step, color);
                    }
                    startX += step;
                }
                startY += step;  
                
                DrawLine(worldLeft, startY, worldRight, startY, Color.Gray);
                DrawLine(startX, worldTop, startX, worldBottom, Color.Gray);
            }

            
        }


        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color);
        }

        public void DrawLine(Vector2 v1, Vector2 v2, Color color)
        {
            var vpc1 = new VertexPositionColor(new Vector3(v1.X, v1.Y, 0), color);
            var vpc2 = new VertexPositionColor(new Vector3(v2.X, v2.Y, 0), color);

            _primitiveBatch.DrawLine(vpc1, vpc2);
        }

        private void DrawRectangle(float x, float y, float x1, float y1, Color color)
        {
            var vpc1 = new VertexPositionColor(new Vector3(x, y, 0), color); 
            var vpc2 = new VertexPositionColor(new Vector3(x1, y, 0), color); 
            var vpc3 = new VertexPositionColor(new Vector3(x1, y1, 0), color); 
            var vpc4 = new VertexPositionColor(new Vector3(x, y1, 0), color); 

            _primitiveBatch.DrawQuad(vpc1, vpc2, vpc3, vpc4);
        }
    }
}