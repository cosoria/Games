// ReSharper disable InconsistentNaming

using System;
using SharpDX;


namespace RayCaster01
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    /// <summary>
    /// Simple RayCaster01 game using SharpDX.Toolkit.
    /// </summary>
    public class RayCaster01 : Game, IGame
    {

        public const int SCREEN_WIDTH = 640;
        public const int SCREEN_HEIGHT = 480;

        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        
        private readonly KeyboardManager _keyboardManager;
        private KeyboardState _keyboardState;

        private readonly MouseManager _mouse;
        private MouseState _mouseState;

        private readonly Map _map;
        private readonly Player _player;
        private readonly ViewRenderer _viewRenderer;
        private readonly Scene _scene;

        public int ScreenWidth { get { return SCREEN_WIDTH; } }
        public int ScreenHeight { get { return SCREEN_HEIGHT; } }
        public MouseState Mouse { get { return _mouseState; } }
        public KeyboardState Keyboard { get { return _keyboardState; } }
        public GraphicsDeviceManager DeviceManager { get { return _graphicsDeviceManager; } }
        public GraphicsDevice Device { get { return GraphicsDevice;  }}

        public Map Map { get { return _map; }}
        public Player Player { get { return _player; } }
        public ViewRenderer ViewRenderer { get { return _viewRenderer; } }
        public Scene Scene { get { return _scene; } }

        public T TrackDisposable<T>(T disposable) where T : IDisposable
        {
            return ToDisposeContent(disposable);
        }
        
        private SpriteBatch spriteBatch;
        private Texture2D ballsTexture;
        private SpriteFont arial16Font;

        
        public RayCaster01()
        {
            var viewportSize = new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT);
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int) viewportSize.X,
                PreferredBackBufferHeight = (int) viewportSize.Y,
                SynchronizeWithVerticalRetrace = false
            };


            _keyboardManager = new KeyboardManager(this);
            _mouse = new MouseManager(this);

            Content.RootDirectory = "Content";

            // Game Objects 
            _map = new Map();
            _player = new Player();
            _scene = new Scene();
            _viewRenderer = new ViewRenderer();
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Modify the title of the window
            Window.Title = "RayCaster Version 0.0.1";
            Window.IsMouseVisible = true;
            Window.AllowUserResizing = false;
            
            _map.Initialize(this);
            _player.Initialize(this);
            _scene.Initialize(this);
            _viewRenderer.Initialize(this);
        }

        protected override void LoadContent()
        {
            // Instantiate a SpriteBatch
            // spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));

            // Loads the balls texture (32 textures (32x32) stored vertically => 32 x 1024 ).
            // The [Balls.dds] file is defined with the build action [ToolkitTexture] in the project
            // ballsTexture = Content.Load<Texture2D>("Balls");

            // Loads a sprite font
            // The [Arial16.xml] file is defined with the build action [ToolkitFont] in the project
            // arial16Font = Content.Load<SpriteFont>("Arial16");

            base.LoadContent();

            _map.LoadContent(this);
            _player.LoadContent(this);
            _scene.LoadContent(this);
            _viewRenderer.LoadContent(this);
            
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            // Get the current state of the _keyboardManager
            _keyboardState = _keyboardManager.GetState();

            // Get the current state of the _mouse
            _mouseState = _mouse.GetState();

            _map.Update(gameTime);
            _player.Update(gameTime);
            _scene.Update(gameTime);
            _viewRenderer.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Use time in seconds directly
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.Black);

            _map.Draw(gameTime);
            _player.Draw(gameTime);
            _scene.Draw(gameTime);
            _viewRenderer.Draw(gameTime);
            
            base.Draw(gameTime);
            
            /*
            // ------------------------------------------------------------------------
            // Draw the some 2d text
            // ------------------------------------------------------------------------
            spriteBatch.Begin();
            var text = new StringBuilder("This text is displayed with SpriteBatch").AppendLine();

            // Display pressed keys
            var pressedKeys = _keyboardState.GetPressedKeys();
            text.Append("Key Pressed: [");
            foreach (var key in pressedKeys)
            {
                text.Append(key.ToString());
                text.Append(" ");
            }
            text.Append("]").AppendLine();

            // Display _mouse coordinates and _mouse button status
            text.AppendFormat("Mouse ({0},{1}) Left: {2}, Right {3}", _mouseState.X, _mouseState.Y, _mouseState.Left, _mouseState.Right).AppendLine();

            spriteBatch.DrawString(arial16Font, text.ToString(), new Vector2(16, 16), Color.White);
            spriteBatch.End();

            // ------------------------------------------------------------------------
            // Use SpriteBatch to draw some balls on the screen using NonPremultiplied mode
            // as the sprite texture used is not premultiplied
            // ------------------------------------------------------------------------
            spriteBatch.Begin(SpriteSortMode.Deferred, GraphicsDevice.BlendStates.NonPremultiplied);
            for (int i = 0; i < 40; i++)
            {
                var posX = (float)Math.Cos(time * 4.5f + i * 0.1f) * 60.0f + 136.0f;
                var posY = GraphicsDevice.BackBuffer.Height * 2.0f / 3.0f + 100.0f * (float)Math.Sin(time * 10.0f + i * 0.4f);

                spriteBatch.Draw(
                    ballsTexture,
                    new Vector2(posX, posY),
                    new Rectangle(0, 0, 32, 32),
                    Color.White,
                    0.0f,
                    new Vector2(16, 16),
                    Vector2.One,
                    SpriteEffects.None,
                    0f);
            }
            spriteBatch.End();
            */


            
        }
    }
}
