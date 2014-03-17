using SharpDX.Toolkit;

namespace RayCaster01.Framework
{
    public interface IGameObject
    {
        void Initialize(IGame game);
        void LoadContent(IGame game);
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}