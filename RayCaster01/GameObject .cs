using SharpDX.Toolkit;

namespace RayCaster01
{
    public class GameObject : IGameObject
    {
        private IGame _game;

        public IGame Game
        {
            get { return _game; }
        }

        public virtual void Initialize(IGame game)
        {
            _game = game;
        }

        public virtual void LoadContent(IGame game)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
        }
    }
}