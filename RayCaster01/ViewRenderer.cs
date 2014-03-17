using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using RayCaster01.Framework;
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
        private Texture2D[] _wallTextures64x64 = new Texture2D[10];
        private Texture2D[] _wallTextures128x128 = new Texture2D[10];
        private Texture2D[] _objectTexttures64x64 = new Texture2D[3];
        private Texture2D[] _skyTextures = new Texture2D[2];
        private TimeSpan _lastTimeF10Pressed;
        private TimeSpan _lastTimeF11Pressed;
        private TimeSpan _lastTimeF12Pressed;
        private int _textureWidth;
        private int _textureHeight;
        private ContentLoader _contentLoader;

        public override void Initialize(IGame game)
        {
            base.Initialize(game);
            _width = game.ScreenWidth;
            _height = game.ScreenHeight;
            _textureWidth = game.TextureWidth;
            _textureHeight = game.TextureHeight;
            
            _primitiveBatch = Game.TrackDisposable(new PrimitiveBatch<VertexPositionColor>(game.Device));
            _spriteBatch = Game.TrackDisposable(new SpriteBatch(Game.Device));
            _basicEffect = new BasicEffect(game.Device);
            _basicEffect.VertexColorEnabled = true;

            _contentLoader = new ContentLoader(game);
            // _renderTarget = RenderTarget2D.New(game.Device, game.Device.Viewport.Width, game.Device.BackBuffer.Height, PixelFormat.R32G32B32A32.Float);
        }

        public override void LoadContent(IGame game)
        {
            base.LoadContent(game);

            _contentLoader.Load64x64WallTextures(_wallTextures64x64);
            _contentLoader.Load64x64ObjectTextures(_objectTexttures64x64);

            _contentLoader.Load128x128WallTextures(_wallTextures128x128);

            _contentLoader.LoadSkyTextures(_skyTextures);
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var keys = Game.Keyboard.GetPressedKeys();

            
            if (keys.Contains(Keys.F10) && (gameTime.TotalGameTime - _lastTimeF10Pressed).Milliseconds > 500)
            {
                _drawMap = !_drawMap;
                _lastTimeF10Pressed = gameTime.TotalGameTime;
            }

            if (keys.Contains(Keys.F11) && (gameTime.TotalGameTime - _lastTimeF11Pressed).Milliseconds > 500)
            {
                _drawRays = !_drawRays;
                _lastTimeF11Pressed = gameTime.TotalGameTime;
            }

            if (keys.Contains(Keys.F12) && (gameTime.TotalGameTime - _lastTimeF12Pressed).Milliseconds > 500)
            {
                _drawWalls = !_drawWalls;
                _lastTimeF12Pressed = gameTime.TotalGameTime;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _basicEffect.Projection = Matrix.OrthoOffCenterRH(0, Game.Device.Viewport.Width, Game.Device.Viewport.Height, 0, 0, 1);
            _basicEffect.Alpha = 1.0f;
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.CurrentTechnique.Passes[0].Apply();

            _primitiveBatch.Begin();

            if (_drawMap)
            {
                DrawPlayer();
                DrawMapGridLines();
            }

            if (_drawRays)
            {
                DrawRays();
            }

            if (!_drawMap && !_drawWalls)
            {
                DrawSolidWalls();
            }

            if (!_drawMap)
            {
                DrawHorizon();
            }
            

            _primitiveBatch.End();

            _spriteBatch.Begin();

            if (!_drawMap)
            {
                DrawSky();
            }

            if (_drawMap)
            {
                DrawMapWallTextures();
            }

            if (!_drawMap && _drawWalls)
            {
                DrawTexturedWall();
                
            }

           

            _spriteBatch.End();
          
        }

        private void DrawSky()
        {
            var skyTexture = _skyTextures[0];

            var dir = Game.Player.Direction;
            var angle = Math.Atan2(dir.Y, dir.X).ToDegrees();
            angle = angle < 0 ? 360 - Math.Abs(angle) : angle;

            var textX = (int) ((angle * (skyTexture.Width / 2.0))/360.0);
            var screenRect = new Rectangle(0, 0, _width, _height / 2);
            var textureRect = new Rectangle(textX, 0, _width, (_height / 2) - 100);
            var color = Color.DarkGray;

            _spriteBatch.Draw(skyTexture, screenRect, textureRect, color, 0f, Vector2.One, SpriteEffects.None, 0f);
        }

        private void DrawHorizon()
        {
            var half = _height / 2;
            DrawRectangle(0, 0, _width, half, Color.SlateGray);
            DrawRectangle(0, half, _width, _height, Color.Gray);
        }

        private void DrawSolidWalls()
        {
            foreach (var hit in Game.Scene.VerticalLines)
            {
                DrawWallStrip(hit);
            }
        }

        private void DrawTexturedWall()
        {
            foreach (var hit in Game.Scene.VerticalLines)
            {
                DrawTexturedWallStrip(hit);
                // DrawFloor(hit);
            }
        }

        private void DrawFloor(RayHit hit)
        {
            double h = Game.ScreenHeight;
            double distWall = hit.Distance;
            double distPlayer = 0.0;
            double currentDist = 0.0;
            double floorXWall = hit.Hit.X;
            double floorYWall = hit.Hit.Y;
            double posX = Game.Player.Position.X;
            double posY = Game.Player.Position.Y;
            int floorTexX, floorTexY;
      
            for (var y = hit.VerticalLine.End.Y + 1; y < h; y++)
            {
                currentDist = h / (2.0 * y - h);
                var weight = (currentDist - hit.Distance) / (distWall - distPlayer);

                double currentFloorX = weight * floorXWall + (1.0 - weight) * posX;
                double currentFloorY = weight * floorYWall + (1.0 - weight) * posY;
          
                floorTexX = (int)((currentFloorX * _textureWidth) % _textureWidth);
                floorTexY = (int)((currentFloorY * _textureHeight) % _textureHeight);

                DrawTextel(0, hit.VerticalLine.End.X, y, floorTexX, floorTexY);
            }
           
        }

        private void DrawRays()
        {
            // Draw Player Position 
            float h = _height;
            float w = _width;
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

        private void DrawMapWallTextures()
        {
            float h = _height;
            float w = _width;
            float mapWidth = this.Game.Map.MapWidth;
            float mapHeight = this.Game.Map.MapHeight;
            float centerY = h / 2;
            float centerX = w / 2;
            float stepX = w / mapWidth;
            float stepY = h / mapWidth;
            
            var center = new Vector2(centerX, centerY);

            // Draw World Bounds
            float worldLeft = centerX - centerY;
            float worldTop = 1;
            float worldRight = centerX + centerY;
            float worldBottom = h;

            float startX = worldLeft;
            float startY = worldTop;


            // Draw Map Walls 
            startX = worldLeft;
            startY = worldTop;
            for (int y = 0; y < mapHeight; y++)
            {
                startX = worldLeft;
                for (int x = 0; x < mapWidth; x++)
                {
                    if (Game.Map.GetBlock(x, y) > 0)
                    {
                        var color = Game.Map.GetBlock(x, y);
                        var texture = color - 1;

                        var screen = new Rectangle((int)startX, (int)startY, (int)stepX, (int)stepY);
                        var textureRect = new Rectangle(0, 0, 64, 64);
                        DrawTexture(texture, screen, textureRect, 0);
                        //DrawLine(startX, startY, startX + step, startY + step, color);
                        //DrawLine(startX + step, startY, startX, startY + step, color);
                    }
                    startX += stepX;
                }
                startY += stepY;

                //DrawLine(worldLeft, startY, worldRight, startY, Color.Gray);
                //DrawLine(startX, worldTop, startX, worldBottom, Color.Gray);
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

        private void DrawWallStrip(RayHit hit)
        {
            DrawLine(hit.VerticalLine.Start.X + 1, hit.VerticalLine.Start.Y, hit.VerticalLine.End.X + 1, hit.VerticalLine.End.Y, hit.Color);
        }

        private void DrawTexturedWallStrip(RayHit hit)
        {
            var texture = hit.MapTexture - 1;
            var source = new Rectangle(hit.TextureLine, 0, 1, _textureHeight);
            var destination = new Rectangle((int)hit.VerticalLine.Start.X + 1, (int)hit.VerticalLine.Start.Y,
                                        1, (int)hit.VerticalLine.End.Y - (int)hit.VerticalLine.Start.Y);

            DrawTexture(texture, destination, source, hit.Side);
        }

        private void DrawMapGridLines()
        {
            float h = Game.ScreenHeight;
            float w = Game.ScreenWidth;
            float mapWidth = this.Game.Map.MapWidth;
            float mapHeight = this.Game.Map.MapHeight;
            float centerY = h/2;
            float centerX = w/2;
            float step = h / mapWidth;


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
            for (int i = 0; i < mapWidth; i++)
            {
                startX += step;
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

        private void DrawTexture(int textureIndex, Rectangle screenRect, Rectangle textureRect, int side)
        {
            var color = side == 0 ? Color.White : Color.DarkGray;
            var texture = GetTexture(textureIndex);
            _spriteBatch.Draw(texture, screenRect, textureRect, color, 0f, Vector2.One, SpriteEffects.None, 0f);
        }

        private void DrawTextel(int textureIndex, float screenX, float screenY, float textureX, float textureY)
        {
            var screenRect = new Rectangle((int)screenX, (int)screenY, 1, 1);
            var textureRect = new Rectangle((int)textureX, (int)textureY, 1, 1);
            var color = Color.White;
            var texture = GetTexture(textureIndex);
            _spriteBatch.Draw(texture, screenRect, textureRect, color, 0f, Vector2.One, SpriteEffects.None, 0f);
        }

        private Texture2D GetTexture(int index)
        {
            if (Game.TextureWidth == 128)
            {
                return _wallTextures128x128[index];
            }

            
            return _wallTextures64x64[index];
        }
    }
}