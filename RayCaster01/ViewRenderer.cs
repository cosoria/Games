using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace RayCaster01
{
    public class ViewRenderer : GameObject
    {
        private int _width;
        private int _height;
        private SpriteBatch _sprite;
        private Texture2D _texture;
        private RenderTarget2D _renderTarget;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private BasicEffect _basicEffect;


        public override void Initialize(IGame game)
        {
            base.Initialize(game);
            _width = 512;
            _height = 384;

            

            // spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));
            // _sprite = Game.TrackDisposable(new SpriteBatch(game.Device));
            // _renderTarget = RenderTarget2D.New(game.Device, game.Device.Viewport.Width, game.Device.BackBuffer.Height, PixelFormat.R32G32B32A32.Float);
            _primitiveBatch = new PrimitiveBatch<VertexPositionColor>(game.Device);
            _basicEffect = new BasicEffect(game.Device);
        }

        public override void Draw(GameTime gameTime)
        {
            _basicEffect.Projection = Matrix.OrthoOffCenterRH(0, Game.Device.Viewport.Width, Game.Device.Viewport.Height, 0, 0, 1);

            _basicEffect.CurrentTechnique.Passes[0].Apply();

            
            _primitiveBatch.Begin();
            _primitiveBatch.DrawLine(new VertexPositionColor(new Vector3(0,0,0), Color.Aquamarine),  new VertexPositionColor(new Vector3(100, 100, 0), Color.AliceBlue));
            _primitiveBatch.End();

            base.Draw(gameTime);
        }
    }
}