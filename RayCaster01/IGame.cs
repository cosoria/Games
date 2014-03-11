using System;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace RayCaster01
{
    public interface IGame
    {
        int ScreenWidth { get; }
        int ScreenHeight { get; }
        GameWindow Window { get; }
        MouseState Mouse { get; }
        KeyboardState Keyboard { get; }
        GraphicsDeviceManager DeviceManager { get; }
        GraphicsDevice Device { get; }
        ContentManager Content { get;  }

        Player Player { get; }
        Map Map { get; }
        Scene Scene { get; }
        ViewRenderer ViewRenderer { get; }

        T TrackDisposable<T>(T disposable) where T : IDisposable;
    }
}