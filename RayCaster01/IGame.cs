using System;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace RayCaster01
{
    public interface IGame
    {
        GameWindow Window { get; }
        MouseState Mouse { get; }
        KeyboardState Keyboard { get; }
        GraphicsDeviceManager DeviceManager { get; }
        GraphicsDevice Device { get; }

        Player Player { get; }
        Map Map { get; }
        ViewRenderer ViewRenderer { get; }

        T TrackDisposable<T>(T disposable) where T : IDisposable;
    }
}