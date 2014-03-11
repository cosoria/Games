using System;
using System.Linq;
using System.Linq.Expressions;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;


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
                DrawWalls();
            }

            DrawHorizon();

            _primitiveBatch.End();
        }

        private void DrawHorizon()
        {
            var half = Game.ScreenHeight / 2;
            DrawRectangle(0, 0, Game.ScreenWidth, half, Color.LightGray);
            DrawRectangle(0, half, Game.ScreenWidth, Game.ScreenHeight, Color.SlateGray);
        }

        private void DrawWalls()
        {
            float h = Game.ScreenHeight;
            float w = Game.ScreenWidth;

            foreach (var line in Game.Scene.VerticalLines)
            {
                DrawLine( w - line.Start.X + 1, line.Start.Y,  w - line.End.X + 1, line.End.Y, line.Color);
            }
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