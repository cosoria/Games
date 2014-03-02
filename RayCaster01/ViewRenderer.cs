using System.Linq.Expressions;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;


namespace RayCaster01
{
    public class ViewRenderer : GameObject
    {
        private int _width;
        private int _height;
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
        
            //var f = new SharpDX.Direct2D1.Factory();
            //var s = ((SharpDX.Direct3D11.Texture2D) Game.Device.Presenter.BackBuffer).QueryInterface<Surface>();
            //var p = new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.R32G32B32A32_Float, AlphaMode.Premultiplied));
            //_renderTarget = new RenderTarget(f, s, p);

            //var d = new SharpDX.Direct2D1.Device(dxgiDevice2);

            //// SwapChain description
            //SwapChainDescription desc = new SwapChainDescription()
            //{
            //    BufferCount = 1,
            //    ModeDescription =
            //        new ModeDescription(form.ClientSize.Width, form.ClientSize.Height,
            //                            new Rational(60, 1), Format.R8G8B8A8_UNorm),
            //    IsWindowed = true,
            //    OutputHandle = form.Handle,
            //    SampleDescription = new SampleDescription(1, 0),
            //    SwapEffect = SwapEffect.Discard,
            //    Usage = Usage.RenderTargetOutput
            //};

            //// Create Device and SwapChain
            //Device1.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, desc, FeatureLevel.Level_10_0, out device, out swapChain);

            //var d2dFactory = new SharpDX.Direct2D1.Factory();

            //// Ignore all windows events
            //Factory factory = swapChain.GetParent<Factory>();
            //factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            //// New RenderTargetView from the backbuffer
            //Texture2D backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            //RenderTargetView renderView = new RenderTargetView(device, backBuffer);

            //Surface surface = backBuffer.QueryInterface<Surface>();

            //RenderTarget d2dRenderTarget = new RenderTarget(d2dFactory, surface,
            //                                                new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));


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

            foreach (var line in Game.Scene.VerticalLines)
            {
                DrawLine(line.Start.X + 1, line.Start.Y, line.End.X + 1, line.End.Y, line.Color);
            }
            

            //DrawLine(0, 0, 100, 100, Color.Blue);
            //DrawLine(110, 100, 20, 110, Color.Yellow);
            //DrawLine(300, 150, 200, 150, Color.Aqua);
            //DrawLine(350, 100, 200, 380, Color.DarkGreen);
            //DrawLine(150, 100, 200, 100, Color.DarkOrange);

            //_renderTarget.
            
            
            _primitiveBatch.End();

           
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
    }
}