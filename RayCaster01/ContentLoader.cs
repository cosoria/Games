using RayCaster01.Framework;
using SharpDX.Toolkit.Graphics;

namespace RayCaster01
{
    public class ContentLoader
    {
        private IGame _game;

        public ContentLoader(IGame game)
        {
            _game = game;
        }


        public void Load64x64WallTextures(Texture2D[] textureArray)
        {
            textureArray[0] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\greystone"));
            textureArray[1] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\mossy"));
            textureArray[2] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\colorstone"));
            textureArray[3] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\bluestone"));
            textureArray[4] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\redbrick"));
            textureArray[5] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\eagle"));
            textureArray[6] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\purplestone"));
            textureArray[7] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\wood"));
        }

        public void Load128x128WallTextures(Texture2D[] textureArray)
        {
            textureArray[0] = _game.TrackDisposable(_game.Content.Load<Texture2D>("128x128\\WALLA1"));
            textureArray[1] = _game.TrackDisposable(_game.Content.Load<Texture2D>("128x128\\WALLA2"));
            textureArray[2] = _game.TrackDisposable(_game.Content.Load<Texture2D>("128x128\\WALLA4"));
            textureArray[3] = _game.TrackDisposable(_game.Content.Load<Texture2D>("128x128\\WALLA5"));
            textureArray[4] = _game.TrackDisposable(_game.Content.Load<Texture2D>("128x128\\WALLA6"));
            textureArray[5] = _game.TrackDisposable(_game.Content.Load<Texture2D>("128x128\\WALLA6A"));
            textureArray[6] = _game.TrackDisposable(_game.Content.Load<Texture2D>("128x128\\WALLA6B"));
            textureArray[7] = _game.TrackDisposable(_game.Content.Load<Texture2D>("128x128\\WALLA7"));
            textureArray[7] = _game.TrackDisposable(_game.Content.Load<Texture2D>("128x128\\WALLA8"));
        }

        public void Load64x64ObjectTextures(Texture2D[] textureArray)
        {
            textureArray[0] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\barrel"));
            textureArray[1] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\pillar"));
            textureArray[2] = _game.TrackDisposable(_game.Content.Load<Texture2D>("64x64\\greenlight"));
        }

        public void LoadSkyTextures(Texture2D[] textureArray)
        {
            textureArray[0] = _game.TrackDisposable(_game.Content.Load<Texture2D>("sky"));
        }
    }
}